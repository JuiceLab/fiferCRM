using CRMContext;
using EnumHelper;
using Excel;
using OfficeOpenXml;
using Settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EntityRepository;
using System.Globalization;

namespace LoaderService.Excel
{
    public enum DemoDataType : byte
    {
        [StringValue("Компании")]
        Compaines = 1,
    }
    public class DemoDataLoader
    {
        private Guid _userId;

        public DemoDataLoader(Guid userId)
        {
            _userId = userId;
        }

        public byte[] RefreshCRMCompanies(FileStream stream, bool isFirstLoad = false)
        {
            byte[] result = null;
            
                using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
                {
                    excelReader.IsFirstRowAsColumnNames = false;
                    DataSet resultSet = excelReader.AsDataSet();
                    if (isFirstLoad)
                        result = CreateDemoData(resultSet);
                    else
                        result = UpdateDemoData(resultSet);
                }
                stream.Close();
           
            return result;
            //using (var pck = new ExcelPackage())
            //{
            //    pck.Workbook.Worksheets.Add(dt.TableName);
            //    ExcelWorksheet ws = pck.Workbook.Worksheets[dt.TableName];
            //    ws.Cells.LoadFromDataTable(dt, true);
            //    output = pck.GetAsByteArray();
            //}
        }

        private byte[] CreateDemoData(DataSet result)
        {
            var data = result.Tables[0];
            var dt = new DataTable("Протокол загрузки");
            dt.Columns.Add("№");
            dt.Columns.Add("НАЗВАНИЕ");
            dt.Columns.Add("РУБРИКА");
            dt.Columns.Add("ПОДРУБРИКА");
            dt.Columns.Add("РЕГИОН");
            dt.Columns.Add("ГОРОД");
            dt.Columns.Add("АДРЕС");
            dt.Columns.Add("ТЕЛЕФОН1");
            dt.Columns.Add("ТЕЛЕФОН2");
            dt.Columns.Add("ТЕЛЕФОН3");
            dt.Columns.Add("СОТОВЫЙ");
            dt.Columns.Add("EMAIL");
            dt.Columns.Add("САЙТ");
            dt.Columns.Add("Широта");
            dt.Columns.Add("Долгота");
            dt.Load(data.CreateDataReader());
            CultureInfo cultureInfo = new CultureInfo("en-US");
            using (CRMEntities context = new CRMEntities(AccessSettings.LoadSettings().CrmEntites))
            {
                var cities = context.Cities.ToList();
                var regions = context.Districts.ToList();
                var categories = context.CompanyServices.ToList();
                var names = context.LegalEntities.Select(m => m.PublicName).ToList();
                var geoLocations = context.GeoLocations.ToList();
                foreach (DataRow item in dt.Rows)
                {
                    var rowVal = item.ItemArray;
                    try
                    {
                        var name = rowVal.ElementAt(16).ToString();
                        if (name.Length > 128)
                            name = name.Substring(128);
                        var category = rowVal.ElementAt(17).ToString();
                        if(category.Length > 128)
                            category = category.Substring(128);
                        var subCategory = rowVal.ElementAt(18).ToString();
                        if(subCategory.Length > 128)
                            subCategory = subCategory.Substring(128);
                        var regionName = rowVal.ElementAt(19).ToString();
                        var cityName = rowVal.ElementAt(20).ToString();

                        var address = rowVal.ElementAt(21).ToString();
                        var phones = string.Join(",", rowVal.ElementAt(22).ToString(), rowVal.ElementAt(23).ToString(), rowVal.ElementAt(24).ToString(), rowVal.ElementAt(25).ToString());

                        var email = rowVal.ElementAt(26).ToString();
                        var webSite = rowVal.ElementAt(27).ToString();


                        var latitude = Convert.ToDouble(rowVal.ElementAt(28).ToString(), cultureInfo);
                        var longitude = Convert.ToDouble(rowVal.ElementAt(29).ToString(), cultureInfo);

                        var city = cities.FirstOrDefault(m => m.Name == cityName);
                        var region = regions.FirstOrDefault(m => m.Name == regionName);

                        var geo = geoLocations.FirstOrDefault(m => m.Latitude == latitude && m.Longitude == m.Longitude);
                        var categoryParent = categories.FirstOrDefault(m => m.Name == category);
                        var categoryChild = categories.FirstOrDefault(m => m.Name == subCategory);
                        var companyExist = names.Any(m => m == name);

                        if (categoryParent == null)
                        {
                            categoryParent = new CompanyService()
                            {
                                Name = category

                            };
                            context.InsertUnit(categoryParent);
                            categories.Add(categoryParent);
                        }

                        if (categoryChild == null)
                        {
                            categoryChild = new CompanyService()
                            {
                                C_ParentId = categoryParent.CompanyServiceId,
                                Name = subCategory

                            };
                            context.InsertUnit(categoryChild);
                            categories.Add(categoryChild);
                        }


                        if (city == null)
                        {
                            city = new City()
                            {
                                Name = cityName,
                                Code = string.Empty
                            };
                            context.InsertUnit(city);
                            cities.Add(city);
                        }

                        if (geo == null)
                        {
                            geo = new GeoLocation()
                            {
                                Longitude = longitude,
                                Latitude = latitude,
                                Address = address,
                                C_CityId = city.CityId,
                                Comment = name
                            };
                            context.InsertUnit(geo);
                            geoLocations.Add(geo);
                        }


                        if (region == null)
                        {
                            region = new District()
                            {
                                Name = regionName,
                                C_CenterGeoId = geo.GeoLocationId
                            };
                            context.InsertUnit(region);

                            regions.Add(region);
                        }

                        if (!companyExist)
                        {
                           var company = new LegalEntity()
                            {
                                LegalName = name.Length > 64? name.Substring(64) : name,
                                C_MainGeoId = geo.GeoLocationId,
                                Phones = phones,
                                Mails = email,
                                Sites = webSite,
                                Created = DateTime.Now,
                                CreatedBy = _userId,
                                PublicName = name.Length > 128 ? name.Substring(128) : name,
                            };
                            context.InsertUnit(company);
                            names.Add(company.PublicName);

                            var legalActivity = context.LegalActivities
                                .FirstOrDefault(m => m.C_ServiceId == categoryChild.CompanyServiceId
                                && m.C_LegalEntityId == company.LegalEntityId);
                            if (legalActivity == null)
                            {
                                context.InsertUnit(new LegalActivity()
                                {
                                    ServiceFullName = subCategory,
                                    C_ServiceId = categoryChild.CompanyServiceId,
                                    C_LegalEntityId = company.LegalEntityId
                                });
                            }
                        }

                        

                    }
                    catch (Exception ex)

                    { 
                    }
                }
            }
            byte[] output = null;
            using (var pck = new ExcelPackage())
            {
                pck.Workbook.Worksheets.Add(dt.TableName);
                ExcelWorksheet ws = pck.Workbook.Worksheets[dt.TableName];
                ws.Cells.LoadFromDataTable(dt, true);
                output = pck.GetAsByteArray();
            }
            return output;
        }

        private byte[] UpdateDemoData(DataSet result)
        {
            throw new NotImplementedException();
        }
    }
}
