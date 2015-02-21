using CRMContext;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using CompanyContext;
using EnumHelper.CRM;
using CompanyModel;
using CRMModel;
using System.Web.Mvc;
using FilterModel;
using AccessModel;
using System.Globalization;

namespace CRMRepositories
{
    public class CRMRepository :CRMBaseRepository, IDisposable
    {
        public CRMEntities BaseContext { get; set; }
        public CRMRepository()
            : base()
        {
            BaseContext = new CRMEntities(AccessSettings.LoadSettings().CrmEntites);
        }

        public void AddCompanyRecord(CompanyRegisterModel model, int companyId, Guid userId)
        {
            var existCompany = BaseContext.LegalEntities.FirstOrDefault(m => m.LegalName == model.CompanyName);
            if (existCompany != null)
            {
                Context.GetUnitById<Company>(companyId).CRMGuid = existCompany.LegalEntityGuid;
            }
            else
            {
                var id = int.Parse(model.City);

                var geo = new GeoLocation()
                {
                    Address = string.Format("{0}, {1} {2}", model.Street, model.Number.Value, model.AddNumber),
                    C_CityId = id
                };
                BaseContext.InsertUnit(geo);

                var company = new CRMContext.LegalEntity()
                {
                    CreatedBy = userId,
                    C_MainGeoId = geo.GeoLocationId,
                    LegalName = model.CompanyName,
                    PublicName = model.CompanyName,
                };
                BaseContext.InsertUnit(company);

                Context.GetUnitById<Company>(companyId).CRMGuid = company.LegalEntityGuid;
                Context.SaveChanges();

                BaseContext.InsertUnit(new Customer()
                {
                    CreatedBy = userId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Phone = model.Phone,
                    IsLPR = true,
                    Mail = model.Email,
                    Position = "Руководитель",
                    C_LegalEntityId = company.LegalEntityId
                });
            }
        }

        public IList<CompanyPreview> GetCompanyPreviews(IEnumerable<Guid> existCompanies)
        {
            return BaseContext.LegalEntities
                .Where(m => !existCompanies.Contains(m.LegalEntityGuid))
                .Select(m => new CompanyPreview()
                {
                    CompanyId = m.LegalEntityId,
                    CompanyGuid = m.LegalEntityGuid,
                    CompanyName = m.LegalName,
                    IsOurClient = false,
                    OwnerId = m.CreatedBy
                }).ToList();
        }
        public virtual IList<CustomerViewModel> GetCustomers(Guid? companyId)
        {
            if (companyId.HasValue)
                return BaseContext.LegalEntities
                   .FirstOrDefault(m => m.LegalEntityGuid == companyId)
                   .Customers.Select(n => new CustomerViewModel()
                   {
                       CompanyId = n.LegalEntity.LegalEntityGuid,
                       FirstName = n.FirstName,
                       PositionName = n.Position,
                       IsLPR = n.IsLPR,
                       LastName = n.LastName,
                       Patronymic = n.Patronymic,
                       Mail = n.Mail,
                       Phone = n.Phone,
                       Comment = n.Comment,
                       UserId = n.CustomerGuid
                   }).ToList();
            else
                return BaseContext.Customers
                           .Where(m => !m.C_LegalEntityId.HasValue).Select(n => new CustomerViewModel()
                           {
                               FirstName = n.FirstName,
                               PositionName = n.Position,
                               IsLPR = n.IsLPR,
                               LastName = n.LastName,
                               Patronymic = n.Patronymic,
                               Mail = n.Mail,
                               Phone = n.Phone,
                               Comment = n.Comment,
                               UserId = n.CustomerGuid
                           }).ToList();
        }

