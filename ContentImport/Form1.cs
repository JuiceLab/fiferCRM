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
using LoaderService.Excel;

namespace ContentImport
{
    public partial class Form1 : Form
    {
        DemoDataLoader loader = new DemoDataLoader(Guid.Empty);

        public Form1()
        {
            InitializeComponent();
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.WorkerSupportsCancellation = true;
            backgroundWorker2.WorkerReportsProgress = true;
            backgroundWorker2.WorkerSupportsCancellation = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            loader.ChangeProcess += new ProgressChangedEventHandler(backgroundInternalWorker1_ProgressChanged);
            backgroundWorker1.RunWorkerAsync();
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

        private void ClearDublicateCities()
        {
            using (CRMEntities context = new CRMEntities(AccessSettings.LoadSettings().CrmEntites))
            {
                var regions = context.Districts.ToList();
                var items = context.Cities.Where(m => !m.C_DistrictId.HasValue).ToList();
                
                foreach (var item in regions)
                {
                      var cities = context.Cities.Where(m=>m.C_DistrictId == item.DistrictId )
                          .ToList()
                          .GroupBy(m=>m.Name + m.CityPrefix)
                          .ToDictionary(m=>m.FirstOrDefault(), m=>m.Skip(1).ToList());
                      foreach (var dublicate in cities)
                      {
                          foreach (var city in dublicate.Value)
                          {
                              context.Cities.Remove(city);
                          }
                          var curCities = items.Where(m =>m.CityId != dublicate.Key.CityId && dublicate.Key.Name ==  m.Name).ToList();
                          if (curCities.Count > 0)
                          {
                              var ids= curCities.Select(m=>m.CityId).ToList();

                              using (CRMEntities context4Update = new CRMEntities(AccessSettings.LoadSettings().CrmEntites))
                              {
                                  foreach (var geoExist in context4Update.Cities.Where(m => ids.Contains(m.CityId)).SelectMany(m=>m.GeoLocations).Distinct().ToList())
                                  {
                                      geoExist.C_CityId = dublicate.Key.CityId;
                                  }
                                  foreach (var city4Delete in curCities)
                                  {
                                      context.Cities.Remove(city4Delete);
                                  }
                                  context.SaveChanges();
                                  context4Update.SaveChanges();
                              }
                          }
                      }
                      context.SaveChanges();
                }
            }
        }


        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {

                this.textBox1.Clear();
                this.textBox1.AppendText((string)e.UserState + "\r\n");
            this.progressBar1.Value = e.ProgressPercentage;
        
            }
            catch (Exception)
            {
            }
        }

        private void backgroundInternalWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            backgroundWorker1.ReportProgress(e.ProgressPercentage, e.UserState);
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] files4Update = Directory.GetFiles(@textBox2.Text);
            foreach (var file4Update in files4Update)
            {
                FileInfo file = new FileInfo(file4Update);
                if (file.LastWriteTimeUtc.Date > DateTime.Now.Date.AddDays(-7))
                {
                    label1.Invoke((MethodInvoker)delegate
                    {
                        label2.Text = Path.GetFileNameWithoutExtension(file4Update);
                    });
                    try
                    {
                        using (var streamReader = new FileStream(file4Update, FileMode.Open))
                        {
                            loader.RefreshCRMCompanies( streamReader,Path.GetFileNameWithoutExtension(file4Update), true);
                        }
                    }
                    catch (IOException)
                    {
                    }
                }
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync(); 

        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            ClearDublicateCities();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Close();
        }
    }
}
