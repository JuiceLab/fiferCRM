using CRMContext;
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
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketModel
{
    [MetadataType(typeof(ITicket))]
    [Serializable]
    public class CallTicket : ICallTicket
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;
        
        private object lockObj = new object();
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
        public Guid? PrevCallId { get; set; }

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
            Guid idDefault = new Guid();

            using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                lock (lockObj)
                {
                    var ticket = new SupportContext.CallTicket()
                    {
                        PrevCallId = PrevCallId,
                        Assigned = Assigned,
                        CallStatus = TicketStatus.HasValue ? TicketStatus.Value : (byte)EnumHelper.TaskStatus.Novelty,
                        CallTicketNumber = GenerateNumber(),
                        CreatedBy = CreatedBy,
                        CustomerGuid = ObjId.Value,
                        DateStarted = DateStarted.Value,
                        Message = Msg,
                        Title = Title,
                        Phone = string.IsNullOrEmpty(Phone) ? "Задан" : Phone,
                    };
                    context.CallTickets.Add(ticket);
                    context.SaveChanges();
                    TicketId = ticket.CallTicketId;

                    TicketsQuery tq = new TicketsQuery()
                    {
                        TicketQueryId = Guid.NewGuid(),
                        TicketId = ticket.CallTicketId,
                        IsNonePreview = true
                    };

                    context.TicketsQueries.Add(tq);
                    context.SaveChanges();
                    if (CurrentCommentId == idDefault)
                        CurrentCommentId = Guid.NewGuid();
                    if (!string.IsNullOrEmpty(CurrentComment))
                    {
                        var comment = new CommentBody()
                        {
                            Comment = CurrentComment,
                            GuidItem = ticket.CallTicketId,
                            CommentId = CurrentCommentId,
                            UserId = CreatedBy
                        };
                        context.CommentBodies.Add(comment);
                        context.SaveChanges();

                        var enclosures = context.CommentEnclosures.Where(m => m.C_CommentId == CurrentCommentId || m.C_PossibleCommentId == CurrentCommentId);
                        foreach (var item in enclosures)
                        {
                            item.C_PossibleCommentId = CurrentCommentId;
                            item.C_CommentId = comment.CommentId;
                        }
                        context.SaveChanges();
                    }
                }
            }
        }

        public void Load(Guid guid)
        {
            using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var existCall = context.CallTickets.FirstOrDefault(m => m.CallTicketId == guid);
                TicketId = existCall.CallTicketId;
                Assigned = existCall.Assigned;
                TicketStatus = existCall.CallStatus;
                CreatedBy = existCall.CreatedBy;
                ObjId = existCall.CustomerGuid;
                DateStarted = existCall.DateStarted;
                TicketNumber = existCall.CallTicketNumber;
                Msg = existCall.Message;
                Title = existCall.Title;
                Phone = existCall.Phone;
                PrevCallId = existCall.PrevCallId;
                DateStartedStr = existCall.DateStarted.ToString("dd.MM.yyyy");
                TimeStartedStr = existCall.DateStarted.ToString("HH:mm");
            }
        }

        public void Save()
        {
            if (!string.IsNullOrEmpty(DateStartedStr))
                DateStarted = Convert.ToDateTime(DateStartedStr, ruDateFormat).Add(Convert.ToDateTime(TimeStartedStr).TimeOfDay);
            if (TicketId == Guid.Empty)
                Create();
            else {
                using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    if (!string.IsNullOrEmpty(CurrentComment))
                    {
                        if(context.CommentBodies.Any(m => m.CommentId == CurrentCommentId))
                             CurrentCommentId = Guid.NewGuid(); 

                        var comment = new CommentBody()
                        {
                            Comment = CurrentComment,
                            DateCreated = DateTime.UtcNow,
                            GuidItem = TicketId,
                            CommentId = CurrentCommentId,
                            UserId = OwnerId,
                        };

                        context.CommentBodies.Add(comment);
                    }

                    if (context.CallTickets.Any(m => m.CallTicketId == TicketId))
                    {
                        var existCall = context.CallTickets.FirstOrDefault(m => m.CallTicketId == TicketId);
                        existCall.Assigned = Assigned;
                        existCall.CallStatus = TicketStatus.HasValue ? TicketStatus.Value : (byte)EnumHelper.TaskStatus.Novelty;
                        existCall.CustomerGuid = ObjId.Value;
                        existCall.DateStarted = DateStarted.Value;

                        var enclosures = context.CommentEnclosures.Where(m => m.C_CommentId == CurrentCommentId || m.C_PossibleCommentId == CurrentCommentId);
                        foreach (var item in enclosures)
                        {
                            item.C_PossibleCommentId = CurrentCommentId;
                            item.C_CommentId = CurrentCommentId;
                        }

                        try
                        {
                            context.SaveChanges();
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            Trace.WriteLine(string.Join("\r\n", dbEx.EntityValidationErrors.SelectMany(m => m.ValidationErrors.Select(n => n.ErrorMessage))));
                        }
                    }
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

            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var ticket = wfContext.CallTickets.FirstOrDefault(m => m.CallTicketId == this.TicketId);

                switch ((WFCallTaskStatus)statusId)
                {
                    case WFCallTaskStatus.EndCall:
                    case WFCallTaskStatus.Meeting:
                        TicketEventsObserver.Instance.EventFired(GetNotifyItem(1, ticket));
                        ticket.CallStatus = (byte)EnumHelper.TaskStatus.Completed;
                        break;
                    case WFCallTaskStatus.FirstCall:
                        ticket.CallStatus = (byte)EnumHelper.TaskStatus.Novelty;
                        break;
                    case WFCallTaskStatus.Recall:
                    case WFCallTaskStatus.Assigned:
                    case WFCallTaskStatus.Commented:                    
                        ticket.CallStatus = (byte)EnumHelper.TaskStatus.Updated;
                        break;
                    case WFCallTaskStatus.Viewed:
                        break;
                    default:
                        ticket.CallStatus = (byte)EnumHelper.TaskStatus.Error;
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
                wfContext.QueryStatus.Add(queryStatus);
                var id = wfContext.TicketsQueries.FirstOrDefault(m=>m.TicketId == ticket.CallTicketId).WorkflowId;
                try
                {
                    wfContext.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    Trace.WriteLine(string.Join("\r\n", dbEx.EntityValidationErrors.SelectMany(m => m.ValidationErrors.Select(n => n.ErrorMessage))));
                }
                finally
                {
                    result = id.HasValue ? id.Value: Guid.Empty;
                }
            }
            return result;
        }

        private NotifyObserveItem GetNotifyItem(int notifyId, SupportContext.CallTicket ticket)
        {
            return new NotifyObserveItem()
            {
                OwnerId = ticket.CreatedBy,
                FromUserId = OwnerId,
                ToUserId = ticket.Assigned,
                NotifyId = notifyId,
                AppendMsg = ticket.Title,
                ObjectId = ticket.CallTicketId.ToString()
            };
        }

        public void ChangePreview(bool value)
        {
            throw new NotImplementedException();
        }

        public string Status { get; set; }

        public bool IsNonePreview { get; set; }
        public Guid OwnerId { get; set; }

        private string GenerateNumber()
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {              
                    do
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            sb.Append(rnd.Next(0, 9));
                        }
                        var val = sb.ToString();
                        if (wfContext.CallTickets.Any(m => m.CallTicketNumber == val))
                            sb = new StringBuilder();
                    } while (sb.Length == 0);
                return sb.ToString();
            }
        }

    }
}
