using AccessRepositories;
using CompanyContext;
using CompanyModel;
using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskModel;
using TaskModel.CRM;
using TaskModel.Task;
using TicketRepositories;

namespace fifer_crm.Models
{
    public class EmployeeWrapViewModel : ICompanyInfo
    {
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }
        public CompanyViewModel Company { get; set; }

        public IEnumerable<EmployeeViewModel> CompanyEmployees { get; set; }

        public IEnumerable<MeetingTaskPreview> Meetings { get; set; }

        public IEnumerable<TaskPreview> TaskTickets { get; set; }
        public IEnumerable<CallTaskPreview> CallTasks { get; set; }
        public Dictionary<DateTime, IEnumerable<CRMEventPreview> > SheduledEvents { get; set; }

        public EmployeeWrapViewModel()
        {
            UserId = Guid.Empty;
        }

        public EmployeeWrapViewModel(Guid userId)
        {
            UserId = userId;
            CompanyRepository repository = new CompanyRepository();
            Company = repository.GetShortCompanyInfoByUserId(userId);

            StaffRepository staffRepository = new StaffRepository();
            CompanyEmployees = staffRepository.GetEmployees(userId).ToList();

            var users = staffRepository.GetSubordinatedUsers(userId);
            TaskTicketRepository _repository = new TaskTicketRepository();
            TaskTickets = _repository.GetTaskTickets(users.Select(m => Guid.Parse(m.Value)));

            CRMLocalRepository crmRepository = new CRMLocalRepository(userId);
            CallTasks = _repository.GetCallTasksPreview(null, userId);
            crmRepository.UpdateCustomerInfo(CallTasks);
            Meetings = crmRepository.GetMeetings(userId);
            SheduledEvents = _repository.SheduledEvents(users.Select(m => Guid.Parse(m.Value)));
            crmRepository.UpdateSheduledEvents(SheduledEvents, users.Select(m => Guid.Parse(m.Value)));

            var ids = TaskTickets.Select(m => m.CreatedById)
                .Distinct()
                .ToList();
            ids.AddRange(TaskTickets.Select(m => m.AssignedById));
            ids.AddRange(SheduledEvents.SelectMany(m => m.Value.Select(n => n.OwnerId)).Distinct());
            ids.AddRange(CallTasks.Where(m => m.AssignId.HasValue).Select(m => m.AssignId.Value).Distinct());
            ids.AddRange(Meetings.Select(m => m.OwnerId).Distinct());
            
            foreach (var item in Meetings)
            {
                var owner = CompanyEmployees.FirstOrDefault(m => m.UserId == item.OwnerId);
                item.OwnerName = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
            }

            foreach (var item in CallTasks)
            {
                if (item.AssignId.HasValue)
                {
                    var assign = CompanyEmployees.FirstOrDefault(m => m.UserId == item.AssignId.Value);
                    item.AssignName = assign != null ? string.Format("{0} {1}", assign.FirstName, assign.LastName) : string.Empty;
                }
            }

            foreach (var item in SheduledEvents.SelectMany(m=>m.Value))
            {
                var owner = CompanyEmployees.FirstOrDefault(m => m.UserId == item.OwnerId);
                item.OwnerName = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
            }

            foreach (var item in TaskTickets)
            {
                var createdId = CompanyEmployees.FirstOrDefault(m => m.UserId == item.CreatedById);
                item.CreatedBy = createdId.FirstName + " " + createdId.LastName;
                item.CreatedByPhotoPath = CompanyEmployees.FirstOrDefault(m => m.UserId == item.CreatedById).PhotoPath;
                var assignedId = CompanyEmployees.FirstOrDefault(m => m.UserId == item.AssignedById);
                item.AssignedBy = assignedId.FirstName + " " + assignedId.LastName;
            }
            CompanyEmployees = CompanyEmployees.Where(m => m.UserId != userId).ToList();
        }

        public int CompanyId { get { return Company.CompanyId; } set { Company.CompanyId = value; } }

        public string Logo { get { return Company.Logo; } set { Company.Logo = value; } }

        public string Name { get { return Company.PublicCompanyName; } set { Company.PublicCompanyName = value; } }
        public string UserPhoto { get { return Company.UserPhoto; } set { Company.UserPhoto = value; } }

    }
}