        protected Guid AddCompanyToBase(CRMCompanyEditModel company, Guid userId)
        {
            IFormatProvider provider = new CultureInfo("en-US");

            var curOGRN = company.Details[0];
            var existLegal = BaseContext.LegalEnityDetails.FirstOrDefault(m => m.OGRN == curOGRN.OGRN);
            if (existLegal == null )
            {
                GeoLocation location = new GeoLocation()
                {
                    Latitude = !string.IsNullOrEmpty(company.GeoAddr.Latitude) ? Convert.ToDouble(company.GeoAddr.Latitude, provider) : new Nullable<double>(),
                    Longitude = !string.IsNullOrEmpty(company.GeoAddr.Longitude) ? Convert.ToDouble(company.GeoAddr.Longitude, provider) : new Nullable<double>(),
                    Address = string.IsNullOrEmpty(company.Address) ? "Не задан" : company.Address,
                    C_CityId = company.City
                };
                BaseContext.InsertUnit(location);

                var legalEnitity = new CRMContext.LegalEntity()
                {
                    LegalEntityId = company.LegalEntityId,
                    LegalName = company.LegalName,
                    PublicName = company.PublicName,
                    Mails = company.Mails,
                    Sites = company.Sites,
                    Phones = company.Phones,
                    Created = DateTime.Now,
                    CreatedBy = userId,
                    ConfirmedBy = userId,
                    C_MainGeoId = location.GeoLocationId,
                    Skype = company.Skype,
                    LogoPath = company.PhotoPath,
                };
                BaseContext.InsertUnit(legalEnitity);
                if (curOGRN.OGRN != 0)
                {
                    var legalDetails = new LegalEnityDetail()
                    {
                        BIK = curOGRN.BIK,
                        INN = curOGRN.INN,
                        IsActive = curOGRN.IsActive,
                        KPP = curOGRN.KPP,
                        KS = curOGRN.KS,
                        OGRN = curOGRN.OGRN,
                        RS = curOGRN.RS,
                        C_GeoId = location.GeoLocationId,
                        C_LegalEntityId = legalEnitity.LegalEntityId,
                        Created = DateTime.Now,
                        CreatedBy = userId,
                    };
                    BaseContext.InsertUnit(legalDetails);
                    legalEnitity.C_CurrentLegalDetailId = legalDetails.LegalEntityDetailId;
                    BaseContext.SaveChanges();
                }
                return legalEnitity.LegalEntityGuid;
            }
            return existLegal.LegalEntity.LegalEntityGuid;
        }

        protected Guid AddCustomerToBase(CustomerEditModel customer, Guid userId)
        {
            var commonModel = new Customer()
            {
                FirstName = customer.FirstName,
                Position = customer.PositionName,
                IsLPR = customer.IsLPR,
                Skype = customer.Skype,
                SocialsLinks = customer.SocialLinks,                 
                LastName = customer.LastName,
                Patronymic = customer.Patronymic,
                Mail = customer.Mail,
                Phone = customer.Phone,
                CreatedBy = userId,
                IsAcctual = true,
            };
            if (customer.CompanyId.HasValue)
                commonModel.C_LegalEntityId = BaseContext.LegalEntities.FirstOrDefault(m => m.LegalEntityGuid == customer.CompanyId).LegalEntityId;

            BaseContext.InsertUnit(commonModel);
            return commonModel.CustomerGuid;
        }

        public List<SelectListItem> GetCitiesSelectItems(int? distId, Guid? cityGuid = null)
        {
            var result = BaseContext.Cities.AsQueryable();
            if (distId.HasValue)
                result = result.Where(m => m.C_DistrictId == distId.Value);
            else if (cityGuid.HasValue)
            {
                var city = BaseContext.Cities.FirstOrDefault(m => m.CityGuid == cityGuid);
                result = result.Where(m => m.C_DistrictId == city.C_DistrictId);
            } return result.Select(m => new { id = m.CityId, name = m.Name, selected =  m.CityGuid == cityGuid })
                  .ToList()
                  .OrderBy(m => m.name)
                  .Select(m => new SelectListItem() { Text = m.name, Value = m.id.ToString() })
                  .ToList();
        }

        public IEnumerable<CityPreview> GetCities()
        {
            var result = BaseContext.Cities
                 .ToList()
                 .Select(m => new CityPreview()
                 {
                     CityId = m.CityId,
                     Name = m.Name,
                     Code = m.Code,
                     NumCompanies = m.GeoLocations.SelectMany(n => n.LegalEntities).Distinct().Count()
                 })
                 .ToList();

            foreach (var item in result)
            {
                item.NumCustomers = Context.Addresses
                                            .Where(m => m.City == item.Name)
                                            .Select(m => m.C_CompanyId)
                                            .Distinct()
                                            .Count();
            }
            return result;
        }

        public CityPreview GetCity(int? cityId)
        {
            CityPreview model = new CityPreview();
            if (cityId.HasValue)
            {
                var existCity  = BaseContext.GetUnitById<City>(cityId.Value);
                model.CityId = existCity.CityId;
                model.Name = existCity.Name;
                model.Code = existCity.Code;
            }
            return model;
        }

        public void UpdateCity(CityPreview city)
        {
            if (city.CityId == 0)
            {
                Context.InsertUnit(new City()
                {
                    Code = city.Code,
                    Name = city.Name
                });
            }
            else
            {
                var existCity = Context.GetUnitById<City>(city.CityId);
                existCity.Code = city.Code;
                existCity.Name = city.Name;
                Context.SaveChanges();
            }
        }

