using AccessModel;
using CompanyContext;
using CompanyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using System.Web.Mvc;
using EntityRepository;
using System.Globalization;
using Interfaces.Finance;
using Interfaces.CRM;
using CRMRepositories;
using EnumHelper.WebSite;

namespace CompanyRepositories
{
    public class CompanyRepository : BaseCompanyRepository, IDisposable
    {
        private const string _defaultLocalDBprefix = "fifer_localcrm_";
        private const string _headerDepartment = "Руководители";
        private const string _headerPosition = "Управляющий";
        private const string _headerPositionGenetive = "Управляющего";
        private object _lockObj = new object();
        public string CreateCompany(CompanyRegisterModel model, Guid userId,string cityName, out int companyId)
        {
            var company = new Company()
            {
                CompanyName = model.CompanyName,
                CreatedBy = userId,
                IsActive = false,
                IsPublic = model.IsPublic,
                LocalDB = string.Empty,
            };

            Context.Companies.Add(company);
            Context.SaveChanges();
            companyId = company.CompanyId;

            company.LocalDB = _defaultLocalDBprefix + company.CompanyId;
            Context.SaveChanges();

            var addr = new Address()
            {
                City = cityName,
                Number = model.Number.Value,
                IsActive = true,
                AddNumber = model.AddNumber,
                Street = model.Street,
                ZipCode = model.ZipCode,
                C_CompanyId = company.CompanyId
            };

            if(model.IsLegalAddress)
            {
                var addrLegal = new Address()
                    {
                        City = cityName,
                        Number = model.Number.Value,
                        IsActive = true,
                        AddNumber = model.AddNumber,
                        Street = model.Street,
                        ZipCode = model.ZipCode,
                        C_CompanyId = company.CompanyId,
                    };
                Context.InsertUnit(addrLegal);
                var legalEnity = new LegalEntity()
                {
                    C_LegalAddrId = addrLegal.AddrId,
                    INN = 0,
                    KPP = 0,
                    OGRN = 0,
                    RS = 0,
                    CreatedBy = userId,
                    // todo set false
                    IsActive = true
                };
                Context.InsertUnit(legalEnity);
                company.C_CurrentLegalEntityId  =  legalEnity.LegalEntityId;
                Context.SaveChanges();
            }
            Context.Addresses.Add(addr);
            Context.SaveChanges();
            company.C_CurrentAddrId = addr.AddrId;
            Context.SaveChanges();
            
            var department = new Department()
            {
                C_CompanyId = company.CompanyId,
                CreatedBy = userId,
                IsActive = true,
                Title = _headerDepartment
            };
            Context.Departments.Add(department);
            Context.SaveChanges();

            var position = new Position()
            {
                C_DepartmentId = department.DepartmentId,
                IsActive = true,
                Name = _headerPosition,
                NameGenetive = _headerPositionGenetive
            };
            Context.Positions.Add(position);
            Context.SaveChanges();

            var employee = new Employee()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                LastLogin = DateTime.Now,
                C_PosId = position.PositionId,
                PhotoPath = string.Empty,
                UserId = userId,
                IsDismissed = false,
                IsKeyEmployee = true
            };
            Context.Employees.Add(employee);
            Context.SaveChanges();
            employee.Departments.Add(department);
            Context.SaveChanges();
            return company.LocalDB;
        }


        public string GetCompanyName(Guid userId)
        {
            return ViewContext.UserInDepartmentView
               .FirstOrDefault(m => m.UserId == userId)
               .CompanyName;
        }

        public CompanyViewModel GetShortCompanyInfoByUserId(Guid userId)
        {
            var user = ViewContext.UserInDepartmentView
                .FirstOrDefault(m => m.UserId == userId);
            var companyId = user.C_CompanyId;
            var company = Context.Companies.FirstOrDefault(m => m.CompanyId == companyId);
            var model = new CompanyViewModel()
            {
                UserPhoto = user.PhotoPath,
                Guid = company.CRMGuid,
                CompanyId = companyId,
                CompanyName = company.CompanyName,
                PublicCompanyName = company.ShortComanyName,
                Logo = company.PhototPath,
            };
            return model;
        }

