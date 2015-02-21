using CRMModel;
using EnumHelper;
using EnumHelper.CRM;
using Newtonsoft.Json;
using SupportContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using TaskModel;
using TaskModel.CRM;
using TaskModel.Task;
using TaskModel.Ticket;
using EntityRepository;

namespace TicketRepositories
{
    public class TaskTicketRepository : BaseTicketRepository, IDisposable
    {
        public TaskTicketRepository()
            : base()
        { }

        public IEnumerable<TaskPreview> GetTaskTickets(IEnumerable<Guid> userIds, EnumHelper.TaskStatus? status = null)
        {
            if (!status.HasValue)
            {
                return Context.Tickets
                   .Where(m => m.TicketStatus != (byte)EnumHelper.TaskStatus.Completed
                       && m.Assigned.HasValue && userIds.Contains(m.Assigned.Value))
                   .ToList()
                   .Select(m => new TaskPreview()
                   {
                       Created = m.DateCreated,
                       CreatedById = m.CreatedBy,
                       Status = (EnumHelper.TaskStatus)m.TicketStatus.Value,
                       TaskNumber = m.TicketNumber,
                       TaskId = m.TicketId,
                       Title = m.Title,
                       Msg = m.Message,
                       IsHighProirity = m.Priory == _highPriority,
                       AssignedById = m.Assigned.Value,
                       DateStarted = m.DateStarted,
                       HasGroup = m.TicketGroups.Count > 0
                   }).ToList();
            }
            else
            {
                return Context.Tickets
                   .Where(m => m.TicketStatus == (byte)status.Value
                       && m.Assigned.HasValue && userIds.Contains(m.Assigned.Value))
                   .ToList()
                   .Select(m => new TaskPreview()
                   {
                       Created = m.DateCreated,
                       CreatedById = m.CreatedBy,
                       Status = (EnumHelper.TaskStatus)m.TicketStatus.Value,
                       TaskNumber = m.TicketNumber,
                       TaskId = m.TicketId,
                       IsHighProirity = m.Priory == _highPriority,
                       Title = m.Title,
                       Msg = m.Message,
                       AssignedById = m.Assigned.Value,
                       DateStarted = m.DateStarted,
                       HasGroup = m.TicketGroups.Count > 0
                   }).ToList();
            }
        }

        public JsonTaskPreview[] GetJsonTaskTickets(IEnumerable<Guid> userIds, DateTime from, DateTime to)
        {
            return Context.Tickets
               .Where(m => m.TicketStatus != (byte)EnumHelper.TaskStatus.Completed
                   && m.Assigned.HasValue && userIds.Contains(m.Assigned.Value) && m.DateStarted.HasValue && m.DateStarted >= from && m.DateStarted <=to)
               .ToList()
               .Select(m => new JsonTaskPreview()
               {
                   TaskNumber = m.TicketNumber,
                   TaskId = m.TicketId,
                   Title = m.Title,
                   DateStarted = m.DateStarted.Value.ToString("yyyy-MM-ddTHH:mm:ss"),
                   Description = m.Message,
                   HasGroup = m.TicketGroups.Count == 0
               }).ToArray();
        }

        public IEnumerable<MessageViewModel> GetMessages4Item(Guid taskId)
        {
            return Context.CommentBodies.Where(m => m.GuidItem == taskId)
               .Select(m => new MessageViewModel()
               {
                   Type = (byte)MsgType.Msg,
                   Created = m.DateCreated,
                   Msg = m.Comment,
                   UserId = m.UserId.Value
               }).ToList();
        }

        public IEnumerable<MessageViewModel> GetStatus4Task(Guid taskId)
        {
            return Context.QueryStatus.Where(m => m.QueryItemId == taskId)
                .ToList()
               .Select(m => new MessageViewModel()
               {
                   Type = (byte)MsgType.Status,
                   Created = m.DateCreated,
                   Msg = m.StatusId.HasValue ? ((WFTaskStatus)m.StatusId.Value).GetStringValue() : m.CustomStatus,
                   UserId =  m.OwnerId.HasValue?  m.OwnerId.Value : Guid.Empty
               }).ToList();
        }

        public IEnumerable<SelectListItem> GetCategories()
        {
            return Context.Categories
              .Where(m => m.TypeId == (byte)CategoryType.Task)
              .ToList()
              .Select(m => new SelectListItem()
              {
                  Text = m.Name,
                  Value = m.CategoryId.ToString()
              });
        }

        public IEnumerable<Guid> GetExpiredTasks()
        {
            var curDate = DateTime.Now.Date;
            return Context.Tickets
                .Where(m => m.TicketStatus != (byte)EnumHelper.TaskStatus.Completed
                    && m.TicketStatus != (byte)EnumHelper.TaskStatus.Expired
                    && m.CategoryId.Value == (byte)CategoryType.Task
                    && m.DateStarted.HasValue && m.DateStarted <= curDate)
                .Select(m => m.TicketId)
                .ToList();
        }

