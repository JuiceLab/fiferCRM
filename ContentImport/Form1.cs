using CRMContext;
using CRMModel;
using Excel;
using Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EntityRepository;

namespace ContentImport
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int size = -1;
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    using (var streamReader = new FileStream(file, FileMode.Open))
                    {
                        using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(streamReader))
                        {
                            excelReader.IsFirstRowAsColumnNames = false;
                            DataSet resultSet = excelReader.AsDataSet();
                            UpdateCities(resultSet);
                        }
                    }
                }
                catch (IOException)
                {
                }
            }
        }

        private void UpdateCities(DataSet result)
        {
            var data = result.Tables[0];
            var dt = new DataTable("Протокол загрузки");
            dt.Load(data.CreateDataReader());
            CultureInfo cultureInfo = new CultureInfo("en-US");
            using (CRMEntities context = new CRMEntities(AccessSettings.LoadSettings().CrmEntites))
            {
                var cities = context.Cities.ToList();
                var regions = context.Districts.ToList();
                List<CityPreview> items = new List<CityPreview>();
                foreach (DataRow item in dt.Rows)
                {
                    var rowVal = item.ItemArray;
                    try
                    {
                        var name = rowVal.ElementAt(1).ToString();
                        var prefix = rowVal.ElementAt(2).ToString();
                        var code = rowVal.ElementAt(9).ToString();
                        var regionCode = rowVal.ElementAt(4).ToString();
                        items.Add(new CityPreview()
                        {
                            Code = Convert.ToInt32(code),
                            Name = name,
                            Prefix = prefix,
                            RegionCode = Convert.ToInt32(regionCode)
                        });
                    }
                    catch { }
                }
                var fileRegions = items.GroupBy(m => m.RegionCode)
                    .ToDictionary(m => m.FirstOrDefault(), m => m.Skip(1).Where(n => n.Prefix == "р-н" || n.Prefix == "у"));
                foreach (var region in fileRegions)
                {
                    if (!regions.Any(m => m.Name == region.Key.Name + " " + region.Key.Prefix))
                    {
                        var noveltyRegion = new District()
                        {
                            Name = region.Key.Name + " " + region.Key.Prefix,
                            Code = region.Key.RegionCode
                        };
                        context.InsertUnit(noveltyRegion);
                        regions.Add(noveltyRegion);
                    }

                }

                var fileCities = items.GroupBy(m => m.RegionCode)
                     .ToDictionary(m => m.FirstOrDefault(), m => m.Skip(1).Where(n => n.Prefix != "р-н" && n.Prefix != "у"));
                foreach (var item in fileCities)
                {
                    var district = regions.FirstOrDefault(m => m.Code == item.Key.RegionCode);
                    if (district == null)
                    {
                        var code = item.Key.Code.ToString();
                        if (!cities.Any(m => m.Name == item.Key.Name && m.CityPrefix == item.Key.Prefix && m.Code == code))
                        {
                            var city = new City()
                            {

                                Code = code,
                                Name = item.Key.Name,
                                CityPrefix = item.Key.Prefix,
                            };
                            context.InsertUnit(city);
                            cities.Add(city);
                        }
                    }
                    int? districtId = district != null ? district.DistrictId : new Nullable<int>();

                    foreach (var city in item.Value)
                    {
                        var code = city.Code.ToString();
                        if (cities.Any(m => (!districtId.HasValue || m.C_DistrictId == districtId.Value)
                             && m.Name == city.Name
                             && string.IsNullOrEmpty(m.CityPrefix)))
                        {

                            var existCity = cities.FirstOrDefault(m => (!districtId.HasValue || m.C_DistrictId == districtId.Value)
                               && m.Name == city.Name
                               && string.IsNullOrEmpty(m.CityPrefix));
                            existCity.CityPrefix = city.Prefix;
                            existCity.Code = city.Code.ToString();
                            var dbCity = context.GetUnitById<City>(existCity.CityId);
                            dbCity.CityPrefix = city.Prefix;
                            dbCity.Code = city.Code.ToString();
                        }
                        else if (!cities.Any(m => (!districtId.HasValue || m.C_DistrictId == districtId.Value)
                            && m.Name == city.Name
                            && m.CityPrefix == city.Prefix
                            && m.Code == code))
                        {
                            var noveltyCity = new City()
                            {
                                C_DistrictId = districtId,
                                Code = code,
                                Name = city.Name,
                                CityPrefix = city.Prefix,
                            };
                            context.Cities.Add(noveltyCity);
                            cities.Add(noveltyCity);
                        }



                    }
                    context.SaveChanges();

                }
            }
        }
    }
}