        public CompanyViewModel GetCompanyByUserId(Guid userId)
        {
            var companyId = ViewContext.UserInDepartmentView
                .FirstOrDefault(m => m.UserId == userId)
                .C_CompanyId;

            var keysEmployees = ViewContext.UserInDepartmentView
                .Where(m => m.C_CompanyId == companyId && m.IsKeyEmployee);

            var company = Context.Companies.FirstOrDefault(m => m.CompanyId == companyId);
            var model = new CompanyViewModel()
            {
                CompanyId = companyId,
                IsPublic = company.IsPublic,
                CurAddrId = company.C_CurrentAddrId.Value,
                CurLegalAddrId = company.C_CurrentLegalEntityId,
                CompanyName = company.CompanyName,
                PublicCompanyName = company.ShortComanyName,
                SiteUrl = company.SiteUrl,
                Fax = company.Fax,
                ICQ = company.ICQ,
                Mail = company.Email,
                Skype = company.Skype,
                Guid = company.CRMGuid,
                Services = !string.IsNullOrEmpty(company.Services) ?
                        company.Services.Split(',').Select(m => int.Parse(m)).ToList()
                        : new List<int>(),
                Logo = company.PhototPath,
                KeyEmployees = keysEmployees.Select(m => new CompanyEmployeeViewModel()
                {
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    BirthDate = m.BirthDate,
                    FullNameGenetive = m.FullNameGenitive,
                    LastLogin = m.LastLogin,
                    IsDissmissed = m.IsDismissed,
                    Patronymic = m.Patronymic,
                    PhotoPath = m.PhotoPath,
                    Position = m.C_PosId,
                    UserId = m.UserId,
                    EmployeeId = m.EmployeeId,
                    PositionName = m.Name,
                    PositionNameGenetive = m.NameGenetive
                }).ToList(),
                Addresses = company.Addresses.ToList().Select(m => new CompanyAddressViewModel()
                {
                    CityGuid = m.CityGuid,
                    City = m.City,
                    Street = m.Street,
                    IsLegal = m.LegalEntities.Count > 0,
                    Mails = m.Emails,
                    Phones = m.Phones,
                    Number = m.Number,
                    ZipCode = m.ZipCode,
                    AddNumber = m.AddNumber,
                    Latitude = m.Latitiude.ToString(),
                    Longitude = m.Longitude.ToString(),
                    AddrId = m.AddrId,
                    LegalEntities = m.LegalEntities.Count > 0 ?
                    m.LegalEntities.Select(n => new LegalEntityViewModel()
                    {
                        BIK = n.BIK,
                        INN = n.INN,
                        IsActive = n.IsActive,
                        KPP = n.KPP,
                        KS = n.KS,
                        OGRN = n.OGRN,
                        RS = n.RS,
                        PaymentLocation = n.PayLocation,
                        LegalEntityId = n.LegalEntityId
                    }).ToList()
                    : null
                }).ToList()
            };

            CRMRepository repository = new CRMRepositories.CRMRepository();

            model.AvailableServices = repository.GetServices(model.Services);
            model.LegalEntity = model.Addresses
                .Where(m => m.LegalEntities != null)
                .SelectMany(m => m.LegalEntities)
                .FirstOrDefault(m => m.LegalEntityId == company.C_CurrentLegalEntityId);

            return model;
        }


        public void UpdateCompany(CompanyViewModel model, Guid userId)
        {
            var company = Context.Companies.FirstOrDefault(m => m.CompanyId == model.CompanyId);
            company.CompanyName = model.CompanyName;
            company.ShortComanyName = model.PublicCompanyName;
            company.PhototPath = model.Logo;
            company.Skype = model.Skype;
            company.Email = model.Mail;
            company.ICQ = model.ICQ;
            company.Fax = model.Fax;
            company.SiteUrl = model.SiteUrl;
            if (model.Services != null)
                company.Services =  string.Join(",", model.Services);
            company.IsPublic = model.IsPublic;
            company.Modified = DateTime.Now;
            company.ModifiedBy = userId;
            Context.SaveChanges();

            var legalEntity = Context.LegalEntities.FirstOrDefault(m => m.LegalEntityId == model.LegalEntity.LegalEntityId);
            if (legalEntity == null)
            {
                legalEntity = new LegalEntity()
                {
                    INN = model.LegalEntity.INN,
                    KPP = model.LegalEntity.KPP,
                    KS = model.LegalEntity.KS,
                    OGRN = model.LegalEntity.OGRN,
                    BIK = model.LegalEntity.BIK,
                    RS = model.LegalEntity.RS,
                    CreatedBy = userId,
                    IsActive = true,
                    PayLocation = model.LegalEntity.PaymentLocation,
                    C_LegalAddrId = model.CurLegalAddrId,
                };
                Context.LegalEntities.Add(legalEntity);
                Context.SaveChanges();
                company.C_CurrentLegalEntityId = legalEntity.LegalEntityId;
            }
            else
            {
                legalEntity.INN = model.LegalEntity.INN;
                legalEntity.KPP = model.LegalEntity.KPP;
                legalEntity.KS = model.LegalEntity.KS;
                legalEntity.OGRN = model.LegalEntity.OGRN;
                legalEntity.BIK = model.LegalEntity.BIK;
                legalEntity.RS = model.LegalEntity.RS;
                legalEntity.IsActive = true;
                legalEntity.PayLocation = model.LegalEntity.PaymentLocation;
            }
            Context.SaveChanges();
        }

