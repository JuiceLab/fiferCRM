using EnumHelper;
using Interfaces.Support;
using Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using UserContext;

namespace SupportContext
{
    [MetadataType(typeof(ITicket))]
    public partial class Ticket : ITicket
    {
        public Guid OwnerId { get; set; }

        public string CategoryName { get; set; }

        public string Msg { get { return Message; } set { Message = value; } }

        public string CurrentComment { get; set; }

        public Guid CurrentCommentId { get; set; }

        public DateTime? TimeToExpired { get; set; }

        public byte Priority { get; set; }

        public string AssignedName { get; set; }

        public string Status { get; set; }

        public Guid? MasterId { get; set; }

        public Guid? ObjId { get; set; }

        public object EnvelopObj { get; set; }

        public bool IsNonePreview { get; set; }

        public Guid TicketOwnerId { get; set; }

        public string TicketStatusStr { get; set; }

        public IDictionary<string, IEnumerable<Guid>> CommonActionTickets { get; set; }

        public Guid ChangeStatus(int statusId, string customStatus = "")
        {
            Guid result = new Guid();
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var queryStatus = new QueryStatu()
                {
                    QueryItemId = this.TicketId,
                };
                if (string.IsNullOrEmpty(customStatus))
                    queryStatus.QueryStatusId = statusId;
                else
                    queryStatus.CustomStatus = customStatus;
                wfContext.QueryStatus.Add(queryStatus);
                try
                {
                    wfContext.SaveChanges();
                }
                catch (UpdateException ex)
                {
                }
                finally
                {
                    result = TicketId;
                }
            }
            return result;
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void AutoClose()
        {
            throw new NotImplementedException();
        }

        public void Load(Guid guid)
        {
            ITicket ticket = null;
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                ticket = wfContext.Tickets.SingleOrDefault(m => m.TicketId == guid);
                CreatedBy = ticket.CreatedBy;
                OwnerId = ticket.CreatedBy;
                Priority = ticket.Priority;
                Assigned = ticket.Assigned;
                if (Assigned.HasValue)
                {
                    using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
                    {
                        var user = access.Users.FirstOrDefault(m => m.UserId == Assigned);
                        if (user != null)
                            AssignedName = user.Login;
                    }
                }

                var queryStatus = wfContext
                    .QueryStatus
                    .Where(m => m.QueryItemId == guid)
                    .OrderByDescending(m => m.DateCreated)
                    .FirstOrDefault();
                var lastComment = wfContext.CommentBodies
                 .Where(m => m.GuidItem == guid)
                 .OrderByDescending(m => m.DateCreated)
                 .First();

                if (queryStatus != null)
                    Status = string.IsNullOrEmpty(queryStatus.CustomStatus) ?
                        ((WFStatus)queryStatus.StatusId).GetStringValue()
                        : queryStatus.CustomStatus;
                else
                    Status = ((TicketStatus)TicketStatus).GetStringValue();

                CategoryName = CategoryId.HasValue ?
                wfContext.Categories.SingleOrDefault(m => m.CategoryId == ticket.CategoryId).Name
                : "рабочий тикет";
                CurrentComment = HttpUtility.HtmlDecode(lastComment.Comment);
                CurrentCommentId = lastComment.CommentId;
            }
            Msg = HttpUtility.HtmlDecode(ticket.Msg);
            CategoryId = ticket.CategoryId;
        }

        public void LoadById(int id)
        {
            throw new NotImplementedException();
        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void LoadTicketByObjId(Guid objID)
        {
            throw new NotImplementedException();
        }


        public Guid CreateMaster(string masterType, Guid wfGuid, Guid? masterId)
        {
            throw new NotImplementedException();
        }

        public void ChangePreview(bool value)
        {
            throw new NotImplementedException();
        }
    }
}
