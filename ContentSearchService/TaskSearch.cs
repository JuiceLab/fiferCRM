using AccessRepositories;
using CompanyRepositories;
using CRMRepositories;
using FilterModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TicketRepositories;

namespace ContentSearchService
{
    public class TaskSearch
    {
        private Guid _userId;

        private CRMLocalRepository crmLocalRepository;
        private CompanyRepository companyRepository = new CompanyRepository();
        private TaskTicketRepository taskrepository = new TaskTicketRepository();

        
        public TaskSearch(Guid userId)
        {
            // TODO: Complete member initialization
            _userId = userId;
            var company = companyRepository.GetShortCompanyInfoByUserId(_userId);
            crmLocalRepository = new CRMLocalRepository(userId);
        }

        public TaskSearchFilter SearchTasks(TaskSearchFilter search)
        {
            var assigned = new List<Guid>();

            if (search.Assigned != null && search.Assigned.Count > 0)
                assigned =
                    search.Assigned.SelectMany(m => m.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(m => Guid.Parse(m)).ToList();


            StaffRepository repostory = new StaffRepository();
            var existAssign = repostory.GetSubordinatedUsers(_userId).Select(m => Guid.Parse(m.Value));
            MembershipRepository accessRepository = new MembershipRepository();

            search.AssignedAvailable = accessRepository.GetUserByIds(existAssign)
                .Where(m => m.UserId != Guid.Empty)
                .Select(m => new SelectListItem() { Text = m.FirstName + " " + m.LastName, Value = m.UserId.ToString() })
                .ToList();
            search.AssignedAvailable.Insert(0, new SelectListItem() { Text = "Не закреплены за сотрудинками" });
            search.AssignedAvailable.Insert(0, new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Все" });
            if (assigned.Count() == 0)
                search.AssignedAvailable.FirstOrDefault(m => m.Value == Guid.Empty.ToString()).Selected = true;
            else
            {
                foreach (var item in search.AssignedAvailable.Where(m =>!string.IsNullOrWhiteSpace(m.Value)&& assigned.Contains(Guid.Parse(m.Value))))
                {
                    item.Selected = true;
                }
            }
            var result = taskrepository.GetTaskTickets(assigned.Count == 0 ? existAssign : assigned);


            search.SearchResult = result.Where(m => m.DateStarted >= search.DateRange[0] && m.DateStarted <= search.DateRange[1]).ToList();

            var periodTickets = crmLocalRepository.FindPeriodTickets(search.SearchResult.Select(m => m.TaskId));
            foreach (var item in search.SearchResult.Where(m => periodTickets.Contains(m.TaskId)))
            {
                item.HasPeriod = true;
            }
            return search;
        }

        public string[] SearchJsonTasks(TaskSearchFilter search)
        {
            var assigned = new List<Guid>();

            if (search.Assigned != null && search.Assigned.Count > 0)
                assigned =
                    search.Assigned.SelectMany(m => m.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    .Select(m => Guid.Parse(m)).ToList();


            StaffRepository repostory = new StaffRepository();
            var existAssign = repostory.GetSubordinatedUsers(_userId).Select(m => Guid.Parse(m.Value));
            MembershipRepository accessRepository = new MembershipRepository();

            search.AssignedAvailable = accessRepository.GetUserByIds(existAssign)
                .Where(m => m.UserId != Guid.Empty)
                .Select(m => new SelectListItem() { Text = m.FirstName + " " + m.LastName, Value = m.UserId.ToString() })
                .ToList();
            search.AssignedAvailable.Insert(0, new SelectListItem() { Text = "Не закреплены за сотрудинками" });
            search.AssignedAvailable.Insert(0, new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Все" });
            if (assigned.Count() == 0)
                search.AssignedAvailable.FirstOrDefault(m => m.Value == Guid.Empty.ToString()).Selected = true;
            else
            {
                foreach (var item in search.AssignedAvailable.Where(m => assigned.Contains(Guid.Parse(m.Value))))
                {
                    item.Selected = true;
                }
            }
            var items = taskrepository.GetJsonTaskTickets(assigned.Count == 0 ? existAssign : assigned, search.DateRange[0], search.DateRange[1]);
            var periodTickets = crmLocalRepository.FindPeriodTickets(items.Select(m => m.TaskId));
            foreach (var item in items.Where(m => periodTickets.Contains(m.TaskId)))
            {
                item.HasPeriod = true;
            }
            return items.Select(m=>JsonConvert.SerializeObject(m)).ToArray();
        }

    }
}
