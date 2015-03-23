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
        private const string defaultLocalDB = "fifer_localcrm_0";
        
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;
        
        private object lockObj = new object();

        public MeetingTicket()
        { }
        public MeetingTicket(Guid userId)
        {
            OwnerId = userId;
        }

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
            Guid idDefault = new Guid();

            using (CompanyContext.CompanyViewEntities viewContext = new CompanyContext.CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites))
            {
                var localDb = viewContext.UserInCompanyView.FirstOrDefault(m => m.UserId == OwnerId).LocalDB;
                using (LocalCRMEntities localContext = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites.Replace(defaultLocalDB, localDb)))
                {
                    lock (lockObj)
                    {
                        var ticket = new Meeting()
                        {
                            C_CustomerId = localContext.Customers.FirstOrDefault(m => m.CustomerGuid == CustomerId).CustomerId,
                            Calls = Calls,
                            CreatedBy = OwnerId,
                            Created = DateTime.UtcNow,
                            Comment = Msg,
                            Date = DateStarted.Value,
                            Goals = Goals,
                            Assigned = Assigned.HasValue? Assigned.Value : Guid.Empty,
                            MeetingGuid = Guid.NewGuid(),
                            ResultComment = string.Empty,
                            StatusId = (byte)WFMeetingStatus.Novelty,
                        };
                        localContext.Meetings.Add(ticket);
                        localContext.SaveChanges();
                        TicketId = ticket.MeetingGuid;
                        using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                        {
                            TicketsQuery tq = new TicketsQuery()
                                    {
                                        TicketQueryId = Guid.NewGuid(),
                                        TicketId = ticket.MeetingGuid,
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
                                    GuidItem = ticket.MeetingGuid,
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
            }
        }

        public void Load(Guid guid)
        {
            using (CompanyContext.CompanyViewEntities viewContext = new CompanyContext.CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites))
            {
                var localDb = viewContext.UserInCompanyView.FirstOrDefault(m => m.UserId == OwnerId).LocalDB;
                using (LocalCRMEntities context = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites.Replace(defaultLocalDB, localDb)))
                {
                    var meeting = context.Meetings.FirstOrDefault(m => m.MeetingGuid == guid);
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
                    TimeStartedStr = meeting.Date.ToShortTimeString();
                    DateStartedStr = meeting.Date.ToShortDateString();
                }
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
                using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    if (!string.IsNullOrEmpty(CurrentComment))
                    {
                        if (context.CommentBodies.Any(m => m.CommentId == CurrentCommentId))
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
                    using (CompanyContext.CompanyViewEntities viewContext = new CompanyContext.CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites))
                    {
                        var localDb = viewContext.UserInCompanyView.FirstOrDefault(m => m.UserId == OwnerId).LocalDB;
                        using (LocalCRMEntities localContext = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites.Replace(defaultLocalDB, localDb)))
                        {
                            var meeting = localContext.Meetings.FirstOrDefault(m => m.MeetingGuid == TicketId);
                            meeting.Date = DateStarted.HasValue ? DateStarted.Value : DateTime.Now;
                            meeting.Goals = Goals;
                            meeting.Assigned = Assigned.Value;
                            meeting.ResultComment = Result;
                            localContext.SaveChanges();
                        }
                    }

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

            using (CompanyContext.CompanyViewEntities viewContext = new CompanyContext.CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites))
            {
                var localDb = viewContext.UserInCompanyView.FirstOrDefault(m => m.UserId == OwnerId).LocalDB;
                using (LocalCRMEntities context = new LocalCRMEntities(AccessSettings.LoadSettings().LocalCrmEntites.Replace(defaultLocalDB, localDb)))
                {
                    var meeting = context.Meetings.FirstOrDefault(m => m.MeetingGuid == TicketId);

                    switch ((WFMeetingStatus)statusId)
                    {
                        case WFMeetingStatus.EndCall:
                        case WFMeetingStatus.EndMeet:
                        case WFMeetingStatus.EndPayment:
                        case WFMeetingStatus.Ended:
                            TicketEventsObserver.Instance.EventFired(GetNotifyItem(1, meeting));
                            meeting.StatusId = (byte)EnumHelper.TaskStatus.Completed;
                            meeting.ResultComment = CurrentComment;
                            break;
                        case WFMeetingStatus.AssignMeet:
                        case WFMeetingStatus.TransferMeet:
                        case WFMeetingStatus.Commented:
                            meeting.StatusId = (byte)EnumHelper.TaskStatus.Updated;
                            break;
                        case WFMeetingStatus.Novelty:
                            meeting.StatusId = (byte)EnumHelper.TaskStatus.Novelty;
                            break;
                        case WFMeetingStatus.Viewed:
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
                        var id = wfContext.TicketsQueries.FirstOrDefault(m => m.TicketId == meeting.MeetingGuid).WorkflowId;

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
                            result = id.HasValue ? id.Value : Guid.Empty;
                        }
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
