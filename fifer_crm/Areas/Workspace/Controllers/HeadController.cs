using AccessRepositories;
using CompanyRepositories;
using ContentSearchService;
using CRMLocalContext;
using CRMModel;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using FilterModel;
using LogService.FilterAttibute;
using SupportContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TicketRepositories;

namespace fifer_crm.Areas.Workspace.Controllers
{
    public class HeadController : BaseFiferController
    {
        [Authorize, CRMLogAttribute]
        // GET: Workspace/Head
        [DisplayName("Главаня страница клиентской базы")]
        public ActionResult Index()
        {
            CRMWrapViewModel model = new CRMWrapViewModel(_userId);
            ViewBag.Profile = model.UserPhoto;
            StaffRepository staffRepository = new StaffRepository();
            CustomerSearch searchService = new CustomerSearch(_userId);
            var search = searchService.SearchLegalEntites(new CustomerSearchFilter()
            {
                EmployeesPhoto = staffRepository.GetEmployeesPhoto(model.Company.CompanyId),
                AssignedBy = new List<Guid>() { _userId }
            });
            search.SearchResult = model.CRMCompanies;
            search = searchService.SearchCRMLegal(search);

            ViewBag.Search = search; model.Menu = GetMenu("Клиенты");
            return View(model);
        }

        [DisplayName("Получение списка клиентов")]
        public ActionResult GetCustomers(CustomerSearchFilter search)
        {
            CompanyRepository repository = new CompanyRepository();
            var company = repository.GetShortCompanyInfoByUserId(_userId);
            StaffRepository staffRepository = new StaffRepository();
            CustomerSearch searchService = new CustomerSearch(_userId);
            search.EmployeesPhoto = staffRepository.GetEmployeesPhoto(company.CompanyId);
           
            MembershipRepository membershipRepository = new MembershipRepository();
              
            if (search.IsLegal)
            {
                search = searchService.SearchLegalEntites(search);
                var ids = new List<Guid>(search.SearchResult.Cast<CRMCompanyViewModel>().Where(m => m.AssignedBy.HasValue).Select(m => m.AssignedBy.Value).Distinct());
                var names = membershipRepository.GetUserByIds(ids.Distinct());
                foreach (var item in search.SearchResult.Cast<CRMCompanyViewModel>())
                {
                    if (item.AssignedBy.HasValue)
                    {
                        var owner = names.FirstOrDefault(m => m.UserId == item.AssignedBy.Value);
                        item.AssignedName = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
                    }
                }
                search = searchService.SearchCRMLegal(search);
                return PartialView("CustomersList", search);
            }
            else
            {
                TaskTicketRepository taskRepository = new TaskTicketRepository();
                CRMLocalRepository localRepository = new CRMLocalRepository(_userId);
                
                search = searchService.SearchNotLegalEntites(search);
                var ids = new List<Guid>(search.SearchResult.Cast<CustomerViewModel>().Where(m => m.AssignedBy.HasValue).Select(m => m.AssignedBy.Value).Distinct());
                var names = membershipRepository.GetUserByIds(ids.Distinct());
                IEnumerable<CallTicket> tasks = null;
                IEnumerable<Meeting> meetings = null;
                
                foreach (CustomerViewModel item in search.SearchResult.Cast<CustomerViewModel>())
                {

                    tasks = taskRepository.GetCallTasksByCustomer(item.Guid);
                    if (tasks.Count() > 0)
                    {
                        item.CallId = tasks.OrderBy(m => m.ExpiredDate).FirstOrDefault().CallTicketId;
                        item.CallDate = tasks.Min(m => m.DateStarted);
                    }
                    
                     meetings = localRepository.GetMeetingsByCustomer(item.CustomerId);
                    if (meetings.Count() > 0)
                    {
                        item.MeetingId = meetings.OrderBy(m => m.Date).FirstOrDefault().MeetingGuid;
                        item.MeetDate = meetings.Min(m => m.Date);
                    }
                   
                    if (item.AssignedBy.HasValue)
                    {
                        var owner = names.FirstOrDefault(m => m.UserId == item.AssignedBy.Value);
                        item.AssignedName = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
                    }
                }
                if (search.HasCallTask || search.HasMeeting)
                {
                    var result = new List<CustomerViewModel>();

                    if (search.HasCallTask && tasks != null && tasks.Where(m => m.DateStarted >= search.DateRange[0] && m.DateStarted <= search.DateRange[1]).Count() > 0)
                        result.AddRange(search.SearchResult.Cast<CustomerViewModel>().Where(m => m.CallId.HasValue));
                    if (search.HasMeeting && meetings !=null && meetings.Where(m => m.Date >= search.DateRange[0] && m.Date <= search.DateRange[1]).Count() > 0)
                        result= result
                            .Union(search.SearchResult.Cast<CustomerViewModel>()
                            .Where(m => m.MeetingId.HasValue))
                            .ToList() ;
                    search.SearchResult = result;
                }
                return PartialView("CustomersNotLegalList", search);
            }
        }
    }
}