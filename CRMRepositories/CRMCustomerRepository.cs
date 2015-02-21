using CRMLocalContext;
using CRMModel;
using EnumHelper.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EntityRepository;
using CompanyModel;
using System.Globalization;

namespace CRMRepositories
{
    public class CRMCustomerRepository : CRMLocalRepository, IDisposable
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU");

        public CRMCustomerRepository(Guid userId)
            :base(userId)
        {}

        public List<SelectListItem> GetCompanies4Act()
        {
            return LocalContext.LegalEntities
                    .Select(m => new
                    {
                        text  = m.LegalName,
                        id = m.CompanyGuid
                    })
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = m.text,
                        Value = m.id.ToString()
                    })
                    .ToList();
        }

        public override IList<CustomerViewModel> GetCustomers(Guid? companyId)
        {
            if (companyId.HasValue)
                return LocalContext.LegalEntities
                   .FirstOrDefault(m => m.CompanyGuid == companyId)
                   .Customers.Select(n => new CustomerViewModel()
                   {
                       CompanyId = n.LegalEntity.CompanyGuid,
                       FirstName = n.FirstName,
                       PositionName = n.Position,
                       IsLPR = n.IsLPR,
                       LastName = n.LastName,
                       Patronymic = n.Patronymic,
                       Mail = n.Mail,
                       StatusId = n.StatusId,
                       Phone = n.Phone,
                       CustomerId = n.CustomerId,
                       Comment = n.Comment,
                       UserId = n.CustomerGuid,
                       AssignedBy = n.Assigned.HasValue ? n.Assigned.Value : Guid.Empty,
                   }).ToList();
            else
                return LocalContext.Customers
                           .Where(m => !m.C_LegalEntityId.HasValue).Select(n => new CustomerViewModel()
                           {
                               CompanyId = n.LegalEntity.CompanyGuid,
                               FirstName = n.FirstName,
                               PositionName = n.Position,
                               IsLPR = n.IsLPR,
                               LastName = n.LastName,
                               Patronymic = n.Patronymic,
                               Mail = n.Mail,
                               CustomerId = n.CustomerId,
                               StatusId = n.StatusId,
                               Phone = n.Phone,
                               Comment = n.Comment,
                               UserId = n.CustomerGuid,
                               AssignedBy = n.Assigned.HasValue ? n.Assigned.Value : Guid.Empty,
                           }).ToList();
        }

        public CustomerEditModel GetCustomerEditModel(int? customerId)
        {
            CustomerEditModel model = new CustomerEditModel();
            if (customerId.HasValue)
            {
                var customer = LocalContext.GetUnitById<Customer>(customerId.Value);
                model.CompanyId = customer.C_LegalEntityId.HasValue? 
                    customer.LegalEntity.CompanyGuid
                    :new Nullable<Guid>();
                model.CustomerId = customer.CustomerId;
                model.FirstName = customer.FirstName;
                model.PositionName = customer.Position;
                model.IsLPR = customer.IsLPR;
                model.LastName = customer.LastName;
                model.Patronymic = customer.Patronymic;
                model.Mail = customer.Mail;
                model.Skype = customer.Skype;
                model.StatusId = customer.StatusId;
                model.PhotoPath = customer.PhotoPath;
                model.Phone = customer.Phone;
                model.Comment = customer.Comment;
                model.AssignedBy = customer.Assigned;
                model.SocialLinks = customer.SocialLinks;
            }
            return model;
        }

        public void UpdateCustomer(CustomerEditModel customer, Guid userId)
        {
            if (customer.CustomerId == 0)
            {
                var customerGuid = customer.CompanyId != Guid.Empty ?
                    AddCustomerToBase(customer, userId)
                    : Guid.NewGuid();

                var model = new Customer()
                {
                    CustomerGuid = customerGuid,
                    FirstName = customer.FirstName,
                    Position = customer.PositionName,
                    IsLPR = customer.IsLPR,
                    LastName = customer.LastName,
                    Patronymic = customer.Patronymic,
                    Mail = customer.Mail,
                    StatusId = customer.StatusId,
                    PhotoPath = customer.PhotoPath,
                    Phone = customer.Phone,
                    SocialLinks = customer.SocialLinks,
                    Skype = customer.Skype,
                    Comment = customer.Comment,
                    CreatedBy = userId,
                    Created = DateTime.Now,
                    Assigned = customer.AssignedBy,
                    C_LegalEntityId = customer.CompanyId.HasValue ?
                        LocalContext.LegalEntities.FirstOrDefault(m => m.CompanyGuid == customer.CompanyId).LegalEntityId
                        : new Nullable<int>()
                };
              

                LocalContext.InsertUnit(model);
            }
            else
            {
                var model = LocalContext.GetUnitById<Customer>(customer.CustomerId);
                model.FirstName = customer.FirstName;
                model.Position = customer.PositionName;
                model.IsLPR = customer.IsLPR;
                model.LastName = customer.LastName;
                model.Patronymic = customer.Patronymic;
                model.Mail = customer.Mail;
                model.StatusId = customer.StatusId;
                model.Skype = customer.Skype;
                model.SocialLinks = customer.SocialLinks;
                model.Phone = customer.Phone;
                model.Comment = customer.Comment;
                model.Assigned = customer.AssignedBy;
               
                model.PhotoPath = customer.PhotoPath;
                LocalContext.SaveChanges();
            }
        }

        public void UpdatePassportCustomer(PassportViewModel model, Guid userId)
        {
            if (model.PassportId != 0)
            {
                var passport = LocalContext.Passports.FirstOrDefault(m => m.PassportId == model.PassportId);
                passport.Serial = model.Serial;
                passport.Number = model.Number;
                passport.RegLocation = model.RegLocation;
                passport.ScanPath = string.Join(",", model.ScanPath);
                passport.WhoIssue = model.WhoIssue;
                passport.DateIssue = Convert.ToDateTime(model.DateIssue, ruDateFormat);
                passport.CodeIssue = model.CodeIssue;
                passport.BirthLocation = model.BirthLocation;
                if (model.City != 0)
                {
                    var cityId = BaseContext.Cities.FirstOrDefault(m => m.CityId == model.City).CityGuid;
                    passport.CityGuid = cityId;
                    passport.Customers.FirstOrDefault().CityGuid = cityId;
                }
                Context.SaveChanges();
            }
            else
            {
                Passport passport = new Passport()
                {
                    Serial = model.Serial,
                    Number = model.Number,
                    RegLocation = model.RegLocation,
                    ScanPath = string.Join(",", model.ScanPath),
                    WhoIssue = model.WhoIssue,
                    DateIssue = Convert.ToDateTime( model.DateIssue, ruDateFormat),
                    CodeIssue = model.CodeIssue,
                    BirthLocation = model.BirthLocation
                };
                
                LocalContext.Passports.Add(passport);
                LocalContext.SaveChanges();
                LocalContext.Customers.FirstOrDefault(m => m.CustomerId == model.EmployeeId).C_PassportId = passport.PassportId;
                LocalContext.SaveChanges();
                if (model.City != 0)
                {
                    var cityId = BaseContext.Cities.FirstOrDefault(m => m.CityId == model.City).CityGuid;
                    passport.CityGuid = cityId;
                    passport.Customers.FirstOrDefault().CityGuid = cityId;
                }
                LocalContext.SaveChanges();
            }
        }

        public PassportViewModel GetCustomerPassport(int? customerId)
        {

            var model = new PassportViewModel()
            {
                DateIssue = DateTime.Now.Date.ToString("dd.MM.yyyy")
            };
            if (customerId.HasValue)
            {
                var passportId = LocalContext.Customers.FirstOrDefault(m => m.CustomerId == customerId.Value).C_PassportId;
                if (passportId.HasValue)
                {
                    var passport = LocalContext.Passports.FirstOrDefault(m => m.PassportId == passportId.Value);
                    model.PassportId = passport.PassportId;
                    model.Serial = passport.Serial;
                    model.CodeIssue = passport.CodeIssue;
                    model.DateIssue = passport.DateIssue.ToString("dd.MM.yyyy");
                    model.Number = passport.Number;
                    model.WhoIssue = passport.WhoIssue;
                    model.ScanPath = passport.ScanPath.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    model.BirthLocation = passport.BirthLocation;
                    model.RegLocation = passport.RegLocation;
                    model.EmployeeId = customerId.Value;
                    model.CityGuid = passport.CityGuid;
                    if (passport.CityGuid.HasValue && model.CityGuid != Guid.Empty)
                    {
                        var cityId = BaseContext.Cities.FirstOrDefault(m => m.CityGuid == model.CityGuid).CityId;
                        model.City = cityId;
                    }
                }
            }
            return model;
        }

        public PassportViewModel GetCustomerPassport(Guid? customerId)
        {
            var model = new PassportViewModel()
            {
                DateIssue = DateTime.Now.Date.ToString("dd.MM.yyyy"),
            };
            if (customerId.HasValue)
            {
                var customer = LocalContext.Customers.FirstOrDefault(m => m.CustomerGuid == customerId.Value);
                var passportId = customer.C_PassportId;
                    model.EmployeeId = customer.CustomerId;
                if (passportId.HasValue)
                {
                    var passport = LocalContext.Passports.FirstOrDefault(m => m.PassportId == passportId.Value);
                    model.PassportId = passport.PassportId;
                    model.Serial = passport.Serial;
                    model.CodeIssue = passport.CodeIssue;
                    model.DateIssue = passport.DateIssue.ToString("dd.MM.yyyy");
                    model.Number = passport.Number;
                    model.WhoIssue = passport.WhoIssue;
                    model.ScanPath = passport.ScanPath.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    model.BirthLocation = passport.BirthLocation;
                    model.RegLocation = passport.RegLocation;
                    model.CityGuid = passport.CityGuid;
                    if (passport.CityGuid.HasValue && model.CityGuid != Guid.Empty)
                    {
                        var cityId = BaseContext.Cities.FirstOrDefault(m => m.CityGuid == model.CityGuid).CityId;
                        model.City = cityId;
                    }
                }
            }
            return model;
        }
    }
}
