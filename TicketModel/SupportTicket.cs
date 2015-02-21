using EnumHelper;
using Interfaces.Support;
using Settings;
using SupportContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace TicketModel
{
    [MetadataType(typeof(ITicket))]
    [Serializable]
    public class SupportTicket : ITicket
    {
        private object lockObj = new object();

        public Guid TicketId { get; set; }
        public string TicketNumber { get; set; }

        public Guid? MasterId { get; set; }

        [DisplayName("Суть проблемы или вопроса")]
        public string Title { get; set; }

        [DisplayName("Тип вопроса")]
        public int? CategoryId { get; set; }

        public string CategoryName { get; set; }

        [DisplayName("Подробное описание")]
        public string Msg { get; set; }

        [DisplayName("Подробное описание")]
        public string CurrentComment { get; set; }

        public Guid CurrentCommentId { get; set; }

        public string AssignedName { get; set; }

        public Guid? Assigned { get; set; }

        public DateTime DateModified { get; set; }

        public Guid OwnerId { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateAssigned { get; set; }

        public DateTime? DateStarted { get; set; }

        public Guid TicketOwnerId { get; set; }

        public string TicketStatusStr { get; set; }

        public string DateCreatedStr { get { return DateCreated.ToString("dd.MM HH:mm"); } }

        public string DateAssignedStr
        {
            get
            {
                return DateAssigned.HasValue ?
                    DateAssigned.Value.ToString("dd.MM HH:mm")
                    : "null";
            }
        }

        public string DateStartedStr
        {
            get
            {
                return DateStarted.HasValue ?
                    DateStarted.Value.ToString("dd.MM HH:mm")
                    : "null";
            }
        }

        public string Status { get; set; }

        public byte? TicketStatus { get; set; }
        
        public DateTime? TimeToExpired { get; set; }
        public DateTime? ExpiredDate { get; set; }

        public byte Priority { get; set; }

        public Guid? ObjId { get; set; }

        public object EnvelopObj { get; set; }

        public bool IsNonePreview { get; set; }

        public IDictionary<string, IEnumerable<Guid>> CommonActionTickets { get; set; }

        public Guid ChangeStatus(int statusId, string customStatus = "")
        {
            Guid result = new Guid();
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var ticket = wfContext.Tickets.FirstOrDefault(m => m.TicketId == this.TicketId);

                switch ((WFStatus)statusId)
                {
                    case WFStatus.Novelty:
                        ticket.TicketStatus = (byte) EnumHelper.TicketStatus.Novelty;
                        break;
                    case WFStatus.Processed:
                    case WFStatus.Opened:
                    case WFStatus.NonConfirm:
                        ticket.TicketStatus = (byte)EnumHelper.TicketStatus.Processing;
                        break;
                    case WFStatus.Commented:
                        ticket.TicketStatus = (byte)EnumHelper.TicketStatus.Updated;
                        break;
                    case WFStatus.Completed:
                        ticket.TicketStatus = (byte)EnumHelper.TicketStatus.Closed;
                        break;
                    default:
                        ticket.TicketStatus = (byte)EnumHelper.TicketStatus.Processing;
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
                    result = TicketId;
                }
            }
            return result;
        }

        public void Create()
        {
            Guid idDefault = new Guid();
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                if (TicketId == idDefault)
                    TicketId = Guid.NewGuid();
                if (string.IsNullOrEmpty(CurrentComment))
                    CurrentComment = Msg;
                lock (lockObj)
                {
                    var ticket = new Ticket()
                 {
                     TicketId = TicketId,
                     Title = Title,
                     CreatedBy = CreatedBy,
                     Assigned = Assigned,
                     DateStarted = DateStarted,
                     DateModified = DateTime.UtcNow,
                     CategoryId = CategoryId,
                     MasterIdFK = MasterId,
                     Message = Msg,
                     TicketStatus = TicketStatus.HasValue ? TicketStatus : 1,
                     Priory = Priority,
                     ExpiredDate = ExpiredDate,
                     TicketNumber = GenerateNumber(),
                     CurrentComment = CurrentComment
                 };

                    if (!wfContext.Tickets.Any(m => m.TicketId == TicketId))
                    {
                        try
                        {
                            wfContext.Tickets.Add(ticket);
                            wfContext.SaveChanges();

                            TicketsQuery tq = new TicketsQuery()
                            {
                                TicketQueryId = Guid.NewGuid(),
                                TicketId = ticket.TicketId,
                                IsNonePreview = true
                            };

                            if (CurrentCommentId == idDefault)
                                CurrentCommentId = Guid.NewGuid();

                            wfContext.TicketsQueries.Add(tq);

                            var comment = new CommentBody()
                            {
                                Comment = CurrentComment,
                                GuidItem = ticket.TicketId,
                                CommentId = CurrentCommentId,
                                UserId = CreatedBy
                            };
                            wfContext.CommentBodies.Add(comment);
                            wfContext.SaveChanges();

                            var enclosures = wfContext.CommentEnclosures.Where(m => m.C_CommentId == CurrentCommentId || m.C_PossibleCommentId == CurrentCommentId);
                            foreach (var item in enclosures)
                            {
                                item.C_PossibleCommentId = CurrentCommentId;
                                item.C_CommentId = comment.CommentId;
                            }


                            wfContext.SaveChanges();
                            ChangeStatus((int)WFStatus.Novelty);
                            //if (wfContext.ActivationTicketsView.Any(m => m.TicketId == ticket.TicketId))
                            //{
                            //    var recordActivation = wfContext.ActivationTicketsView.FirstOrDefault(m => m.TicketId == ticket.TicketId);
                            //    InternalFilterApiConnector apiFiltering = new InternalFilterApiConnector(ManagerContext.CurDest, "Ticket");
                            //    apiFiltering.GetUrlContentFilter(new SupportTicket(), 0, "/AddTicketObjOrders?destSource=" + ManagerContext.CurDest + "&ticketId=" + recordActivation.TicketId + "&objId=" + recordActivation.ActivationId);
                            //}

                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            Trace.WriteLine(string.Join("\r\n", dbEx.EntityValidationErrors.SelectMany(m => m.ValidationErrors.Select(n => n.ErrorMessage))));
                        }
                    }
                }
            }
        }

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
                    if (wfContext.Tickets.Any(m => m.TicketNumber == val))
                        sb = new StringBuilder();
                } while (sb.Length == 0);
                return sb.ToString();
            }
        }

        public void Save()
        {
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {

                if (!string.IsNullOrEmpty(CurrentComment))
                {
                    var comment = new CommentBody()
                    {
                        Comment = CurrentComment,
                        DateCreated = DateTime.UtcNow,
                        GuidItem = TicketId,
                        CommentId = wfContext.CommentBodies.Any(m => m.CommentId == CurrentCommentId) ? Guid.NewGuid() : CurrentCommentId,
                        UserId = OwnerId,
                    };

                    wfContext.CommentBodies.Add(comment);
                }

                if (wfContext.Tickets.Any(m => m.TicketId == TicketId))
                {
                    Ticket existTicket = wfContext.Tickets.FirstOrDefault(m => m.TicketId == TicketId);
                    existTicket.Assigned = Assigned;
                    existTicket.TicketStatus = TicketStatus;
                    existTicket.DateAssigned = DateAssigned;
                    existTicket.DateModified = DateTime.UtcNow;
                    existTicket.Message = Msg;
                    existTicket.IsNonePreview = true;
                    existTicket.CurrentComment = string.Empty;
                    existTicket.ExpiredDate = TimeToExpired.HasValue ?
                        this.TimeToExpired.Value.Add(new TimeSpan(DateTime.UtcNow.Ticks))
                        : ExpiredDate.HasValue ?
                            ExpiredDate
                            : new Nullable<DateTime>();
                    existTicket.Priority = Priority;

                }

                var enclosures = wfContext.CommentEnclosures.Where(m => m.C_CommentId == CurrentCommentId || m.C_PossibleCommentId == CurrentCommentId);
                foreach (var item in enclosures)
                {
                    item.C_PossibleCommentId = CurrentCommentId;
                    item.C_CommentId = CurrentCommentId;
                }

                try
                {
                    wfContext.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    Trace.WriteLine(string.Join("\r\n", dbEx.EntityValidationErrors.SelectMany(m => m.ValidationErrors.Select(n => n.ErrorMessage))));
                }
            }
        }

        public void Load(Guid guid)
        {
            Ticket ticket = null;
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                ticket = wfContext.Tickets.FirstOrDefault(m => m.TicketId == guid);

                if (ticket != null)
                {
                    ticket.Load(guid);
                    this.TicketId = ticket.TicketId;
                    this.Title = ticket.Title;
                    this.MasterId = ticket.MasterIdFK;
                    this.TicketNumber = ticket.TicketNumber;
                    this.TimeToExpired = ticket.ExpiredDate.HasValue && ticket.ExpiredDate > DateTime.UtcNow ?
                        ticket.ExpiredDate.Value.Add(-new TimeSpan(DateTime.UtcNow.Ticks))
                        : new Nullable<DateTime>();
                    this.Priority = ticket.Priority;
                    this.OwnerId = ticket.OwnerId;
                    this.Assigned = ticket.Assigned;
                    this.AssignedName = ticket.AssignedName;
                    this.CategoryId = ticket.CategoryId;
                    this.CategoryName = ticket.CategoryId.HasValue ? ticket.Category.Name : string.Empty;
                    this.DateStarted = ticket.DateStarted;
                    this.CurrentCommentId = ticket.CurrentCommentId;
                    this.Msg = ticket.Message == null ? "" : HttpUtility.HtmlDecode(ticket.Message);
                    this.CurrentComment = HttpUtility.HtmlDecode(wfContext.CommentBodies.Where(m => m.GuidItem == ticket.TicketId).OrderByDescending(m => m.DateCreated).First().Comment);
                    var status = wfContext.QueryStatus.Where(m => m.QueryItemId == guid).OrderByDescending(m => m.DateCreated).FirstOrDefault();
                    this.Status = status == null ? "" : status.StatusId.HasValue ? ((WFStatus)status.StatusId).GetStringValue() : status.CustomStatus;
                    this.TicketStatus = ticket.TicketStatus;
                    this.CreatedBy = ticket.CreatedBy;
                    this.DateModified = ticket.DateModified;
                    this.DateCreated = ticket.DateCreated;
                    this.DateAssigned = ticket.DateAssigned;
                }
            }
        }

        public void LoadById(int id)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public Guid CreateMaster(string masterType, Guid wfGuid, Guid? masterId = null)
        {
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                Guid result = masterId.HasValue ?
                                     masterId.Value
                                     : System.Guid.NewGuid();
                wfContext.TicketMasters.Add(new TicketMaster()
                {
                    MasterId = result,
                    MasterType = masterType,
                    InstanceWfId = wfGuid
                });
                try
                {
                    wfContext.SaveChanges();
                    return result;
                }
                catch (DbEntityValidationException dbEx)
                {
                    Trace.WriteLine(string.Join("\r\n", dbEx.EntityValidationErrors.SelectMany(m => m.ValidationErrors.Select(n => n.ErrorMessage))));
                    return Guid.Empty;
                }
            }
        }

        public void AutoTicket(string title, string infoComment, string typeMaster, Guid instanceId, TicketStatus status)
        {
            Title = title;
            MasterId = CreateMaster(typeMaster, instanceId);
            DateModified = DateTime.UtcNow;
            DateCreated = DateTime.UtcNow;
            CreatedBy = new Guid();
            CurrentComment = string.IsNullOrEmpty(infoComment) ? "" : infoComment;
            TicketStatus = (byte)status;
            Priority = 1;
            Create();
            ChangeStatus(0, title + " " + infoComment);
        }

        public void AutoClose()
        {
            if (CurrentCommentId == Guid.Empty)
                CurrentCommentId = Guid.NewGuid();
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                if (!wfContext.QueryStatus.Any(m => m.QueryItemId == TicketId && m.StatusId == (int)WFStatus.Completed))
                {
                    var ticket = wfContext.Tickets.FirstOrDefault(m => m.TicketId == TicketId);
                    if (ticket.TicketStatus != (byte)EnumHelper.TicketStatus.Closed)
                    {
                        ticket.TicketStatus = (byte)EnumHelper.TicketStatus.Closed;
                        ticket.DateModified = DateTime.UtcNow;
                        ticket.Assigned = Assigned;
                        var comment = wfContext.CommentBodies.SingleOrDefault(m => m.CommentId == CurrentCommentId);
                        if (comment == null)
                        {
                            comment = new CommentBody()
                            {
                                CommentId = CurrentCommentId,
                                Comment = CurrentComment,
                                DateCreated = DateTime.UtcNow,
                                UserId = Assigned.HasValue ? Assigned : CreatedBy
                            };
                            wfContext.CommentBodies.Add(comment);
                        }
                        comment.Comment = CurrentComment;
                        wfContext.SaveChanges();

                    }
                    ChangeStatus((int)WFStatus.Completed);
                }
            }
        }

        public void LoadTicketByObjId(Guid objID)
        {
            Load(objID);
        }

        public void ChangePreview(bool value)
        {
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                wfContext.TicketsQueries.FirstOrDefault(m => m.TicketId == this.TicketId).IsNonePreview = value;

                var ticket = wfContext.Tickets.FirstOrDefault(m => m.TicketId == this.TicketId);
              
                ticket.TicketStatus = value ?
                    (byte)EnumHelper.TicketStatus.Processing
                    : (byte)EnumHelper.TicketStatus.Updated;

                var queryStatus = new QueryStatu()
                {
                    QueryItemId = this.TicketId,
                    OwnerId = this.OwnerId,
                    StatusId = value ? (byte)WFStatus.Processed : (byte)WFStatus.Opened,
                    DateCreated = DateTime.UtcNow
                };
                wfContext.QueryStatus.Add(queryStatus);
                try
                {
                    wfContext.SaveChanges();
                }
                catch (DbEntityValidationException dbEx)
                {
                    Trace.WriteLine(string.Join("\r\n", dbEx.EntityValidationErrors.SelectMany(m => m.ValidationErrors.Select(n => n.ErrorMessage))));
                }
            }
        }
        private void NotifyAssigned()
        {
            List<string> notifyMail = new List<string>();
           //todo notify
        }
    }

}