        public void UpdateAddress(CompanyAddressViewModel model, Guid userId)
        {
           IFormatProvider provider = new CultureInfo("en-US");
            var addr = Context.Addresses.FirstOrDefault(m => m.AddrId == model.AddrId);
            if (addr != null)
            {
                addr.CityGuid = model.CityGuid;
                addr.City = model.City;
                addr.AddNumber = model.AddNumber;
                addr.Street = model.Street;
                addr.Phones = model.Phones;
                addr.Emails = model.Mails;
                addr.Number = model.Number;
                addr.Latitiude = Convert.ToDouble(model.Latitude, provider);
                addr.Longitude = Convert.ToDouble(model.Longitude, provider);
                addr.ZipCode = model.ZipCode;
                Context.SaveChanges();
            }
            else
            {
                addr = new Address()
                {
                    CityGuid = model.CityGuid,
                    City = model.City,
                    AddNumber = model.AddNumber,
                    Street = model.Street,
                    Phones = model.Phones,
                    Emails = model.Mails,
                    Number = model.Number,
                    ZipCode = model.ZipCode,
                    C_CompanyId = model.CompanyId,
                    Latitiude = !string.IsNullOrEmpty(model.Latitude)? Convert.ToDouble(model.Latitude, provider) : new Nullable<double>(),
                    Longitude = !string.IsNullOrEmpty(model.Longitude) ? Convert.ToDouble(model.Longitude, provider) : new Nullable<double>(),
                    IsActive = true
                };
                Context.Addresses.Add(addr);
                Context.SaveChanges();
                if (model.IsLegal)
                {
                    var company = Context.Companies.FirstOrDefault(m => m.CompanyId == model.CompanyId);
                    if (!company.C_CurrentLegalEntityId.HasValue)
                    {
                        var legalEntity = new LegalEntity()
                        {
                            INN = 0,
                            KPP = 0,
                            KS = 0,
                            OGRN = 0,
                            BIK = 0,
                            RS = 0,
                            CreatedBy = userId,
                            IsActive = true,
                        };
                        Context.LegalEntities.Add(legalEntity);
                        Context.SaveChanges();
                        company.C_CurrentLegalEntityId = legalEntity.LegalEntityId;
                        Context.SaveChanges();
                    }
                    addr.LegalEntities.Add(company.LegalEntity);
                    Context.SaveChanges();
                }
            }
        }
        
        public IList<CompanyPreview> GetCompanyPreviews()
        {
            return Context.Companies
                .Select(m => new CompanyPreview()
                {
                    CompanyId = m.CompanyId,
                    IsOurClient  = true,
                    CompanyGuid = m.CRMGuid,
                    CompanyLogo = m.PhototPath,
                    Created = m.Created,
                    OwnerId = m.CreatedBy,
                    CompanyName = m.CompanyName,
                }).ToList();
        }
        
        public void UpdateCompanyInfoByUserId(Guid _userId, ICompanyInfo financeStat)
        {
            var companyInfo = GetCompanyByUserId(_userId);
            financeStat.CompanyId = companyInfo.CompanyId;
            financeStat.Logo = companyInfo.Logo;
            financeStat.Name = companyInfo.PublicCompanyName;
        }

        public IEnumerable<SiteProjectModel> GetSiteProjects(int companyId)
        {
            return Context.WebSites
                .Where(m => m.C_CompanyId == companyId)
                .Select(m => new SiteProjectModel()
                {
                    SiteId = m.SiteId,
                    Name = m.SiteName,
                    SiteUrl = m.Url,
                    Type = m.Type,
                    SiteGuid = m.SiteGuid
                }).ToList();
        }

        public bool IsExistSiteUrl(string url)
        {
            return Context.WebSites.Any(m => m.Url == url);
        }

        public Guid UpdateWebSite(SiteProjectModel model, Guid userId)
        {
            var prefix = ((SiteType)model.Type).ToString().ToLower();
            Guid result = model.SiteGuid;
            if (model.SiteId == 0 && !IsExistSiteUrl(model.SiteUrl))
            {
                lock (_lockObj)
                {
                    var count = Context.WebSites.Where(m => m.DbSource.StartsWith(prefix)).Count() + 1;
                    var site = new WebSite()
                    {
                        TemplateName = "Default",
                        SiteGuid = Guid.NewGuid(),
                        CreatedBy = userId,
                        C_CompanyId = ViewContext.UserInCompanyView.FirstOrDefault(m => m.UserId == userId).CompanyId,
                        SiteName = model.Name,
                        Type = model.Type,
                        Url = model.SiteUrl,
                        DbSource = prefix + "_" + count,
                    };
                    Context.InsertUnit(site);
                    result = site.SiteGuid;
                }
            }
            return result;
        }


        public WebSite GetSiteProject(Guid siteId)
        {
            return Context.WebSites.FirstOrDefault(m => m.SiteGuid == siteId);
        }

        public WebSite GetSiteProject(string site)
        {
            return Context.WebSites.FirstOrDefault(m => m.Url == site);
        }

        public string GetCompanyCityByUserId(Guid userId)
        {
            var companyId = ViewContext.UserInDepartmentView
                           .FirstOrDefault(m => m.UserId == userId)
                           .C_CompanyId;

            var company = Context.Companies.FirstOrDefault(m => m.CompanyId == companyId);
            return company.Address.City;
        }

        public WebSite GetSiteProjectByUserId(Guid guid)
        {
            return Context.WebSites.FirstOrDefault(m => m.CreatedBy == guid);
        }
    }
}
