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
            var result = taskrepository.GetTaskTickets(assigned.Where(m=>m != Guid.Empty).Count() == 0 ? existAssign : assigned, search.HasUnssigned, search.Statuses);
            

            search.SearchResult = result.Where(m =>!m.DateStarted.HasValue ||
                (m.DateStarted >= search.DateRange[0] && m.DateStarted <= search.DateRange[1])).ToList();

            var periodTickets = crmLocalRepository.FindPeriodTickets(search.SearchResult.Select(m => m.TaskId));
            foreach (var item in search.SearchResult.Where(m => periodTickets.Contains(m.TaskId)))
            {
                item.HasPeriod = true;
            }
            var ids = search.SearchResult.Select(m => m.CreatedById)
                  .Distinct()
                  .ToList();
            ids.AddRange(search.SearchResult.Select(m => m.AssignedById));

            MembershipRepository membershipRepository = new MembershipRepository();
            var names = membershipRepository.GetUserByIds(ids);
            foreach (var item in search.SearchResult)
            {
                var createdId = names.FirstOrDefault(m => m.UserId == item.CreatedById);
                item.CreatedBy = createdId.FirstName + " " + createdId.LastName;

                var assignedId = names.FirstOrDefault(m => m.UserId == item.AssignedById);
                item.AssignedBy = assignedId.FirstName + " " + assignedId.LastName;
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
            search.AssignedAvailable.Insert(0, new SelectListItem() { Value = Guid.Empty.ToString(), Text = "Все" });
            if (assigned.Count() == 0)
                search.AssignedAvailable.FirstOrDefault(m => m.Value == Guid.Empty.ToString()).Selected = true;
            else
            {
                foreach (var item in search.AssignedAvailable.Where(m => !string.IsNullOrWhiteSpace(m.Value) && assigned.Contains(Guid.Parse(m.Value))))
                {
                    item.Selected = true;
                }
            }
            var result = taskrepository.GetJsonTaskTickets(assigned.Where(m=>m != Guid.Empty).Count() == 0 ? existAssign : assigned, search.DateRange[0], search.DateRange[1], search.HasUnssigned, search.Statuses);
            var periodTickets = crmLocalRepository.FindPeriodTickets(result.Select(m => m.TaskId));
            foreach (var item in result.Where(m => periodTickets.Contains(m.TaskId)))
            {
                item.HasPeriod = true;
            }

            return result.Select(m => JsonConvert.SerializeObject(m)).ToArray();
        }

    }
}
