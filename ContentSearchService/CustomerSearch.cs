using CompanyRepositories;
using CRMContext;
using CRMRepositories;
using EnumHelper.CRM;
using FilterModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EnumHelper;
using AccessRepositories;
using CRMModel;
using TicketRepositories;
using FinanceRepositories;

namespace ContentSearchService
{
    public class CustomerSearch
    {
        private Guid _userId;
        private City city;

        private CRMLocalRepository crmLocalRepository;
        private CompanyRepository companyRepository = new CompanyRepository();
        private CRMRepository crmRepository = new CRMRepository();
        
        public CustomerSearch(Guid userId)
        {
            // TODO: Complete member initialization
            _userId = userId;
            var company = companyRepository.GetShortCompanyInfoByUserId(_userId);
             city = crmRepository.GetCompanyCity(company.Guid);
             crmLocalRepository = new CRMLocalRepository(userId);
        }

        public LegalEntitySearch GetDefaultLegalSearch(out Guid cityGuid)
        {
            var search = new LegalEntitySearch();
            search.DistrictId = city.C_DistrictId.HasValue ? city.C_DistrictId.Value : 0;
            search.City = new List<int>() { city.CityId };
            cityGuid = city.CityGuid;
            return search;
        }

        public CustomerSearchFilter SearchLegalEntites(CustomerSearchFilter search)
        {
            var companies =crmLocalRepository.LocalContext.LegalEntities
                .Select(m=>new { name =m.LegalName, id = m.LegalEntityId, status = m.StatusId, assign = m.Assigned})
                .ToList();

            var result = crmLocalRepository.GetCompanies();
            if (search.StatusId.Count>0)
                result = result.Where(m => search.StatusId.Contains(m.StatusId)).ToList();
           foreach(CompanyStatus status in Enum.GetValues(typeof(CompanyStatus)))
           {
               if(companies.Any(m=>m.status == (byte)status))
               {
                   search.AvailableStatuses.Add(new SelectListItem() { 
                       Text = status.GetStringValue(),
                       Value = ((byte)status).ToString(),
                       Selected = search.StatusId.Contains((byte)status) });
               }
           }
           
            if(search.Services.Count > 0)
                result = result.Where(m => m.Activities !=null && m.Activities.Intersect(search.Services).Count() > 0).ToList();

           search.AvailableServices = crmLocalRepository.LocalContext.CompanyServices
               .Where(m => m.C_ParentId.HasValue && m.LegalActivities.Count > 0)
               .ToList()
               .GroupBy(m => m.CompanyService2.Name)
               .ToDictionary(m => m.Key,
                           m => m.Select(n => new SelectListItem()
                           {
                               Text = n.Name,
                               Value = n.CompanyServiceId.ToString(),
                               Selected = search.Services.Contains(n.CompanyServiceId)
                           }).ToList()
                       );
           if (search.Name.Count > 0)
           {
               result = result.Where(m => search.Name.Contains(m.LegalEntityId)).ToList();
           }
           else if (search.Cities.Count > 0)
               result = result.Where(m => search.Cities.Contains(m.CityGuid)).ToList();
         
           if (!string.IsNullOrEmpty(search.Mail))
               result = result.Where(m => m.Mails.Split(',').Any(n => n.IndexOf(search.Mail) != -1)).ToList();
           else if (!string.IsNullOrEmpty(search.Site))
               result = result.Where(m => m.Sites.Split(',').Any(n=>n.IndexOf(search.Site) != -1)).ToList();
           else if (!string.IsNullOrEmpty(search.Phone))
           {
               var clearPhone = search.Phone
                   .Replace("+7", string.Empty)
                   .Replace("(", string.Empty)
                   .Replace(")", string.Empty)
                   .Replace(" ", string.Empty)
                   .Replace("-", string.Empty);

               result = result.Where(m => m.Phones.Split(',').Any(n => n.IndexOf(search.Phone) != -1) || m.Phones.Split(',').Any(n => n.IndexOf(clearPhone) != -1)).ToList();
           }

           search.AvailableNames = companies
              .Select(m => new SelectListItem() { Text = m.name, Value = m.id.ToString(), Selected = search.Name.Contains(m.id) })
              .ToList();

            var cities = crmLocalRepository.LocalContext.GeoLocations
                .Where(m=>m.LegalEntities.Count > 0)
                .Select(m=>m.CityGuid)
                .Distinct()
                .ToList();
            search.AvailableCities = crmLocalRepository.BaseContext.Cities.Where(m => cities.Contains(m.CityGuid))
                .Select(m => new { id = m.CityGuid, name = m.Name, districtName = m.District.Name })
                .ToList()
                .GroupBy(m => m.districtName)
                .ToDictionary(m => m.Key, m => m.Select(n => new SelectListItem() {
                    Value = n.id.ToString(), 
                    Text = n.name, 
                    Selected = search.Cities.Contains(n.id) }).ToList());
            
            if(search.AssignedBy.Count  == 0)
                result = result.Where(m => !m.AssignedBy.HasValue).ToList();
            else if (!search.AssignedBy.Contains(Guid.Empty))
                result = result.Where(m =>m.AssignedBy.HasValue &&  search.AssignedBy.Contains(m.AssignedBy.Value)).ToList(); 
            
            MembershipRepository accessRepository = new MembershipRepository();
            var assigned = companies.Where(m=>m.assign.HasValue)
                .Select(m=>m.assign.Value)
                .Distinct()
                .ToList();
            search.AssignedAvailable =  accessRepository.GetUserByIds(assigned)
                .Where(m=>m.UserId != Guid.Empty)
                .Select(m=> new SelectListItem(){ Text = m.FirstName + " " + m.LastName, Value = m.UserId.ToString()})
                .ToList();
            search.AssignedAvailable.Insert(0, new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Все сотрудники" });
            if (search.AssignedBy.Where(m=>m != Guid.Empty).Count() > 0)
                foreach (var item in search.AssignedAvailable.Where(m => !string.IsNullOrEmpty(m.Value) && search.AssignedBy.Contains(Guid.Parse(m.Value))))
                {
                    item.Selected = true;
                }
            else if (search.AssignedBy.Count() > 0 || !search.AssignedAvailable.Any(m=>m.Value == _userId.ToString()))
                search.AssignedAvailable.FirstOrDefault(m => m.Value == Guid.Empty.ToString()).Selected = true;
            else 
                search.AssignedAvailable.FirstOrDefault(m => m.Value == null).Selected = true;
            search.SearchResult  = result;
            return SearchCRMLegal(search);
        }

        public CustomerSearchFilter SearchNotLegalEntites(CustomerSearchFilter search)
        {
            var customers = crmLocalRepository.LocalContext.Customers.Where(m=>!m.C_LegalEntityId.HasValue)
                .Select(m => new { name = m.FirstName + " " + m.LastName + " " + m.Patronymic, id = m.CustomerId, status = m.StatusId, assign = m.Assigned })
                .ToList();
            var result = crmLocalRepository.GetCustomers(null);
            if (search.StatusId.Count > 0)
                result = result.Where(m => search.StatusId.Contains(m.StatusId)).ToList();
            foreach (CustomerStatus status in Enum.GetValues(typeof(CustomerStatus)))
            {
                if (customers.Any(m => m.status == (byte)status))
                {
                    search.AvailableStatuses.Add(new SelectListItem() {
                        Text = status.GetStringValue(), 
                        Value = ((byte)status).ToString(),
                       Selected = search.StatusId.Contains((byte)status) });
                }
            }

            if (!string.IsNullOrEmpty(search.Mail))
                result = result.Where(m => m.Mail.IndexOf(search.Mail) != - 1).ToList();

            if (!string.IsNullOrEmpty(search.Phone))
                result = result.Where(m => m.Phone == search.Phone).ToList();

            if (search.Name.Count > 0)
            {
                result = result.Where(m => search.Name.Contains(m.CustomerId)).ToList();
            }
            search.AvailableNames = customers
                .Select(m => new SelectListItem() { 
                    Text = m.name, 
                    Value = m.id.ToString(),
                    Selected = search.Name.Contains(m.id) })
                .ToList();
            if (!search.HasUnssigned)
                result = result.Where(m => m.AssignedBy.HasValue).ToList();
            if (!search.AssignedBy.Contains(Guid.Empty))
                result = result.Where(m => m.AssignedBy.HasValue && search.AssignedBy.Contains(m.AssignedBy.Value)).ToList();
            
            if (search.Cities.Count > 0)
                result = result.Where(m => search.Cities.Contains(m.CityGuid)).ToList();
         
            var cities = crmLocalRepository.LocalContext.Passports.Where(m=>m.CityGuid.HasValue)
                .Select(m=>m.CityGuid.Value)
                .Distinct()
                .ToList();
            search.AvailableCities = crmLocalRepository.BaseContext.Cities.Where(m => cities.Contains(m.CityGuid))
                .Select(m => new { id = m.CityGuid, name = m.Name, districtName = m.District.Name })
                .ToList()
                .GroupBy(m => m.districtName)
                .ToDictionary(m => m.Key, m => m.Select(n => new SelectListItem()
                {
                    Value = n.id.ToString(),
                    Text = n.name,
                    Selected = search.Cities.Contains(n.id)
                }).ToList());
            

            MembershipRepository accessRepository = new MembershipRepository();
            var assigned = customers.Where(m => m.assign.HasValue).Select(m => m.assign.Value).Distinct().ToList();
            search.AssignedAvailable = accessRepository.GetUserByIds(assigned)
                .Select(m => new SelectListItem() { Text = m.FirstName + " " + m.LastName, Value = m.UserId.ToString() })
                .ToList();
            search.AssignedAvailable.Insert(0, new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Все сотрудники" });
            if (search.AssignedBy.Where(m => m != Guid.Empty).Count() > 0)
                foreach (var item in search.AssignedAvailable.Where(m => !string.IsNullOrEmpty(m.Value) && search.AssignedBy.Contains(Guid.Parse(m.Value))))
                {
                    item.Selected = true;
                }
            else if (search.AssignedBy.Count() > 0 || !search.AssignedAvailable.Any(m=>m.Value == _userId.ToString()))
                search.AssignedAvailable.FirstOrDefault(m => m.Value == Guid.Empty.ToString()).Selected = true;
            else
                search.AssignedAvailable.FirstOrDefault(m => m.Value == _userId.ToString()).Selected = true;
            search.SearchResult = result;
            return search;
        }

        public CustomerSearchFilter SearchCRMLegal(CustomerSearchFilter search)
        {
            var existResult = search.SearchResult.Cast<CRMCompanyViewModel>();
            TaskTicketRepository taskRepository = new TaskTicketRepository();
            CRMLocalRepository localRepository = new CRMLocalRepository(_userId);
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);

            var customers = localRepository.GetCustomerIds();

            var filtredItems = new List<CRMCompanyViewModel>();
            foreach (var item in existResult)
            {
                if (customers.ContainsKey(item.CompanyGuid))
                {
                    var tasks = taskRepository.GetCallTasksPreview(customers[item.CompanyGuid]).Where(m => m.Date >= DateTime.Now.Date).ToList();
                    var meetings = localRepository.GetMeetingsByCustomerIds(customers[item.CompanyGuid]).Where(m => m.Date >= DateTime.Now.Date).ToList();
                    var payments = repository.GetPaymentsByIds(new List<int>(), item.CompanyGuid, false).Where(m => m.PayBefore >= DateTime.Now.Date).ToList();
                    if (tasks.Count() > 0)
                    {
                        item.CallDate = tasks.Min(m => m.Date);
                    }
                    if (meetings.Count() > 0)
                    {
                        item.MeetDate = meetings.Min(m => m.Date);
                    }
                    if (payments.Count() > 0)
                    {
                        item.PaymentDate = payments.Min(m => m.PayBefore);
                    }
                    if ((search.HasMeeting && meetings.Where(m => m.Date >= search.DateRange[0] && m.Date <= search.DateRange[1]).Count() > 0)
                         || (search.HasCallTask && tasks.Where(m => m.Date >= search.DateRange[0] && m.Date <= search.DateRange[1]).Count() > 0)
                         || (search.HasPayment && payments.Where(m => m.PayBefore >= search.DateRange[0] && m.PayBefore <= search.DateRange[1]).Count() > 0)
                         || (!search.HasMeeting && !search.HasPayment && !search.HasCallTask))
                        filtredItems.Add(item);
                }
            }
            search.SearchResult = filtredItems;
            return search;
        }
    }
}
