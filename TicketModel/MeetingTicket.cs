using CRMContext;
using CRMLocalContext;
using EnumHelper.CRM;
using Interfaces.CRM;
using Interfaces.Support;
using LogRepositories;
using NotifyModel;
using NotifyService.EventHandlers;
using Settings;
using SupportContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketModel
{
    [MetadataType(typeof(IMeetingTicket))]
    [Serializable]
    public class MeetingTicket : IMeetingTicket, ITicket
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;
        
        private object lockObj = new object();

        public int MeetingId { get; set; }
        public Guid CustomerId { get; set; }
        public string Goals { get; set; }
        public string Result { get; set; }
        public string Calls { get; set; }

        public Guid TicketId { get; set; }

        public Guid? MasterId { get; set; }

        public string Title { get; set; }

        public Guid TicketOwnerId { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateStarted { get; set; }
        [Required]
        public string DateStartedStr { get; set; }

        [Required]
        public string TimeStartedStr { get; set; }

        public DateTime? TimeToExpired { get; set; }
        public string NotifyBeforeStr { get; set; }
        public DateTime? NotifyDate
        {
            get
            {
                return DateStarted.HasValue ?
                    DateStarted.Value.Add(-Convert.ToDateTime(NotifyBeforeStr).TimeOfDay)
                    : new Nullable<DateTime>();
            }
        }

        public string TicketStatusStr { get; set; }
        [Required]
        public string Msg { get; set; }

        public byte? TicketStatus { get; set; }

        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string CurrentComment { get; set; }

        public Guid CurrentCommentId { get; set; }

        public string AssignedName { get; set; }

        public Guid? Assigned { get; set; }

        public DateTime? DateAssigned { get; set; }

        public DateTime DateModified { get; set; }

        public byte Priority { get; set; }
        public string Phone { get; set; }

        public string TicketNumber { get; set; }
        public Guid CreateMaster(string masterType, Guid wfGuid, Guid? masterId)
        {
            throw new NotImplementedException();
        }

        public Guid? ObjId { get; set; }

        public object EnvelopObj { get; set; }

        public IDictionary<string, IEnumerable<Guid>> CommonActionTickets { get; set; }

        public void Create()
        {
            using (LocalCRMEntities context = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites))
            {
                lock (lockObj)
                {
                    var ticket = new Meeting()
                    {
                        C_CustomerId = context.Customers.FirstOrDefault(m => m.CustomerGuid == CustomerId).CustomerId,
                        Calls = Calls,
                        CreatedBy = OwnerId,
                        Created = DateTime.UtcNow,
                        Comment = Msg,
                        Date = DateStarted.Value,
                        Goals = Goals,
                        MeetingGuid = Guid.NewGuid(),
                        ResultComment = string.Empty,
                        StatusId = TicketStatus.Value,
                    };
                    context.Meetings.Add(ticket);
                    context.SaveChanges();
                    TicketId = ticket.MeetingGuid;

                }
            }
        }

        public void Load(Guid guid)
        {
            using (LocalCRMEntities context = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites))
            {
                var meeting = context.Meetings.FirstOrDefault(m => m.MeetingGuid == TicketId);
                CustomerId = meeting.Customer.CustomerGuid;
                Calls = meeting.Calls;
                Assigned = meeting.Assigned;
                CreatedBy = meeting.CreatedBy;
                DateCreated = meeting.Created;
                Msg = meeting.Comment;
                DateStarted = meeting.Date;
                Goals = meeting.Goals;
                TicketId = meeting.MeetingGuid;
                Result = meeting.ResultComment;
                TicketStatus = meeting.StatusId;
            }
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(DateStartedStr))
                DateStarted = Convert.ToDateTime(DateStartedStr, ruDateFormat).Add(Convert.ToDateTime(TimeStartedStr).TimeOfDay);

            if (TicketId == Guid.Empty)
                Create();
            else
            {

                using (LocalCRMEntities context = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites))
                {
                    var meeting = context.Meetings.FirstOrDefault(m => m.MeetingGuid == TicketId);
                    meeting.Date = DateStarted.HasValue ? DateStarted.Value : DateTime.Now;
                    meeting.Goals = Goals;
                    meeting.Assigned = Assigned.Value;
                    meeting.ResultComment = Result;
                    meeting.StatusId = TicketStatus.Value;
                    context.SaveChanges();
                }
            }
        }

        public void AutoClose()
        {
            throw new NotImplementedException();
        }

        public void LoadTicketByObjId(Guid objID)
        {
            throw new NotImplementedException();
        }

        public Guid ChangeStatus(int statusId, string customStatus = "")
        {
            Guid result = new Guid();
            NotifyRepository repository = new NotifyRepository();

            using (LocalCRMEntities context = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites))
            {
                var meeting = context.Meetings.FirstOrDefault(m => m.MeetingGuid == TicketId);
                
                switch ((WFMeetingStatus)statusId)
                {
                    case WFMeetingStatus.EndCall:
                    case WFMeetingStatus.EndMeet:
                    case WFMeetingStatus.EndPayment:
                        TicketEventsObserver.Instance.EventFired(GetNotifyItem(1, meeting));
                        meeting.StatusId = (byte)EnumHelper.TaskStatus.Completed;                        
                        break;
                    case WFMeetingStatus.AssignMeet:
                        meeting.StatusId = (byte)EnumHelper.TaskStatus.Updated;                        
                        break;
                    case WFMeetingStatus.Novelty:
                        meeting.StatusId = (byte)EnumHelper.TaskStatus.Novelty;
                        break;
                    case WFMeetingStatus.TransferMeet:
                        meeting.StatusId = (byte)EnumHelper.TaskStatus.Updated;
                        break;
                    default:
                        meeting.StatusId = (byte)EnumHelper.TaskStatus.Error;
                        break;
                };

                var queryStatus = new QueryStatu()
                {
                    QueryItemId = this.TicketId,
                    OwnerId = this.OwnerId,
                    DateCreated = DateTime.UtcNow
                };

                if (string.IsNullOrEmpty(customStatus))
                    queryStatus.StatusId = statusId;
                else
                    queryStatus.CustomStatus = customStatus;
                using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    wfContext.QueryStatus.Add(queryStatus);

                    try
                    {
                        wfContext.SaveChanges();
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        Trace.WriteLine(string.Join("\r\n", dbEx.EntityValidationErrors.SelectMany(m => m.ValidationErrors.Select(n => n.ErrorMessage))));
                    }
                    finally
                    {
                        result = TicketId;
                    }
                }
            }
            return result;
        }

        private NotifyObserveItem GetNotifyItem(int notifyId, Meeting ticket)
        {
            return new NotifyObserveItem()
            {
                OwnerId = ticket.CreatedBy,
                FromUserId = OwnerId,
                ToUserId = ticket.Assigned,
                NotifyId = notifyId,
                AppendMsg = ticket.Goals,
                ObjectId = ticket.MeetingGuid.ToString()
            };
        }

        public void ChangePreview(bool value)
        {
            throw new NotImplementedException();
        }

        public string Status { get; set; }

        public bool IsNonePreview { get; set; }
        public Guid OwnerId { get; set; }

    }
}
