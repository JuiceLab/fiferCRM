using CRMModel;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using TaskModel.CRM;
using System.Web.Mvc;
using CRMLocalContext;
using System.Globalization;
using CompanyModel;
using Interfaces.Finance;
using EnumHelper.CRM;
using TaskModel;
using EnumHelper;
using FilterModel;

namespace CRMRepositories
{
    public class CRMLocalRepository : CRMRepository, IDisposable
    {
        public LocalCRMEntities LocalContext { get; set; }

        public CRMLocalRepository(Guid userId)
            : base()
        {
            using (CompanyContext.CompanyViewEntities viewContext = new CompanyContext.CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites))
            {
                var localDb = viewContext.UserInCompanyView.FirstOrDefault(m => m.UserId == userId).LocalDB;
                LocalContext = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites.Replace(_defaultLocalDB, localDb));
            }
        }

        public void CreateCompanyDB()
        {
            LocalContext.Database.Create();
            var existServices = new List<CompanyService>();
            LocalContext.InsertUnits(BaseContext.CompanyServices.Select(m => new CompanyService()
            {
                Name = m.Name,
                BaseServiceGuid = m.ServiceGuid,
            }));
            foreach (var item in BaseContext.CompanyServices.Where(m=>m.C_ParentId.HasValue))
            {
                LocalContext.CompanyServices.FirstOrDefault(m => m.BaseServiceGuid == item.ServiceGuid).C_ParentId =
                    LocalContext.CompanyServices.FirstOrDefault(m => m.BaseServiceGuid == item.CompanyServiceParent.ServiceGuid).CompanyServiceId;
            }
            LocalContext.SaveChanges();
        }

        public CRMCompanyEditModel GetCompanyEditModel(int? legalEnitityId)
        {
            CRMCompanyEditModel model = new CRMCompanyEditModel()
            {
                Details = new List<LegalEntityViewModel>() { new LegalEntityViewModel() }
            };
            if (legalEnitityId.HasValue)
            {
                var company = LocalContext.GetUnitById<LegalEntity>(legalEnitityId.Value);
                model.LegalEntityId = company.LegalEntityId;
                model.LegalName = company.LegalName;
                model.PublicName = company.PublicName;
                model.Mails = company.Mails;
                model.Sites = company.Sites;
                model.PhotoPath = company.LogoPath;
                model.StatusId = company.StatusId;
                model.Phones = company.Phones;
                model.Address = company.GeoLocation.Address;
                model.Activities = company.LegalEnityDetails.SelectMany(m=>m.LegalActivities).Select(m => m.C_ServiceId).ToList();
                model.AssignedBy = company.Assigned;
                model.Comment = company.Comment;
                model.CityGuid = company.GeoLocation.CityGuid;
                var city = BaseContext.Cities.FirstOrDefault(m => m.CityGuid == model.CityGuid);
               if(city!=null)
               {
                   model.City= city.CityId;
                   model.DistrictId = city.C_DistrictId;
               }
                model.GeoAddr = new GeoEditModel()
                {
                    Latitude = company.GeoLocation.Latitude.HasValue ? company.GeoLocation.Latitude.Value.ToString().Replace(",", ".") : string.Empty,
                    Longitude = company.GeoLocation.Longitude.HasValue ? company.GeoLocation.Longitude.Value.ToString().Replace(",", ".") : string.Empty
                };
                model.Details = company.LegalEnityDetails.Select(m => new LegalEntityViewModel()
                {
                    BIK = m.BIK,
                    INN = m.INN,
                    IsActive = m.IsActive,
                    KPP = m.KPP,
                    KS = m.KS,
                    OGRN = m.OGRN,
                    RS = m.RS,
                    PaymentLocation = m.PayLocation,
                    LegalEntityId = m.LegalEntityDetailId
                }).ToList();
            }
            if (model.Details.Count == 0)
                model.Details.Add(new LegalEntityViewModel());
            return model;
        }

        public MeetingEditModel GetMeetingEditModel(Guid? taskId, Guid userId,int? companyId = null)
        {
            MeetingEditModel model = new MeetingEditModel();
            {
                model.Date = DateTime.Now.Date.AddDays(1).ToShortDateString();
                model.OwnerId = userId;
                model.StatusId = (byte)EnumHelper.TaskStatus.Novelty;
            };
            if (companyId.HasValue)
            {
                var meeting = LocalContext.Meetings.Where(m =>m.Customer.C_LegalEntityId == companyId &&  m.StatusId != (byte)EnumHelper.TaskStatus.Completed)
                .OrderBy(m=>m.Date)
                .FirstOrDefault();
                if (meeting != null)
                {
                    model.MeetingId = meeting.MeetingId;
                    model.CustomerId = meeting.Customer.CustomerGuid;
                    model.Goals = meeting.Goals;
                    model.StatusId = meeting.StatusId;
                    model.OwnerId = meeting.CreatedBy;
                    model.Date = DateTime.Now.Date == meeting.Date.Date ? meeting.Date.ToShortTimeString() : meeting.Date.ToShortDateString();
                    model.Result = meeting.ResultComment;
                    model.Comment = meeting.Comment;
                }
            }

            else  if (taskId.HasValue)
            {
                var meeting = LocalContext.Meetings.FirstOrDefault(m=>m.MeetingGuid == taskId.Value);
                model.MeetingId = meeting.MeetingId;
                model.CustomerId = meeting.Customer.CustomerGuid;
                model.Goals = meeting.Goals;
                model.StatusId = meeting.StatusId;
                model.OwnerId = meeting.CreatedBy;
                model.Date = DateTime.Now.Date == meeting.Date.Date ? meeting.Date.ToShortTimeString() : meeting.Date.ToShortDateString();
                model.Result = meeting.ResultComment;
                model.Comment = meeting.Comment;
            }
            return model;
        }

        public void UpdateCompany(CRMCompanyEditModel company, Guid userId)
        {
            IFormatProvider provider = new CultureInfo("en-US");

            if (company.LegalEntityId == 0)
            {
                var companyGuid = AddCompanyToBase(company, userId);


                var geo = BaseContext.LegalEntities.FirstOrDefault(m => m.LegalEntityGuid == companyGuid).GeoLocation;

                var legalEnitity = AddLocalLegalEnity(company, userId, BaseContext.GetUnitById<CRMContext.City>(geo.C_CityId).CityGuid, companyGuid);
                var detail = LocalContext.LegalEnityDetails.FirstOrDefault(m => m.C_LegalEntityId == legalEnitity.LegalEntityId);

                if (company.Activities != null)
                {
                    foreach (var item in company.Activities)
                    {
                        var service = LocalContext.CompanyServices.FirstOrDefault(m => m.CompanyServiceId == item);
                        LocalContext.InsertUnit(new LegalActivity()
                        {
                            ActivityFullName = service.CompanyService2.Name + ": " + service.Name,
                            C_LegalEntityId = detail.LegalEntityDetailId,
                            C_ServiceId = service.CompanyServiceId
                        });
                    }
                }
            }
            else
            {
                var legalEntity = LocalContext.GetUnitById<LegalEntity>(company.LegalEntityId);
                legalEntity.LegalEntityId = company.LegalEntityId;
                legalEntity.LegalName = company.LegalName;
                legalEntity.PublicName = company.PublicName;
                legalEntity.Mails = company.Mails;
                legalEntity.LogoPath = company.PhotoPath;
                legalEntity.Sites = company.Sites;
                legalEntity.Skype = company.Skype;
                legalEntity.GeoLocation.Address = company.Address;
                legalEntity.GeoLocation.Latitude = !string.IsNullOrEmpty(company.GeoAddr.Latitude) ? Convert.ToDouble(company.GeoAddr.Latitude, provider) : new Nullable<double>();
                legalEntity.GeoLocation.Longitude = !string.IsNullOrEmpty(company.GeoAddr.Longitude) ? Convert.ToDouble(company.GeoAddr.Longitude, provider) : new Nullable<double>();
                legalEntity.StatusId = company.StatusId;
                legalEntity.Phones = company.Phones;
                legalEntity.Assigned = company.AssignedBy;
                legalEntity.Comment = company.Comment;
                LocalContext.SaveChanges();

                var detail = LocalContext.LegalEnityDetails.FirstOrDefault(m => m.C_LegalEntityId == legalEntity.LegalEntityId);
                if (detail != null)
                {
                    detail.KPP = company.Details[0].KPP;
                    detail.KS = company.Details[0].KS;
                    detail.INN = company.Details[0].INN;
                    detail.OGRN = company.Details[0].OGRN;
                    detail.RS = company.Details[0].RS;
                    detail.BIK = company.Details[0].BIK;
                    detail.PayLocation = company.Details[0].PaymentLocation;
                    detail.IsActive = true;
                    LocalContext.SaveChanges();
                }
                else if(company.Details[0].INN !=0)
                {
                    detail = new LegalEnityDetail()
                                {
                                    KPP = company.Details[0].KPP,
                                    KS = company.Details[0].KS,
                                    INN = company.Details[0].INN,
                                    OGRN = company.Details[0].OGRN,
                                    RS = company.Details[0].RS,
                                    BIK = company.Details[0].BIK,
                                    PayLocation = company.Details[0].PaymentLocation,
                                    IsActive = true,
                                    Created = DateTime.Now,
                                    CreatedBy = userId,
                                    C_GeoId = legalEntity.C_MainGeoId,
                                    C_LegalEntityId = legalEntity.LegalEntityId
                                };
                    LocalContext.InsertUnit(detail);
                }
                if (detail != null)
                {
                    foreach (var item in LocalContext.LegalActivities.Where(m => m.C_LegalEntityId == detail.LegalEntityDetailId))
                    {
                        LocalContext.LegalActivities.Remove(item);
                    }
                    if (company.Activities != null)
                    {
                        foreach (var item in company.Activities)
                        {
                            var service = LocalContext.CompanyServices.FirstOrDefault(m => m.CompanyServiceId == item);
                            LocalContext.InsertUnit(new LegalActivity()
                            {
                                ActivityFullName = service.CompanyService2.Name + ": " + service.Name,
                                C_LegalEntityId = detail.LegalEntityDetailId,
                                C_ServiceId = service.CompanyServiceId,
                            });
                        }
                    }
                }
            }
        }

        private LegalEntity AddLocalLegalEnity(CRMCompanyEditModel company, Guid userId, Guid cityGuid, Guid companyGuid)
        {
            IFormatProvider provider = new CultureInfo("en-US");

            GeoLocation location = new GeoLocation()
            {
                Address = string.IsNullOrEmpty(company.Address)? "Не задан" : company.Address,
                CityGuid = cityGuid,
                Latitude = !string.IsNullOrEmpty(company.GeoAddr.Latitude) ? Convert.ToDouble(company.GeoAddr.Latitude, provider) : new Nullable<double>(),
                Longitude = !string.IsNullOrEmpty(company.GeoAddr.Longitude) ? Convert.ToDouble(company.GeoAddr.Longitude, provider) : new Nullable<double>(),
            };
            LocalContext.InsertUnit(location);

            var legalEnitity = new LegalEntity()
            {
                LegalEntityId = company.LegalEntityId,
                LegalName = company.LegalName,
                PublicName = company.PublicName,
                Mails = company.Mails,
                Sites = company.Sites,
                StatusId = company.StatusId,
                Phones = company.Phones,
                Assigned = company.AssignedBy,
                Comment = company.Comment,
                Created = DateTime.Now,
                Skype = company.Skype,
                LogoPath = company.PhotoPath,
                CreatedBy = userId,
                CompanyGuid = companyGuid,
                C_MainGeoId = location.GeoLocationId
            };
            LocalContext.InsertUnit(legalEnitity);
            LocalContext.InsertUnit(new LegalEnityDetail()
            {
                KPP = company.Details[0].KPP,
                KS = company.Details[0].KS,
                INN = company.Details[0].INN,
                OGRN = company.Details[0].OGRN,
                RS = company.Details[0].RS,
                BIK = company.Details[0].BIK,
                PayLocation  = company.Details[0].PaymentLocation,
                IsActive = true,
                Created = DateTime.Now,
                CreatedBy = userId,
                C_GeoId = location.GeoLocationId,
                C_LegalEntityId = legalEnitity.LegalEntityId
            });
            return legalEnitity;
        }

        public void UpdateMeeting(MeetingEditModel meeting)
        {
            if (meeting.MeetingId == 0)
            {
                var model = new Meeting();
                model.C_CustomerId = LocalContext.Customers.FirstOrDefault(m=>m.CustomerGuid == meeting.CustomerId).CustomerId;
                model.Goals = meeting.Goals;
                model.StatusId = meeting.StatusId;
                model.CreatedBy = meeting.OwnerId;
                model.Created = DateTime.Now;
                model.Date = Convert.ToDateTime(meeting.Date);
                model.ResultComment = meeting.Result;
                model.Comment = meeting.Comment;
                model.MeetingGuid = Guid.NewGuid();
                LocalContext.InsertUnit(model);
            }
            else
            {
                var model = LocalContext.GetUnitById<Meeting>(meeting.MeetingId);
                model.C_CustomerId = LocalContext.Customers.FirstOrDefault(m => m.CustomerGuid == meeting.CustomerId).CustomerId;
                model.Goals = meeting.Goals;
                model.StatusId = meeting.StatusId;
                model.CreatedBy = meeting.OwnerId;
                model.Date = Convert.ToDateTime(meeting.Date);
                model.ResultComment = meeting.Result;
                model.Comment = meeting.Comment;
                LocalContext.SaveChanges();
            }
        }

        public IList<CRMCompanyViewModel> GetCompanies()
        {
            return LocalContext.LegalEntities
                .ToList()
                .Select(m => new CRMCompanyViewModel()
            {
                CompanyGuid = m.CompanyGuid,
                LegalEntityId = m.LegalEntityId,
                LegalName = m.LegalName,
                PublicName = m.PublicName,
                StatusId = m.StatusId,
                ActivitiesStr = string.Join(", ", m.LegalEnityDetails.SelectMany(n => n.LegalActivities).Select(n => n.ActivityFullName)),
                AssignedBy = m.Assigned,
                Comment = m.Comment,
                Phones = m.Phones + (m.Customers.Count > 0 ? "," + string.Join(",", m.Customers.Where(n=>!string.IsNullOrEmpty(n.Phone)).Select(n=>n.Phone)) : string.Empty),
                Mails = m.Mails + (m.Customers.Count > 0 ? "," + string.Join(",", m.Customers.Where(n => !string.IsNullOrEmpty(n.Mail)).Select(n => n.Mail)) : string.Empty),
                Sites = m.Sites,
                PhotoPath = m.LogoPath,
                Skype = m.Skype,
                CityGuid = m.GeoLocation.CityGuid,
                GeoAddr = new GeoEditModel()
                {
                    Address = m.GeoLocation.Address,
                    Comment = m.GeoLocation.Comment,
                    Lat = m.GeoLocation.Latitude.HasValue ? m.GeoLocation.Latitude.Value : 0,
                    Long = m.GeoLocation.Longitude.HasValue ? m.GeoLocation.Longitude.Value : 0
                }
            }).ToList();
        }

        public List<CustomerViewModel> GetCustomers(int? companyId)
        {
            return companyId.HasValue? 
                LocalContext.GetUnitById<LegalEntity>(companyId.Value)
                .Customers
                .Select(n => new CustomerViewModel()
               {
                   CompanyId = n.LegalEntity.CompanyGuid,
                   FirstName = n.FirstName,
                   PositionName = n.Position,
                   IsLPR = n.IsLPR,
                   LastName = n.LastName,
                   Patronymic = n.Patronymic,
                   Mail = n.Mail,
                   PhotoPath = n.PhotoPath,
                   Skype = n.Skype,
                   StatusId = n.StatusId,
                   Phone = n.Phone,
                   Guid = n.CustomerGuid,
                   Comment = n.Comment,
                   CustomerId = n.CustomerId,
                   AssignedBy = n.Assigned.HasValue ? n.Assigned.Value : Guid.Empty,
               }).ToList()
               : LocalContext.Customers
               .Where(m=>!m.C_LegalEntityId.HasValue)
               .Select(n => new CustomerViewModel()
               {
                   CompanyId = null,
                   FirstName = n.FirstName,
                   PositionName = n.Position,
                   IsLPR = n.IsLPR,
                   LastName = n.LastName,
                   Guid = n.CustomerGuid,
                   Patronymic = n.Patronymic,
                   Mail = n.Mail,
                   StatusId = n.StatusId,
                   Phone = n.Phone,
                   PhotoPath = n.PhotoPath,
                   Skype = n.Skype,
                   Comment = n.Comment,
                   CustomerId = n.CustomerId,
                   AssignedBy = n.Assigned.HasValue ? n.Assigned.Value : Guid.Empty,
               }).ToList();
        }

        public IList<SelectListItem> GetSelectListCompanies()
        {
            return LocalContext.LegalEntities
                .Select(m => new { id = m.CompanyGuid, name = m.PublicName })
                .Select(m => new SelectListItem() { Text = m.name, Value = m.id.ToString() })
                .ToList();
        }

        public Dictionary<Guid, List<Guid>> GetCustomerIds()
        {
            var model = new Dictionary<Guid, List<Guid>>();
            var keys = LocalContext.LegalEntities.Where(m => m.Customers.Count == 0).Select(m => m.CompanyGuid).ToList();
            foreach (var item in keys.Distinct())
            {
                model.Add(item, new List<Guid>());
            }
            if (LocalContext.Customers.Where(m => m.C_LegalEntityId.HasValue).Count() > 0)
            {
                var items = LocalContext.Customers.Where(m => m.C_LegalEntityId.HasValue).Select(m => new { id = m.CustomerGuid, companyId = m.LegalEntity.CompanyGuid })
                   .GroupBy(m => m.companyId)
                   .ToDictionary(m => m.Key, m => m.Select(n => n.id).ToList());
                foreach (var item in items)
                {
                    model.Add(item.Key, item.Value);
                }
            };
            return model;
        }

        public IList<MeetingTaskPreview> GetMeetings(Guid? ownerId = null)
        {
            if (ownerId.HasValue)
            {
                return LocalContext.Meetings
                    .Where(m => m.CreatedBy == ownerId.Value)
                    .Select(m => new MeetingTaskPreview()
                {
                    CustomerId = m.Customer.CustomerGuid,
                    Date = m.Date,
                    CustomerName = m.Customer.LegalEntity.LegalName + ":" + m.Customer.FirstName + " " + m.Customer.LastName,
                    MeetingId = m.MeetingGuid,
                    OwnerId = m.CreatedBy,
                    StatusId = m.StatusId,
                    Result = m.ResultComment,

                }).ToList();
            }
            else
            {
                return LocalContext.Meetings.Select(m => new MeetingTaskPreview()
                {
                    CustomerId = m.Customer.CustomerGuid,
                    Date = m.Date,
                    CustomerName = m.Customer.LegalEntity.LegalName + ":" + m.Customer.FirstName + " " + m.Customer.LastName,
                    MeetingId = m.MeetingGuid,
                    OwnerId = m.CreatedBy,
                    StatusId = m.StatusId,
                    Result = m.ResultComment
                }).ToList();
            }
        }

        public IList<MeetingTaskPreview> GetMeetingsByCustomerIds(List<Guid> customers)
        {

            return LocalContext.Meetings
                .Where(m => customers.Contains(m.Customer.CustomerGuid))
                .Select(m => new MeetingTaskPreview()
            {
                CustomerId = m.Customer.CustomerGuid,
                Date = m.Date,
                CustomerName = m.Customer.LegalEntity.LegalName + ":" + m.Customer.FirstName + " " + m.Customer.LastName,
                MeetingId = m.MeetingGuid,
                OwnerId = m.CreatedBy,
                StatusId = m.StatusId,
                Result = m.ResultComment,

            }).ToList();
        }

        public IEnumerable<SelectListItem> GetCustomers4Subordinate(IEnumerable<Guid> availableOwners, int? companyId = null)
        {
            var customers =
                LocalContext.Customers
                .Where(m => (availableOwners.Contains(m.CreatedBy)
                    || (m.Assigned.HasValue && availableOwners.Contains(m.Assigned.Value))));

            if(companyId.HasValue)
                customers = customers.Where(m=>m.C_LegalEntityId.HasValue && m.C_LegalEntityId == companyId.Value);
            
            return customers
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = (string.IsNullOrEmpty(m.Phone) ? "Не задан телефон! " : string.Empty) + (m.C_LegalEntityId.HasValue? m.LegalEntity.LegalName : string.Empty ) + ": " + m.FirstName + " " + m.LastName + ". " + m.Phone,
                        Value = string.IsNullOrEmpty(m.Phone) ? "" : m.CustomerGuid.ToString()
                    }).ToList();
        }

        public IEnumerable<SelectListItem> GetSerivces4CustomerCompany(int customerId)
        {
            return LocalContext.GetUnitById<Customer>(customerId)
                .LegalEntity
                .LegalEnityDetails
                .SelectMany(m=> m.LegalActivities)
                .ToList()
                .Select(m => new SelectListItem()
                {
                    Text = m.ActivityFullName + (m.Cost.HasValue ? ": " + m.Cost.Value.ToString("c") : ""),
                    Value = m.LegalActivityId.ToString(),
                });
        }

        public void UpdateCompanyLegals(int companyId, Guid userId)
        {
            var company = Context.GetUnitById<CompanyContext.Company>(companyId);
            var legalOgrns = company.LegalEntities.Select(m => m.OGRN).ToList();
            var item = BaseContext.LegalEnityDetails.FirstOrDefault(m => legalOgrns.Contains(m.OGRN));
            int legalEntityId = TryUpdateCRMCompany(company, item, userId);

            if (item == null)
            {
                var curLegalAddr = company.LegalEntity.Address;
                if (curLegalAddr != null)
                {
                    CRMContext.GeoLocation geo = new CRMContext.GeoLocation()
                    {
                        Address = string.Format("{0} {1}, {2}{3}", curLegalAddr.City, curLegalAddr.Street, curLegalAddr.Number, curLegalAddr.AddNumber),
                        Latitude = curLegalAddr.Latitiude,
                        Longitude = curLegalAddr.Longitude,
                        C_CityId = BaseContext.Cities.FirstOrDefault(m => m.Name == curLegalAddr.City).CityId,
                        Comment = "Добавлен при обновлении данных о компании"
                    };
                    BaseContext.InsertUnit(geo);
                    item = new CRMContext.LegalEnityDetail()
                    {
                        C_LegalEntityId = legalEntityId,
                        INN = company.LegalEntity.INN,
                        KPP = company.LegalEntity.KPP,
                        KS = company.LegalEntity.KS,
                        OGRN = company.LegalEntity.OGRN,
                        RS = company.LegalEntity.RS,
                        BIK = company.LegalEntity.BIK,
                        CreatedBy = userId,
                        IsActive = true,
                        PayLocation = company.LegalEntity.PayLocation,
                        PayLocationRS = company.LegalEntity.RS,
                        Created = DateTime.Now,
                        C_GeoId = geo.GeoLocationId
                    };
                    BaseContext.LegalEnityDetails.Add(item);
                    BaseContext.SaveChanges();
                }
            }
            if (!company.CRMGuid.HasValue)
            {
                company.CRMGuid = item.LegalEntity.LegalEntityGuid;
                Context.SaveChanges();
            }

            CopyLegalEntity(company, userId);
        }

        private void CopyLegalEntity(CompanyContext.Company company, Guid userId)
        {
            var existLegal = LocalContext.LegalEntities.FirstOrDefault(m => m.CompanyGuid == company.CRMGuid.Value);
            var cityGuid = BaseContext.LegalEntities
                .FirstOrDefault(m => m.LegalEntityGuid == company.CRMGuid.Value)
                .GeoLocation.City.CityGuid;

            if (existLegal == null)
            {
                var geo = new GeoLocation()
                {
                    Address = string.Format("{0}, {1} {2}", company.Address.Street, company.Address.Number, company.Address.AddNumber),
                    CityGuid = cityGuid
                };
                LocalContext.InsertUnit(geo);
                existLegal = new LegalEntity()
                {
                    CreatedBy = userId,
                    C_MainGeoId = geo.GeoLocationId,
                    LegalName = company.CompanyName,
                    PublicName = string.IsNullOrEmpty(company.ShortComanyName) ? string.Empty : company.ShortComanyName,
                    Mails = company.Email,
                    Phones = string.Empty,
                    CompanyGuid = company.CRMGuid.Value,
                    Created = DateTime.Now,
                };
                LocalContext.InsertUnit(existLegal);
            }

            var legalOgrns = company.LegalEntities.Select(m => m.OGRN).ToList();
            var item = LocalContext.LegalEnityDetails.FirstOrDefault(m => legalOgrns.Contains(m.OGRN));
            if (item == null)
            {
                var curLegalAddr = company.LegalEntity.Address;
                GeoLocation geo = new GeoLocation()
                {
                    Address = string.Format("{0} {1}, {2}{3}", curLegalAddr.City, curLegalAddr.Street, curLegalAddr.Number, curLegalAddr.AddNumber),
                    Latitude = curLegalAddr.Latitiude,
                    Longitude = curLegalAddr.Longitude,
                    CityGuid = cityGuid,
                    Comment = "Добавлен при обновлении данных о компании"
                };

                LocalContext.InsertUnit(geo);
                item = new LegalEnityDetail()
                {
                    C_LegalEntityId = existLegal.LegalEntityId,
                    INN = company.LegalEntity.INN,
                    KPP = company.LegalEntity.KPP,
                    KS = company.LegalEntity.KS,
                    OGRN = company.LegalEntity.OGRN,
                    RS = company.LegalEntity.RS,
                    BIK = company.LegalEntity.BIK,
                    CreatedBy = userId,
                    IsActive = true,
                    PayLocation = company.LegalEntity.PayLocation,
                    PayLocationRS = company.LegalEntity.RS,
                    C_GeoId = geo.GeoLocationId,
                    Created = DateTime.Now
                };
                LocalContext.LegalEnityDetails.Add(item);
                LocalContext.SaveChanges();
            }
        }

        private int TryUpdateCRMCompany(CompanyContext.Company company, CRMContext.LegalEnityDetail legalDetail, Guid userId)
        {
            int result = 0;
            if (legalDetail != null)
            {
                result = legalDetail.C_LegalEntityId;
                legalDetail.LegalEntity.ConfirmedBy = userId;
            }
            else
            {
                var pretendentByName = BaseContext.LegalEntities.FirstOrDefault(m =>
                                       m.LegalName == company.CompanyName
                                    || m.LegalName == company.ShortComanyName
                                    || m.PublicName == company.CompanyName
                                    || m.PublicName == company.ShortComanyName);
                if (pretendentByName != null)
                {
                    pretendentByName.ConfirmedBy = userId;
                    result = pretendentByName.LegalEntityId;
                }
                else
                {
                    var city = BaseContext.Cities.FirstOrDefault(m => m.Name == company.Address.City);
                    if (city == null)
                    {
                        city = new CRMContext.City()
                        {
                            Name = company.Address.City,
                            Code = string.Empty
                        };
                        BaseContext.InsertUnit(city);
                    }

                    var geo = new CRMContext.GeoLocation()
                    {
                        Address = string.Format("{0}, {1} {2}", company.Address.Street, company.Address.Number, company.Address.AddNumber),
                        C_CityId = city.CityId
                    };
                    BaseContext.InsertUnit(geo);

                    CRMContext.LegalEntity legalEntity = new CRMContext.LegalEntity()
                   {
                       ConfirmedBy = userId,
                       CreatedBy = userId,
                       C_MainGeoId = geo.GeoLocationId,
                       LegalName = company.CompanyName,
                       PublicName = company.ShortComanyName,
                       Mails = company.Email,
                       Phones = string.Empty
                   };
                    BaseContext.InsertUnit(legalEntity);
                    result = legalEntity.LegalEntityId;
                }
            }
            return result;
        }

        public void CopyCompanyLocal(Guid companyId, Guid userId)
        {
            var company = BaseContext.LegalEntities.FirstOrDefault(m => m.LegalEntityGuid == companyId);
            IFormatProvider provider = new CultureInfo("en-US");

            GeoLocation location = new GeoLocation()
            {
                Address = company.GeoLocation.Address,
                CityGuid = company.GeoLocation.City.CityGuid,
                Latitude = company.GeoLocation.Latitude,
                Longitude = company.GeoLocation.Longitude,
            };
            LocalContext.InsertUnit(location);

            var legalEnitity = new LegalEntity()
            {
                LegalEntityId = company.LegalEntityId,
                LegalName = company.LegalName,
                PublicName = company.PublicName,
                Mails = company.Mails,
                Sites = company.Sites,
                StatusId = (byte)CompanyStatus.Novelty,
                Phones = company.Phones,
                Assigned = userId,
                Comment = string.Empty,
                Created = DateTime.Now,
                CreatedBy = userId,
                CompanyGuid = company.LegalEntityGuid,
                C_MainGeoId = location.GeoLocationId
            };
            LocalContext.InsertUnit(legalEnitity);

            foreach (var item in company.LegalEnityDetails)
            {
                LocalContext.InsertUnit(new LegalEnityDetail()
                {
                    KPP = item.KPP,
                    KS = item.KS,
                    INN = item.INN,
                    OGRN = item.OGRN,
                    RS = item.RS,
                    BIK = item.BIK,
                    IsActive = true,
                    Created = DateTime.Now,
                    CreatedBy = userId,
                    C_GeoId = location.GeoLocationId,
                    C_LegalEntityId = legalEnitity.LegalEntityId
                });
            }

            foreach (var item in company.LegalEntityAddPhones)
            {
                LocalContext.InsertUnit(new LegalEntityAddPhone()
                {
                    Phones = item.Phones,
                    C_LegalEntityId = legalEnitity.LegalEntityId,
                    C_GeoLocationId = location.GeoLocationId
                });
            }

            BaseContext.InsertUnit(new CRMContext.LegalCopy()
            {
                CopyBy = userId,
                CopyDate = DateTime.Now,
                C_LegalEntityId = company.LegalEntityId
            });
           
        }

        public override List<SelectListItem> GetServices(IEnumerable<int> parentServices, bool isAll = false)
        {
            if (isAll)
            {
                return LocalContext.LegalActivities
                .ToList()
                .Select(m => new SelectListItem()
                {
                    Text = m.ActivityFullName + ": " + m.Cost,
                    Value = m.LegalActivityId.ToString(),
                    Selected = false
                }).ToList();
            }
            else
            {
                return LocalContext.CompanyServices
                    .Where(m => m.C_ParentId.HasValue)
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = m.CompanyService2.Name.ToUpper() + " - " + m.Name,
                        Value = m.CompanyServiceId.ToString(),
                        Selected = m.C_ParentId.HasValue && parentServices.Contains(m.C_ParentId.Value)
                    }).ToList();
            }
        }

        public void UpdateCustomerInfo(IEnumerable<CallTaskPreview> CallTasks)
        {
            var ids= CallTasks.Select(m => m.CustomerId).ToList();
           var existCustomers =  LocalContext.Customers.Where(m => ids.Contains(m.CustomerGuid)).Select(m => new { id = m.CustomerGuid, phone = m.Phone, company = m.LegalEntity.LegalName, name = m.FirstName + " " + m.LastName });
           foreach (var item in CallTasks)
           {
               var customer = existCustomers.FirstOrDefault(m => m.id == item.CustomerId);
               item.AssignId = ids[0];
               LocalContext.SaveChanges();
               if (customer != null)
               {
                   item.Phone = customer.phone;
                   item.CustomerName = customer.name;
                   item.CompanyName = customer.company;
               }
           }
        }

        public Customer GetCustomerByGuid(Guid guid)
        {
            return LocalContext.Customers
                .FirstOrDefault(m => m.CustomerGuid == guid);
        }

        public void UpdateSheduledEvents(Dictionary<DateTime, IEnumerable<CRMEventPreview>> SheduledEvents, IEnumerable<Guid> userIds)
        {
            var items = LocalContext.Meetings.Where(m =>
                   m.StatusId != (byte)EnumHelper.TaskStatus.Completed
                   && userIds.Contains(m.CreatedBy))
                   .Select(m => new CRMEventPreview()
                   {
                       TypeId = (byte)CRMEventType.Meeting,
                       ContactId = m.Customer.CustomerGuid,
                       ContactName = m.Customer.FirstName + " " + m.Customer.LastName + (m.Customer.C_LegalEntityId.HasValue ?
                        m.Customer.LegalEntity.PublicName
                        : "физ. лицо"),
                       OwnerId = m.CreatedBy,
                       StatusId = m.StatusId,
                       EventDate = m.Date,
                       EventId = m.MeetingGuid
                   }).ToList();
            var customers = SheduledEvents.SelectMany(m => m.Value.Where(n => n.ContactId.HasValue).Select(n => n.ContactId.Value));
            var customerNames = LocalContext.Customers
                .Where(m => customers.Contains(m.CustomerGuid))
                .ToList()
                .Select(m => new
                {
                    id = m.CustomerGuid,
                    name = m.FirstName + " " + m.LastName + (m.C_LegalEntityId.HasValue ?
                        m.LegalEntity.PublicName
                        : "физ. лицо")
                });
            foreach (var item in SheduledEvents.Keys.ToList())
            {
                foreach (var eventItem in SheduledEvents[item].Where(m => m.ContactId.HasValue))
                {
                    eventItem.ContactName = customerNames.FirstOrDefault(m => m.id == eventItem.ContactId.Value).name;
                }
                SheduledEvents[item] = SheduledEvents[item].Union(items.Where(m => m.EventDate.Date == item).ToList());
            }
        }

        public IEnumerable<SelectListItem> GetExpenseServices()
        {
            return LocalContext.Expenses.ToList()
                .Select(m => new SelectListItem()
                {
                    Text = m.Name,
                    Value = m.ExpenseId.ToString()
                })
                .ToList();
        }

        public LegalEntityViewModel GetLegalDetail(int? legalEntityDetailId)
        {
            var result = new LegalEntityViewModel();
            if (legalEntityDetailId.HasValue)
            {
                var detail = LocalContext.GetUnitById<LegalEnityDetail>(legalEntityDetailId.Value);
                result.BIK = detail.BIK;
                result.INN = detail.INN;
                result.IsActive = detail.IsActive;
                result.KPP = detail.KPP;
                result.KS = detail.KS;
                result.OGRN = detail.OGRN;
                result.RS = detail.RS;
                result.PaymentLocation = detail.GeoLocation.Address;
                result.LegalEntityId = detail.LegalEntityDetailId;
            }
            return result;
        }

        public void ChangeAssign(CRMCompanyEditModel model, Guid userId, Dictionary<Guid, string> names)
        {
            var item = LocalContext.GetUnitById<LegalEntity>(model.LegalEntityId);
            if (item.Assigned != model.AssignedBy)
            {
                if (model.AssignedBy.HasValue)
                    WriteModify(string.Format(" Закреплена за новым сотрудником.'{0}' вместо '{1}'. Причина:'{2}' ",
                        names[model.AssignedBy.Value],
                        item.Assigned.HasValue ?
                            names[item.Assigned.Value]
                            : "Свободная компания", model.Comment)
                            , userId, item.CompanyGuid);
                else if (item.Assigned.HasValue)
                    WriteModify(string.Format("Свободная компания вместо {0}. Причина:'{1}' ", names[item.Assigned.Value], model.Comment), userId, item.CompanyGuid);
            }
            // todo logged action
            item.Assigned = model.AssignedBy;
            LocalContext.SaveChanges();
        }

        public void UpdateComment(CRMCompanyEditModel model, Guid userId)
        {
             var item = LocalContext.GetUnitById<LegalEntity>(model.LegalEntityId);
            if (item.Comment  != model.Comment)
            {
                WriteModify(string.Format(" Изменен комментарий.'{0}' \r\n НА \r\n '{1}' ", item.Comment, model.Comment),userId, item.CompanyGuid);
            }
            item.Comment = model.Comment;
            LocalContext.SaveChanges();
        }

        public void ChangeStatus(CRMCompanyEditModel model, Guid userId)
        {
              var item = LocalContext.GetUnitById<LegalEntity>(model.LegalEntityId);
              if (item.StatusId != model.StatusId)
              {
                  WriteModify(string.Format(" Изменен статус.'{0}' \r\n НА \r\n '{1}' ", ((CompanyStatus)item.StatusId).GetStringValue(), ((CompanyStatus)model.StatusId).GetStringValue()), userId, item.CompanyGuid);
              }
            item.StatusId = model.StatusId;
            LocalContext.SaveChanges();
        }
        public void ChangeAssign(CustomerEditModel model, Guid userId, Dictionary<Guid, string> names)
        {
            var item = LocalContext.GetUnitById<Customer>(model.CustomerId);
            if (item.Assigned != model.AssignedBy)
            {
                if (model.AssignedBy.HasValue)
                    WriteModify(string.Format(" Закреплен за новым сотрудником.{0} вместо {1} ",
                        names[model.AssignedBy.Value],
                        item.Assigned.HasValue ?
                            names[item.Assigned.Value]
                            : "Cвободное физ.лицо")
                            , userId, item.CustomerGuid);
                else if (item.Assigned.HasValue)
                    WriteModify(string.Format("Cвободное физ.лицо вместо {0} ", names[item.Assigned.Value]), userId, item.CustomerGuid);
            }
            item.Assigned = model.AssignedBy;
            LocalContext.SaveChanges();
        }

        public void ChangeStatus(CustomerEditModel model, Guid userId)
        {
            var item = LocalContext.GetUnitById<Customer>(model.CustomerId);
            if (item.StatusId != model.StatusId)
            {
                WriteModify(string.Format(" Изменен статус.'{0}' \r\n НА \r\n '{1}' ", ((CustomerStatus)item.StatusId).GetStringValue(), ((CustomerStatus)model.StatusId).GetStringValue()), userId, item.CustomerGuid);
            }
            item.StatusId = model.StatusId;
            LocalContext.SaveChanges();
        }

        public void UpdateComment(CustomerEditModel model, Guid userId)
        {
            var item = LocalContext.GetUnitById<Customer>(model.CustomerId);
            if (item.Comment != model.Comment)
            {
                WriteModify(string.Format(" Изменен комментарий.'{0}' \r\n НА \r\n '{1}' ", item.Comment, model.Comment), userId, item.CustomerGuid);
            }
            item.Comment = model.Comment;
            LocalContext.SaveChanges();
        }

        public IEnumerable<Meeting> GetMeetingsByCustomer(int customerId)
        {
            return LocalContext.Meetings.Where(m => m.C_CustomerId == customerId);
        }

        public LegalEntity GetCompany(Guid companyId)
        {
            return LocalContext.LegalEntities.FirstOrDefault(m => m.CompanyGuid == companyId);
        }

        public List<MessageViewModel> GetModifyLog(Guid companyId)
        {
            var items = LocalContext.ModifyLogs.Where(m => m.ObjId == companyId)
                .Select(m => new MessageViewModel()
                {
                    Created = m.Created,
                    UserId = m.CreatedBy,
                    Type = (byte)MsgType.ModifyLog,
                    Msg = m.Msg
                });
            return items.ToList();
        }

        private void WriteModify(string msg, Guid userId, Guid objId)
        {

            LocalContext.InsertUnit(new ModifyLog()
            {
                Msg = msg,
                ObjId = objId,
                Created = DateTime.Now,
                CreatedBy = userId
            });
        }

        public List<CRMModel.TaskPeriod> GetTaskPeriods(Guid guid)
        {
            return LocalContext.TaskPeriods.Where(m => m.TaskTicketId == guid)
                 .ToList()
                 .Select(m => 
                     new CRMModel.TaskPeriod()
                     {
                         TicketId = guid,
                         AddNumber = m.AddNumber.ToString(),
                         Assigned = m.Assigned,
                         Comment = m.Comment,
                         DateStarted = m.DateStarted,
                         NotifyBefore = m.NotifyBefore,
                         PeriodType = (TaskPeriodType)m.PeriodType,
                         StatusId = (WFTaskStatus)m.StatusId,
                         TaskPeriodId = m.TaskPeriodId

                     })
                     .ToList();
        }

        public void CreatePeriods(List<CRMModel.TaskPeriod> periods, TaskGroup group)
        {
            var curPeriod = periods[0];
            int addDays = 1;
            switch (curPeriod.PeriodType)
            {
                case TaskPeriodType.Monthly:
                    addDays = 30;
                    break;
                case TaskPeriodType.Quaterly:
                    addDays = 90;
                    break;
                case TaskPeriodType.Weekly:
                    addDays = 7;
                    break;
                case TaskPeriodType.Yearly:
                    addDays = 365;
                    break;
                case TaskPeriodType.Daily:
                default:
                    addDays = 1;
                    break;
            };
            IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;

            if (!string.IsNullOrEmpty(curPeriod.DateStartedStr))
                curPeriod.DateStarted = Convert.ToDateTime(curPeriod.DateStartedStr, ruDateFormat);
            else
                curPeriod.DateStarted = DateTime.Now.Date;

            if (!string.IsNullOrEmpty(curPeriod.DateBeforeStr))
                curPeriod.DateBefore = Convert.ToDateTime(curPeriod.DateBeforeStr, ruDateFormat);
            else
                curPeriod.DateBefore = DateTime.Now.Date.AddYears(20);

            int maxCountTasks = 1;
            DateTime curDate = curPeriod.DateStarted;
            if (group != null && group.AssignedUsers!=null && group.AssignedUsers.Count > 0 )
            {
                foreach (var item in group.AssignedUsers)
                {
                    do
                    {
                        LocalContext.InsertUnit(new CRMLocalContext.TaskPeriod()
                        {
                            TaskPeriodId = Guid.NewGuid(),
                            Assigned = Guid.Parse(item),
                            TaskTicketId = curPeriod.TicketId,
                            StatusId = (byte)WFTaskStatus.Novelty,
                            Comment = curPeriod.Comment,
                            PeriodType = (byte)curPeriod.PeriodType,
                            NotifyBefore = curPeriod.NotifyBefore,
                            DateStarted = curDate.Add(Convert.ToDateTime(curPeriod.TimeStartedStr).TimeOfDay),
                            AddNumber = maxCountTasks
                        });
                        maxCountTasks++;
                        if (curPeriod.PeriodType == TaskPeriodType.Monthly)
                            curDate = curDate.AddMonths(1);
                        else if (curPeriod.PeriodType == TaskPeriodType.Yearly)
                            curDate = curDate.AddYears(1);
                        else if (curPeriod.PeriodType == TaskPeriodType.Quaterly)
                            curDate = curDate.AddMonths(3);
                        else
                           curDate= curDate.AddDays(addDays);
                    }
                    while (maxCountTasks < 50 && curDate < curPeriod.DateBefore);
                }
            }
            else
                do
                {
                    LocalContext.InsertUnit(new CRMLocalContext.TaskPeriod()
                    {
                        TaskPeriodId = Guid.NewGuid(),
                        Assigned = curPeriod.Assigned,
                        TaskTicketId = curPeriod.TicketId,
                        StatusId = (byte)WFTaskStatus.Novelty,
                        Comment = curPeriod.Comment,
                        PeriodType = (byte)curPeriod.PeriodType,
                        NotifyBefore = curPeriod.NotifyBefore,
                        DateStarted = curDate.Add(Convert.ToDateTime(curPeriod.TimeStartedStr).TimeOfDay),
                        AddNumber = maxCountTasks
                    });
                    maxCountTasks++;
                    if (curPeriod.PeriodType == TaskPeriodType.Monthly)
                        curDate = curDate.AddMonths(1);
                    else if (curPeriod.PeriodType == TaskPeriodType.Yearly)
                        curDate = curDate.AddYears(1);
                    else if (curPeriod.PeriodType == TaskPeriodType.Quaterly)
                        curDate = curDate.AddMonths(3);
                    else
                        curDate = curDate.AddDays(addDays);
                }
                while (maxCountTasks > 50 && curDate < curPeriod.DateBefore);
        }

        public void UpdatePeriods(List<CRMModel.TaskPeriod> periods, TaskGroup group)
        {
            if (periods.Count == 1 && periods[0].TaskPeriodId == Guid.Empty)
                CreatePeriods(periods, group);
            else
            {
                foreach (var item in periods)
                {
                    var existItem = LocalContext.GetUnitById<CRMLocalContext.TaskPeriod>(item.TaskPeriodId);
                    existItem.Assigned = item.Assigned;
                    existItem.TaskTicketId = item.TicketId;
                    existItem.StatusId = (byte)item.StatusId;
                    existItem.TaskPeriodId = item.TaskPeriodId;
                    existItem.Comment = item.Comment;
                    existItem.PeriodType = (byte)item.PeriodType;
                    existItem.NotifyBefore = item.NotifyBefore;
                    existItem.DateStarted = item.DateStarted;
                    existItem.AddNumber = int.Parse(item.AddNumber);
                }
                LocalContext.SaveChanges();
            }
        }

        public IEnumerable<SelectListItem> GetTaskCatergories()
        {
            return LocalContext.TaskCategories
                .ToList()
                .Select(m => new SelectListItem() { Text = m.Name, Value = m.TaskCategoryId.ToString() })
                .ToList();
        }

        public SelectListItem GetCategory(int? categoryId)
        {
            SelectListItem model = new SelectListItem();
            if (categoryId.HasValue)
                LocalContext.GetUnitById<TaskCategory>(categoryId.Value);
            return model;
        }

        public void UpdateCategory(SelectListItem item)
        {
            if (string.IsNullOrEmpty(item.Value))
                LocalContext.InsertUnit(new TaskCategory() { Name = item.Text });
            else
            {
                LocalContext.GetUnitById<TaskCategory>(int.Parse(item.Value)).Name = item.Text;
                LocalContext.SaveChanges();
            }
        }

        public IEnumerable<Guid> FindPeriodTickets(IEnumerable<Guid> ids)
        {
            return LocalContext.TaskPeriods.Where(m => ids.Contains(m.TaskTicketId)).Select(m => m.TaskTicketId).Distinct();
        }

        public CRMModel.TaskPeriod GetPeriod(Guid periodId)
        {
            var exist = LocalContext.GetUnitById<CRMLocalContext.TaskPeriod>(periodId);
            var model = new CRMModel.TaskPeriod()
            {
                TaskPeriodId = exist.TaskPeriodId,
                AddNumber = exist.AddNumber.ToString(),
                StatusId = (WFTaskStatus)exist.StatusId,
                Comment = exist.Comment,
                Assigned = exist.Assigned,
                DateStartedStr = exist.DateStarted.ToString("dd.MM.yyyy"),
                TimeStartedStr = exist.DateStarted.ToString("hh:mm"),
                TicketId = exist.TaskTicketId
            };
            return model;
        }

        public void UpdatePeriod(CRMModel.TaskPeriod period)
        {
            IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;

            var exist = LocalContext.GetUnitById<CRMLocalContext.TaskPeriod>(period.TaskPeriodId);
            exist.StatusId = (byte)period.StatusId;
            exist.Comment = period.Comment;
            exist.Assigned = period.Assigned;
            exist.DateStarted = Convert.ToDateTime(period.DateStartedStr, ruDateFormat).Add(Convert.ToDateTime(period.TimeStartedStr).TimeOfDay);
            LocalContext.SaveChanges();
        }

        public IEnumerable<LegalEntitySearch> GetLegalCompanyPreviews(LegalEntitySearch model)
        {
            var exitsids = LocalContext.LegalEntities.Select(m => m.CompanyGuid).ToList();
            var existLegalEnititeDetails = BaseContext.LegalEnityDetails as IQueryable<CRMContext.LegalEnityDetail>;
            if (model.INN.HasValue)
                existLegalEnititeDetails = existLegalEnititeDetails.Where(n => n.INN == model.INN);
            if (model.KPP.HasValue)
                existLegalEnititeDetails = existLegalEnititeDetails.Where(n => n.KPP == model.KPP);
            if (model.OGRN.HasValue)
                existLegalEnititeDetails = existLegalEnititeDetails.Where(n => n.OGRN == model.OGRN);
            if (model.RS.HasValue)
                existLegalEnititeDetails = existLegalEnititeDetails.Where(n => n.RS == model.RS);


            var existLegalEnitites = BaseContext.LegalEntities as IQueryable<CRMContext.LegalEntity>;

            if (!string.IsNullOrEmpty(model.WebSite) && model.WebSite.Length > 3)
                existLegalEnitites = existLegalEnitites.Where(n => n.Sites.Contains(model.WebSite));
            if (!string.IsNullOrEmpty(model.EMail) && model.EMail.Length > 5)
                existLegalEnitites = existLegalEnitites.Where(n => n.Mails.Contains(model.EMail));
            if (!string.IsNullOrEmpty(model.MaskedToPhone) && model.MaskedToPhone.Length > 5)
                existLegalEnitites = existLegalEnitites.Where(n => n.Phones.Contains(model.MaskedToPhone));
            if (!string.IsNullOrEmpty(model.CompanyName) && model.CompanyName.Length > 3)
                existLegalEnitites = existLegalEnitites.Where(n => n.LegalName.Contains(model.CompanyName) || n.PublicName.Contains(model.CompanyName));
            if (existLegalEnitites.Count() > 1)
            {
                if (model.City != null && model.City.Count > 0)
                    existLegalEnitites = existLegalEnitites.Where(n => model.City.Contains(n.GeoLocation.C_CityId)
                        || (n.C_CurrentLegalDetailId.HasValue && n.LegalEnityDetail.C_GeoId.HasValue && model.City.Contains(n.LegalEnityDetail.GeoLocation.C_CityId)));
                if (model.Services != null && model.Services.Count > 0)
                    existLegalEnitites = existLegalEnitites.Where(m => m.LegalActivities.Any(n => model.Services.Contains(n.C_ServiceId)));
            }
            var searchResult = existLegalEnitites.ToList().Select(m => new LegalEntitySearch()
            {
                CompanyId = m.LegalEntityGuid,
                INN = m.C_CurrentLegalDetailId.HasValue ? m.LegalEnityDetail.INN : new Nullable<long>(),
                CompanyName = m.LegalName,
                EMail = m.Mails,
                Phone = m.Phones,
                OGRN = m.C_CurrentLegalDetailId.HasValue ? m.LegalEnityDetail.OGRN : new Nullable<long>(),
                KPP = m.C_CurrentLegalDetailId.HasValue ? m.LegalEnityDetail.KPP : new Nullable<long>(),
                RS = m.C_CurrentLegalDetailId.HasValue ? m.LegalEnityDetail.RS : new Nullable<long>(),
                WebSite = m.Sites,
                ActivityName = string.Join(", ", m.LegalActivities.Select(n => n.ServiceFullName)),
                IsExist = exitsids.Contains(m.LegalEntityGuid)
            })
                            .ToList();
            if (model.IsLegalSearch && existLegalEnititeDetails.Count() > 0)
            {
                return existLegalEnititeDetails
                          .ToList()
                          .Select(m => new LegalEntitySearch()
                          {
                              CompanyId = m.LegalEntity.LegalEntityGuid,
                              INN = m.INN,
                              CompanyName = m.LegalEntity.LegalName,
                              EMail = m.LegalEntity.Mails,
                              Phone = m.LegalEntity.Phones,
                              OGRN = m.OGRN,
                              KPP = m.KPP,
                              RS = m.RS,
                              WebSite = m.LegalEntity.Sites,
                              ActivityName = string.Join(", ", m.LegalEntity.LegalActivities.Select(n => n.ServiceFullName)),
                                IsExist = exitsids.Contains(m.LegalEntity.LegalEntityGuid)

                          }).ToList();
            }
            return searchResult;
        }

        public IEnumerable<CompanyAddressViewModel> GetLocations(int companyId)
        {
            var legalEntity = LocalContext.GetUnitById<LegalEntity>(companyId);
            var items = new List<GeoLocation>();
            items.AddRange(legalEntity.GeoLocations);
            items.Add(legalEntity.GeoLocation);
            return items.Select(m => new CompanyAddressViewModel()
            {
                Latitude = m.Latitude.ToString(),
                Longitude = m.Longitude.ToString(),
                Phones = m.LegalEntityAddPhones.Count > 0 ? string.Join(",", m.LegalEntityAddPhones.Select(n => n.Phones)) : string.Empty,
                AddrId = m.GeoLocationId,
                CompanyId = companyId,
                CityGuid = m.CityGuid,
                Address = m.Address
            }).ToList();
        }

        public void UpdateLegalAddress(CompanyAddressViewModel model)
        {
            IFormatProvider provider = new CultureInfo("en-US");

            if (model.AddrId == 0)
            {
                GeoLocation geo = new GeoLocation()
                {
                    Address = model.Address,
                    Latitude = Convert.ToDouble(model.Latitude, provider),
                    Longitude = Convert.ToDouble(model.Longitude, provider),
                    CityGuid = model.CityGuid,
                };
                
                LocalContext.InsertUnit(geo);
                LocalContext.LegalEntities
                    .FirstOrDefault(m => m.LegalEntityId == model.CompanyId)
                    .GeoLocations
                    .Add(geo);
                LocalContext.SaveChanges();
                if (!string.IsNullOrEmpty(model.Phones))
                {
                    LocalContext.InsertUnit(new LegalEntityAddPhone()
                    {
                        Phones = model.Phones,
                        C_LegalEntityId = model.CompanyId,
                        C_GeoLocationId = geo.GeoLocationId
                    });
                }
            }
            else
            {
                var geo = LocalContext.GetUnitById<GeoLocation>(model.AddrId);
                geo.Address = model.Address;
                    geo.Latitude = Convert.ToDouble(model.Latitude, provider);
                    geo.Longitude = Convert.ToDouble(model.Longitude, provider);
                    geo.CityGuid = model.CityGuid;
                    LocalContext.SaveChanges();
                if (!string.IsNullOrEmpty(model.Phones))
                {
                    if (!geo.LegalEntityAddPhones.Any(m => m.C_LegalEntityId == model.CompanyId))
                    {
                        LocalContext.InsertUnit(new LegalEntityAddPhone()
                        {
                            Phones = model.Phones,
                            C_LegalEntityId = model.CompanyId,
                            C_GeoLocationId = geo.GeoLocationId
                        });
                    }
                    else
                    {
                        geo.LegalEntityAddPhones.FirstOrDefault(m => m.C_LegalEntityId == model.CompanyId).Phones = model.Phones;
                        LocalContext.SaveChanges();
                    }
                }
            }
        }

        public CompanyAddressViewModel GetLocation(int companyId, int? addrId)
        {
            var geo = LocalContext.GetUnitById<LegalEntity>(companyId).GeoLocation;
            var model = new CompanyAddressViewModel()
            {
                CityGuid = geo.CityGuid,
                Latitude = geo.Latitude.ToString().Replace(",", "."),
                Longitude = geo.Longitude.ToString().Replace(",", ".")
            };

            if (addrId.HasValue)
            {
                var curGeo = LocalContext.GetUnitById<GeoLocation>(addrId.Value);
                model.Latitude = curGeo.Latitude.ToString().Replace(",",".");
                model.Longitude = curGeo.Longitude.ToString().Replace(",", ".");
                    model.Phones = curGeo.LegalEntityAddPhones.Where(m=>m.C_LegalEntityId == companyId).Count() > 0 ?
                        string.Join(",", curGeo.LegalEntityAddPhones.Where(m => m.C_LegalEntityId == companyId).Select(n => n.Phones))
                        : string.Empty;
                    model.AddrId = curGeo.GeoLocationId;
                    model.CompanyId = companyId;
                    model.CityGuid = curGeo.CityGuid;
                    model.Address = curGeo.Address;
            }
            return model;
        }

        public List<MessageViewModel> GetMeetingsHistory(Guid companyId)
        {
            return new List<MessageViewModel>();

        }
    }
}