        public IEnumerable<CallTaskPreview> GetCallTasksPreview(IList<Guid> customerIds, Guid? assigned = null)
        {
            if (assigned.HasValue)
            {
                return Context.CallTickets
                 .Where(m => m.CallStatus != (byte)EnumHelper.TaskStatus.Completed && m.Assigned.HasValue && m.Assigned == assigned.Value)
                 .Select(m => new CallTaskPreview()
                 {
                     AssignId = m.Assigned,
                     OwnerId = m.CreatedBy,
                     CallTaskId = m.CallTicketId,
                     Date = m.DateStarted,
                     StatusId = m.CallStatus,
                     TaskNumber = m.CallTicketNumber,
                     Title = m.Title,
                     CustomerId = m.CustomerGuid.Value
                 }).ToList();
            }
            else
            {
                return Context.CallTickets
                    .Where(m => m.CustomerGuid.HasValue && customerIds.Contains(m.CustomerGuid.Value))
                    .Select(m => new CallTaskPreview()
                    {
                        AssignId = m.Assigned,
                        OwnerId = m.CreatedBy,
                        CallTaskId = m.CallTicketId,
                        Date = m.DateStarted,
                        StatusId = m.CallStatus,
                        TaskNumber = m.CallTicketNumber,
                        Title = m.Title,
                        CustomerId = m.CustomerGuid.Value
                    }).ToList();
            }
        }

        public Dictionary<DateTime, IEnumerable<CRMEventPreview>> SheduledEvents(IEnumerable<Guid> users, DateTime? date=null)
        {
            var dateOfEnd = date == null ? DateTime.Now.Date : date.Value;

            var items = Context.CallTickets
                 .Where(m => m.Assigned.HasValue && users.Contains(m.Assigned.Value) && m.CallStatus != (byte)EnumHelper.TaskStatus.Completed)
                 .Select(m => new CRMEventPreview()
                 {
                     TypeId = (byte)CRMEventType.TaskCall,
                     EventId = m.CallTicketId,
                     OwnerId = m.Assigned.Value,
                     StatusId = m.CallStatus,
                     EventDate = m.DateStarted,
                     ContactId = m.CustomerGuid
                 })
                 .ToList();
            
            
            items.AddRange(Context.Tickets
                .Where(m => m.CategoryId.Value == (byte)CategoryType.Task
                    && m.Assigned.HasValue
                    && users.Contains(m.Assigned.Value) && m.TicketStatus != (byte)EnumHelper.TaskStatus.Completed)
                    .ToList()
                .Select(m => new CRMEventPreview()
                {
                    TypeId = (byte)CRMEventType.Task,
                    EventId = m.TicketId,
                    OwnerId = m.Assigned.Value,
                    StatusId = m.TicketStatus.HasValue ? m.TicketStatus.Value : (byte)EnumHelper.TaskStatus.Novelty,
                    EventDate = m.DateStarted.HasValue ? m.DateStarted.Value : DateTime.Now.Date,
                    ContactId = null
                })
                .ToList());
            
            if (dateOfEnd != DateTime.Now.Date)
                items = items.Where(m => m.EventDate == dateOfEnd).ToList();

            var model = new Dictionary<DateTime, IEnumerable<CRMEventPreview>>();
            do
            {
                model.Add(dateOfEnd, items.Where(m => m.EventDate.Date == dateOfEnd.Date).ToList());
                dateOfEnd = dateOfEnd.AddDays(1);
            } while (dateOfEnd.Month == DateTime.Now.Month || dateOfEnd.DayOfWeek != DayOfWeek.Monday);
            return model;
        }

        public Guid? GetFirstCallTask(List<Guid> customers)
        {
            var task = Context.CallTickets.Where(m =>m.CustomerGuid.HasValue &&  customers.Contains(m.CustomerGuid.Value) && m.CallStatus != (byte)TicketStatus.Closed)
                .OrderBy(m => m.ExpiredDate)
                .FirstOrDefault();
            return task != null ? task.CallTicketId : new Nullable<Guid>();
        }

        public IEnumerable<CallTicket> GetCallTasksByCustomer(Guid customerId)
        {
           return Context.CallTickets.Where(m => m.CustomerGuid == customerId && m.CallStatus != (byte)TicketStatus.Closed);
        }

        public void UpdateGroup(TaskGroup taskGroup)
        {
            var existGroup = Context.GetUnitById<TicketGroup>(taskGroup.GroupTaskId);
            existGroup.AssignedDepartments = taskGroup.AssignedDepartments !=null?
                string.Join(",", taskGroup.AssignedDepartments)
            : string.Empty;
            existGroup.AssignedGroups = taskGroup.AssignedGroups !=null?
                string.Join(",", taskGroup.AssignedGroups)
                 : string.Empty;
            var existUsers = existGroup.AssignedUsers.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList();
            var curUsers = taskGroup.AssignedUsers;
            var existStatuse = existGroup.UserStatuses.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (curUsers.Count > existUsers.Count)
            {
                foreach (var item in curUsers.Except(existUsers))
                {
                    existUsers.Add(item);
                    existStatuse.Add(WFTaskStatus.Novelty.ToString("d"));
                }
                        existGroup.AssignedUsers = string.Join(",", existUsers);
                    existGroup.UserStatuses = string.Join(",", existStatuse);
            }
            else
            {
                var updatedUsers = new List<string>();
                var updatedStatuses = new List<string>();

                foreach (var item in curUsers)
                {
                    updatedUsers.Add(item);
                    if (existUsers.Contains(item))
                    {
                        updatedStatuses.Add(existStatuse[existUsers.IndexOf(item)]);
                    }
                    else
                    {
                        updatedStatuses.Add(WFTaskStatus.Novelty.ToString("d"));
                    }
                }
                existGroup.AssignedUsers = string.Join(",", updatedUsers);
                existGroup.UserStatuses = string.Join(",", updatedStatuses);
            }

    
            //todo change statuses
            Context.SaveChanges();
        }

     
    }
}
