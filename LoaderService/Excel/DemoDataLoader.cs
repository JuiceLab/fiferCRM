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
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data.Entity.Infrastructure;

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
        private object _lock = new object();
        public DemoDataLoader(Guid userId)
        {
            _userId = userId;
        }

        public byte[] RefreshCRMCompanies(FileStream stream, string regionName, bool isFirstLoad = false)
        {
            byte[] result = null;

            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                excelReader.IsFirstRowAsColumnNames = false;
                DataSet resultSet = excelReader.AsDataSet();
                if (isFirstLoad)
                    result = CreateDemoData(regionName, resultSet);
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

        private byte[] CreateDemoData(string districtName, DataSet result)
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
            using (CRMEntities globalcontext = new CRMEntities(AccessSettings.LoadSettings().CrmEntites) { })
            {
                var objectContext = (globalcontext as IObjectContextAdapter).ObjectContext;

                // Sets the command timeout for all the commands
                objectContext.CommandTimeout = 0;
                var regionName = districtName;
                var region = globalcontext.Districts.ToList().FirstOrDefault(m => m.Name == regionName || regionName.StartsWith(m.Name));
                BlockingCollection<City> cities = new BlockingCollection<City>();
                foreach (var item in region.Cities.ToList())
                {
                    cities.Add(item);
                }
                var regions = globalcontext.Districts.ToList();
                var categories = new BlockingCollection<CompanyService>();
                foreach (var item in globalcontext.CompanyServices.ToList())
                {
                    categories.Add(item);
                }
                var names = new BlockingCollection<string>();
                foreach (var item in globalcontext.LegalEntityShortViews.Select(m => m.PublicName))
                {
                    names.Add(item);
                };

                var geoLocations = new BlockingCollection<GeoLocationCityView>();
                foreach (var item in globalcontext.GeoLocationCityViews.Where(m => m.C_DistrictId == region.DistrictId).Distinct().ToList())
                {
                    geoLocations.Add(item);
                }

                int counter = dt.Rows.Count;
                int maxCount = dt.Rows.Count;
                Parallel.ForEach(dt.Rows.Cast<DataRow>().AsParallel(), new ParallelOptions() { MaxDegreeOfParallelism = 1 }, item =>
                {
                    using (CRMEntities context = new CRMEntities(AccessSettings.LoadSettings().CrmEntites))
                    {
                        var rowVal = item.ItemArray;
                        string name = string.Empty;
                        try
                        {
                            name = rowVal.ElementAt(16).ToString();
                            if (name.Length > 128)
                                name = name.Substring(128);
                            var category = rowVal.ElementAt(17).ToString();
                            if (category.Length > 128)
                                category = category.Substring(128);
                            var subCategory = rowVal.ElementAt(18).ToString();
                            if (subCategory.Length > 128)
                                subCategory = subCategory.Substring(128);
                            var cityName = rowVal.ElementAt(20).ToString();

                            var address = rowVal.ElementAt(21).ToString();
                            var phones = string.Join(",", rowVal.ElementAt(22).ToString(), rowVal.ElementAt(23).ToString(), rowVal.ElementAt(24).ToString(), rowVal.ElementAt(25).ToString());

                            var email = rowVal.ElementAt(26).ToString();
                            var webSite = rowVal.ElementAt(27).ToString();

                            var categoryParent = categories.FirstOrDefault(m => m.Name == category);
                            if (categoryParent == null)
                            {
                                lock (_lock)
                                {
                                    if (!categories.Any(m => m.Name == category))
                                    {
                                        categoryParent = new CompanyService()
                                        {
                                            Name = category
                                        };
                                        context.InsertUnit(categoryParent);
                                        categories.Add(categoryParent);
                                    }
                                    else
                                    {
                                        categoryParent = categories.FirstOrDefault(m => m.Name == category);
                                    }
                                }
                            }

                            var categoryChild = categories.FirstOrDefault(m => m.Name == subCategory);
                            if (categoryChild == null)
                            {
                                lock (_lock)
                                {
                                    if (!categories.Any(m => m.Name == subCategory))
                                    {
                                        categoryChild = new CompanyService()
                                        {
                                            C_ParentId = categoryParent.CompanyServiceId,
                                            Name = subCategory

                                        };
                                        context.InsertUnit(categoryChild);
                                        categories.Add(categoryChild);
                                    }
                                    else
                                    {
                                        categoryChild = categories.FirstOrDefault(m => m.Name == subCategory);
                                    }
                                }
                            }

                            var city = cities.FirstOrDefault(m => m.Name == cityName);

                            if (city == null)
                            {
                                lock (_lock)
                                {
                                    if (!cities.Any(m => m.Name == cityName))
                                    {
                                        var existCities = context.Cities.Where(m => m.Name == cityName && !m.C_DistrictId.HasValue).ToList();

                                        if (existCities.Count() > 0)
                                        {
                                            var noDistrictCity = existCities.Where(m => m.GeoLocations.Count > 0).ToList();
                                            if (noDistrictCity.Count > 0)
                                            {
                                                city = noDistrictCity.FirstOrDefault();
                                                city.C_DistrictId = region.DistrictId;
                                            }
                                            if (noDistrictCity.Count > 1)
                                            {
                                                foreach (var noDistrict in noDistrictCity.Skip(1))
                                                {
                                                    var locations = context.GeoLocations.Where(m => m.C_CityId == noDistrict.CityId).ToList();
                                                    foreach (var curLocation in locations)
                                                    {
                                                        curLocation.C_CityId = city.CityId;
                                                    }
                                                }
                                            }
                                            foreach (var existCity in existCities.Where(m => m.GeoLocations.Count == 0))
                                            {
                                                context.Cities.Remove(existCity);
                                            }
                                            context.SaveChanges();
                                            if (city != null)
                                                cities.Add(city);
                                        }
                                        else
                                        {
                                            city = new City()
                                            {
                                                Name = cityName,
                                                Code = string.Empty,
                                                C_DistrictId = region.DistrictId
                                            };
                                            context.InsertUnit(city);
                                            cities.Add(city);
                                        }
                                    }
                                    else
                                    {
                                        city = cities.FirstOrDefault(m => m.Name == cityName);

                                    }
                                }
                            }

                            var latitude = Convert.ToDouble(rowVal.ElementAt(28).ToString(), cultureInfo);
                            var longitude = Convert.ToDouble(rowVal.ElementAt(29).ToString(), cultureInfo);

                            var geoView = geoLocations
                                .FirstOrDefault(m =>
                                    m.Latitude.HasValue && m.Latitude == latitude
                                    && m.Longitude.HasValue && m.Longitude == longitude);
                            if (geoView == null)
                            {
                                lock (_lock)
                                {
                                    if (!geoLocations.Any(m =>
                                        m.Latitude.HasValue && m.Latitude == latitude
                                        && m.Longitude.HasValue && m.Longitude == longitude))
                                    {
                                        var geo = new GeoLocation()
                                         {
                                             Longitude = longitude,
                                             Latitude = latitude,
                                             Address = address,
                                             C_CityId = city.CityId,
                                             Comment = name
                                         };
                                        context.InsertUnit(geo);
                                        geoView = new GeoLocationCityView() { C_DistrictId = region.DistrictId, CityGuid = city.CityGuid, CityId = city.CityId, Latitude = geo.Latitude, GeoLocationId = geo.GeoLocationId, Longitude = geo.Longitude };

                                        geoLocations.Add(geoView);
                                    }
                                    else
                                    {
                                        geoView = geoLocations.FirstOrDefault(m =>
                                        m.Latitude.HasValue && m.Latitude == latitude
                                        && m.Longitude.HasValue && m.Longitude == longitude);
                                    }
                                }
                            }

                            if (!names.Any(m => m == name))
                            {
                                lock (_lock)
                                {
                                    var company = new LegalEntity();
                                    if (!names.Any(m => m == name))
                                    {
                                        company = new LegalEntity()
                                        {
                                            LegalName = name.Length > 64 ? name.Substring(64) : name,
                                            C_MainGeoId = geoView.GeoLocationId,
                                            Phones = phones,
                                            Mails = email,
                                            Sites = webSite,
                                            Created = DateTime.Now,
                                            CreatedBy = _userId,
                                            PublicName = name.Length > 128 ? name.Substring(128) : name,
                                        };
                                        context.InsertUnit(company);
                                        names.Add(company.PublicName);
                                    }
                                    else
                                        company = context.LegalEntities.FirstOrDefault(m => m.PublicName == name);
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
                            else
                            {
                                lock (_lock)
                                {
                                    var legalItem = context.LegalEntities.FirstOrDefault(m => m.PublicName == name || m.LegalName == name);
                                    var clearPhone = string.Join(",", legalItem.Phones.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

                                    if (legalItem.Phones != phones
                                        && legalItem.Phones != clearPhone
                                        && legalItem.LegalEntityAddPhones.Where(m => m.C_GeoLocationId == geoView.GeoLocationId).Count() == 0)
                                    {
                                        context.InsertUnit(new LegalEntityAddPhone()
                                        {
                                            C_LegalEntityId = legalItem.LegalEntityId,
                                            C_GeoLocationId = geoView.GeoLocationId,
                                            Phones = string.Join(",", phones.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                        });
                                    }
                                    legalItem.Phones = clearPhone;

                                    if (geoView.GeoLocationId != legalItem.C_MainGeoId && !legalItem.GeoLocations.Any(m => m.GeoLocationId == geoView.GeoLocationId))
                                    {
                                        legalItem.GeoLocations.Add(context.GetUnitById<GeoLocation>(geoView.GeoLocationId));
                                        context.SaveChanges();

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                        counter--;
                        OnChangeProgress((int)((((double)maxCount - (double)counter) / (double)maxCount) * 100.0), string.Format("Осталось{1}. Обработана {0}", name, counter));
                    }
                });
            }
            byte[] output = null;
            return output;
        }

        private byte[] UpdateDemoData(DataSet result)
        {
            throw new NotImplementedException();
        }

        private void OnChangeProgress(int counter, string msg)
        {
            ChangeProcess(this, new ProgressChangedEventArgs(counter, msg));
        }

        public event ProgressChangedEventHandler ChangeProcess;

        public void AddCitiesTemp(FileStream stream)
        {
            using (IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream))
            {
                excelReader.IsFirstRowAsColumnNames = false;
                DataSet resultSet = excelReader.AsDataSet();

                var data = resultSet.Tables[0];
                var dt = new DataTable("Протокол загрузки");
                dt.Columns.Add("№");
                dt.Columns.Add("НАЗВАНИЕ");
                dt.Columns.Add("РУБРИКА");
                dt.Load(data.CreateDataReader());
                CultureInfo cultureInfo = new CultureInfo("en-US");
                using (CRMEntities globalcontext = new CRMEntities(AccessSettings.LoadSettings().CrmEntites) { })
                {
                    var objectContext = (globalcontext as IObjectContextAdapter).ObjectContext;

                    Parallel.ForEach(dt.Rows.Cast<DataRow>().AsParallel(), new ParallelOptions() { MaxDegreeOfParallelism = 1 }, item =>
                {
                    using (CRMEntities context = new CRMEntities(AccessSettings.LoadSettings().CrmEntites))
                    {
                        var rowVal = item.ItemArray;
                        string name = string.Empty;
                        try
                        {
                            name = rowVal.ElementAt(3).ToString();
                            if (name.Length > 128)
                                name = name.Substring(128);
                            var prefix = rowVal.ElementAt(4).ToString();
                            var code = rowVal.ElementAt(5).ToString();

                            context.Cities.Add(new City()
                            {
                                C_DistrictId = 90,
                                Code = code,
                                CityPrefix = prefix,
                                Name = name
                            });
                            context.SaveChanges();
                        }
                        catch
                        { }
                    }
                });
                }
            }
            stream.Close();
        }
    }
}