        public virtual List<SelectListItem> GetServices(IEnumerable<int> parentServices, bool isAll = false)
        {
            if (isAll)
            {
                return BaseContext.CompanyServices
                .ToList()
                .Select(m => new SelectListItem()
                {
                    Text = (m.C_ParentId.HasValue ? m.CompanyServiceParent.Name.ToUpper() + "||" : string.Empty) + m.Name,
                    Value = m.CompanyServiceId.ToString(),
                    Selected = false
                }).ToList();
            }
            else
            {
                return BaseContext.CompanyServices
                    .Where(m => m.C_ParentId.HasValue)
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = m.CompanyServiceParent.Name.ToUpper() + "||" + m.Name,
                        Value = m.CompanyServiceId.ToString(),
                        Selected = m.C_ParentId.HasValue && parentServices.Contains(m.C_ParentId.Value)
                    }).ToList();
            }
        }

        public ServiceEditModel GetService(int? serviceId)
        {
            ServiceEditModel model = new ServiceEditModel();
            if (serviceId.HasValue)
            {
                var existService = BaseContext.GetUnitById<CompanyService>(serviceId.Value);
                model.ParentId = existService.C_ParentId;
                model.ServiceId = existService.CompanyServiceId;
                model.Name = existService.Name;
                if (model.ParentId.HasValue)
                {
                    model.AvailableParents =
                        BaseContext.CompanyServices
                        .Where(m => !m.C_ParentId.HasValue)
                        .ToList()
                        .Select(m => new SelectListItem()
                        {
                            Text = m.Name,
                            Value = m.CompanyServiceId.ToString(),
                            Selected = m.CompanyServiceId == model.ParentId
                        }).ToList();
                }
            }
            else
            {
                model.AvailableParents = BaseContext.CompanyServices
                    .Where(m => !m.C_ParentId.HasValue)
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = m.Name,
                        Value = m.CompanyServiceId.ToString()
                    }).ToList();
            }
            if (model.AvailableParents != null)
                model.AvailableParents.Insert(0, new SelectListItem() { Text = "Выберите главный раздел", Value = "-1" });
            else
                model.AvailableParents = new List<SelectListItem>();
            return model;
        }

        public void UpdateService(ServiceEditModel service)
        {
            if (service.ServiceId == 0)
            {
                BaseContext.InsertUnit(new CompanyService()
                {
                    Name = service.Name,
                    C_ParentId = !service.ParentId.HasValue || service.ParentId == -1 ? null : service.ParentId
                });
            }
            else
            {
                var existService = BaseContext.GetUnitById<CompanyService>(service.ServiceId);
                existService.C_ParentId = !service.ParentId.HasValue || service.ParentId == -1 ? null : service.ParentId;
                Context.SaveChanges();
            }
        }



        public Guid[] GetCustomerAndCompany(Guid _userId)
        {
            var user = BaseContext.Customers.FirstOrDefault(m => m.PossibleUserId == _userId);
            if (user != null)
            {
                return new Guid[] { user.CustomerGuid, user.LegalEntity.LegalEntityGuid};
            }
            else return null;
        }

        public string GetCompanyBase(Guid guid)
        {
            var customer = BaseContext.Customers.FirstOrDefault(m => m.CustomerGuid == guid);
            if (customer == null)
                return string.Empty;
            var companyId = customer.LegalEntity.LegalEntityGuid;
            var company = Context.Companies.FirstOrDefault(m => m.CRMGuid == companyId);
            return company !=null? company.LocalDB : string.Empty;
        }

        public List<SelectListItem> GetDistricts()
        {
            List<SelectListItem> result = new List<SelectListItem>(){
                new SelectListItem(){ Text="Выберите регион вашей компании", Value=""}
            };
            result.AddRange(BaseContext.Districts.ToList()
                .Select(m => new SelectListItem() { Text = m.Name, Value = m.DistrictId.ToString() }));
            return result;
        }

        public City GetCompanyCity(Guid? companyGuid)
        {
            var city = Context.Companies.FirstOrDefault(m => m.CRMGuid == companyGuid).Address.City;
            return BaseContext.Cities.FirstOrDefault(m => m.Name == city);
        }

        public int? GetDistrictByCity(Guid cityGuid)
        {
            return BaseContext.Cities.FirstOrDefault(m => m.CityGuid == cityGuid).C_DistrictId;
        }
    }
}
