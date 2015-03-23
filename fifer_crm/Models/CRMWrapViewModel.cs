using AccessRepositories;
using CompanyModel;
using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using FinanceRepositories;
using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskModel.CRM;
using TicketRepositories;

namespace fifer_crm.Models
{
    public class CRMWrapViewModel : ICompanyInfo
    {
        public int CustomerCount { get; set; }
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }
        public CompanyViewModel Company { get; set; }
        public IEnumerable<CRMCompanyViewModel> CRMCompanies { get; set; }
        public Dictionary<Guid, int> CompanyPayments { get; set; }
        public IEnumerable<CallTaskPreview> CallTasks { get; set; }
        public IEnumerable<MeetingTaskPreview> Meetings { get; set; }

        public CRMWrapViewModel()
        {
            UserId = Guid.Empty;
        }

        public CRMWrapViewModel(Guid userId, bool onlyTasks = false)
        {
            UserId = userId;
            CompanyRepository repository = new CompanyRepository();
            Company = repository.GetShortCompanyInfoByUserId(userId);
            
            CRMLocalRepository crmRepository = new CRMLocalRepository(userId);

            Meetings = crmRepository.GetMeetings();
            if (!onlyTasks)
            {
                CRMCompanies = crmRepository.GetCompanies();
            }
          
            Dictionary<Guid, List<Guid>> customerIds = crmRepository.GetCustomerIds();
            TaskTicketRepository taskRepository = new TaskTicketRepository();
            CallTasks = taskRepository.GetCallTasksPreview(customerIds.SelectMany(m=>m.Value).ToList());

            var ids = new List<Guid>();
            if (!onlyTasks)
            {
                ids.AddRange(CRMCompanies.Where(m => m.AssignedBy.HasValue).Select(m => m.AssignedBy.Value).Distinct());
            }
            ids.AddRange(Meetings.Select(m => m.OwnerId).Distinct());
            ids.AddRange(CallTasks.Where(m => m.AssignId.HasValue).Select(m => m.AssignId.Value).Distinct());
            MembershipRepository membershipRepository = new MembershipRepository();
            var names = membershipRepository.GetUserByIds(ids.Distinct());

            foreach (var item in CallTasks)
            {
                if (item.AssignId.HasValue)
                {
                    var assign = names.FirstOrDefault(m => m.UserId == item.AssignId.Value);
                    item.AssignName = assign != null ? string.Format("{0} {1}", assign.FirstName, assign.LastName) : string.Empty;
                }
            }

            foreach (var item in Meetings)
            {
                var owner = names.FirstOrDefault(m => m.UserId == item.OwnerId);
                item.OwnerName = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
            }

            if (!onlyTasks)
            {
                foreach (var item in CRMCompanies)
                {
                    if (item.AssignedBy.HasValue)
                    {
                        var owner = names.FirstOrDefault(m => m.UserId == item.AssignedBy.Value);
                        item.AssignedName = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
                    }
                }
            }
        }

        public int CompanyId { get { return Company.CompanyId; } set { Company.CompanyId = value; } }

        public string Logo { get { return Company.Logo; } set { Company.Logo = value; } }
        public string UserPhoto { get { return Company.UserPhoto; } set { Company.UserPhoto = value; } }

        public string Name { get { return Company.PublicCompanyName; } set { Company.PublicCompanyName = value; } }
    }
}
