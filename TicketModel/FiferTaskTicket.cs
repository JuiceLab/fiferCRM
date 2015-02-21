using AccessRepositories;
using CRMModel;
using CRMRepositories;
using EnumHelper;
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TicketModel.Interfaces;
using UserContext;
using EntityRepository;
using System.Globalization;

namespace TicketModel
{
    [MetadataType(typeof(ITicket))]
    [Serializable]
    public class FiferTaskTicket : TaskTicket, IFiferTask
    {
        private IFormatProvider ruDateFormat = new CultureInfo("ru-RU").DateTimeFormat;

        [Required]
        public string DateStartedStr { get; set; }

        [Required]
        public string TimeStartedStr { get; set; }

        public DateTime? NotifyDate
        {
            get
            {
                return DateStarted.HasValue && !string.IsNullOrEmpty(NotifyBeforeStr) ?
                    DateStarted.Value.Add(-Convert.ToDateTime(TimeStartedStr).TimeOfDay) 
                    : new Nullable<DateTime>();
            }
        }
        public string NotifyBeforeStr { get; set; }
       
        public bool HasGroup { get; set; }
        public bool HasPeriods { get; set; }
        public TaskGroup Group { get; set; }

        public List<TaskPeriod> Periods { get; set; }

        public Guid ChangeStatus(int statusId, string customStatus = "")
        {
            Guid result = new Guid();
            NotifyRepository repository = new NotifyRepository();

            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var ticket = wfContext.Tickets.FirstOrDefault(m => m.TicketId == this.TicketId);

                switch ((WFTaskStatus)statusId)
                {
                    case WFTaskStatus.Novelty:
                        TicketEventsObserver.Instance.EventFired(GetNotifyItem(1, ticket));
                        ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Novelty;
                        break;
                    case WFTaskStatus.Views:
                        if (ticket.TicketStatus != (byte)EnumHelper.TaskStatus.Expired)
                            ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Processing;
                        break;
                    case WFTaskStatus.Processed:
                        ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Processing;
                        break;
                    case WFTaskStatus.Commented:
                    case WFTaskStatus.Assigned:
                        if (ticket.TicketStatus != (byte)EnumHelper.TaskStatus.Expired)
                            ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Updated;
                        break;
                    case WFTaskStatus.Scheduling:
                        TicketEventsObserver.Instance.EventFired(GetNotifyItem(6, ticket));
                        ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Updated;
                        break;
                    case WFTaskStatus.Expired:
                        ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Expired;
                        break;
                    case WFTaskStatus.Completed:
                        TicketEventsObserver.Instance.EventFired(GetNotifyItem(3, ticket));
                        ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Completed;
                        break;
                    default:
                        ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Error;
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

        private NotifyObserveItem GetNotifyItem(int notifyId, Ticket ticket)
        {
            return new NotifyObserveItem()
            {
                OwnerId = ticket.CreatedBy,
                FromUserId = OwnerId,
                ToUserId = ticket.Assigned,
                NotifyId = notifyId,
                AppendMsg = ticket.TicketNumber,
                ObjectId = ticket.TicketId.ToString()
            };
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
                        CategoryId = 3,
                        MasterIdFK = MasterId,
                        CustomCategoryId = CategoryId,
                        Message = CurrentComment,
                        TicketStatus = TicketStatus.HasValue ? TicketStatus : (byte)EnumHelper.TaskStatus.Novelty,
                        Priory = Priority,
                        ExpiredDate = ExpiredDate,
                        TicketNumber = GenerateNumber()
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
                            ChangeStatus((int)WFTaskStatus.Novelty);
                        }
                        catch (DbEntityValidationException dbEx)
                        {
                            Trace.WriteLine(string.Join("\r\n", dbEx.EntityValidationErrors.SelectMany(m => m.ValidationErrors.Select(n => n.ErrorMessage))));
                        }
                    }
                }
            }

            CreateGroup();
            CreatePeriods(CreatedBy);
        }

        public void Save()
        {
            using (TicketEntities wfContext = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
               
                    UpdateGroup();
                    UpdatePeriods(OwnerId);
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
                    this.HasGroup = ticket.TicketGroups.Count > 0;
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
                    this.Status = status == null ? "" : status.StatusId.HasValue ? ((WFTaskStatus)status.StatusId).GetStringValue() : status.CustomStatus;
                    this.TicketStatus = ticket.TicketStatus;
                    this.CreatedBy = ticket.CreatedBy;
                    this.DateModified = ticket.DateModified;
                    this.DateCreated = ticket.DateCreated;
                    this.DateAssigned = ticket.DateAssigned;
                }
            }
            LoadPeriods(OwnerId);
            LoadGroup();
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
                if (!wfContext.QueryStatus.Any(m => m.QueryItemId == TicketId && m.StatusId == (int)WFTaskStatus.Completed))
                {
                    var ticket = wfContext.Tickets.FirstOrDefault(m => m.TicketId == TicketId);
                    if (ticket.TicketStatus != (byte)EnumHelper.TaskStatus.Completed)
                    {
                        ticket.TicketStatus = (byte)EnumHelper.TaskStatus.Completed;
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
                    ChangeStatus((int)WFTaskStatus.Completed);
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
                    (byte)EnumHelper.TaskStatus.Processing
                    : (byte)EnumHelper.TaskStatus.Updated;

                var queryStatus = new QueryStatu()
                {
                    QueryItemId = this.TicketId,
                    OwnerId = this.OwnerId,
                    StatusId = value ? (byte)WFTaskStatus.Processed : (byte)WFTaskStatus.Views,
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

        public void CreatePeriods(Guid userId)
        {
            if (HasPeriods)
            {
                Periods[0].Assigned = Assigned;
                Periods[0].TicketId = TicketId;
                CRMLocalRepository repository = new CRMLocalRepository(userId);
                repository.CreatePeriods(Periods, Group);
            }
        }

        public void UpdatePeriods(Guid userId)
        {
            if (HasPeriods)
            {
                CRMLocalRepository repository = new CRMLocalRepository(userId);
                repository.UpdatePeriods(Periods, Group);
            }
        }
        public void LoadPeriods(Guid userId) {
            CRMLocalRepository repository = new CRMLocalRepository(userId);
            Periods = repository.GetTaskPeriods(this.TicketId);
            if (Periods.Count > 0)
                HasPeriods = true;
        }

        public void CreateGroup()
        {
            if (HasGroup)
            {
                using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    if (this.Group.GroupTaskId == 0)
                    {
                        var existGroup = context.InsertUnit(new TicketGroup()
                        {
                            TicketId = TicketId,
                            AssignedUsers = Group.AssignedGroups == null ? string.Empty 
                                            :string.Join(",", Group.AssignedUsers),
                            AssignedGroups = Group.AssignedGroups == null ? string.Empty 
                                            : string.Join(",", Group.AssignedGroups),
                            AssignedDepartments = Group.AssignedGroups == null ? string.Empty 
                                            : string.Join(",", Group.AssignedDepartments),
                            UserStatuses = Group.AssignedGroups == null ? string.Empty 
                                            :  string.Join(",", Group.AssignedUsers.Select(m => (byte)EnumHelper.TaskStatus.Novelty)),
                        });
                    }
                }
            }
        }
        public void LoadGroup()
        {
            using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
            {
                var curGroup = context.TicketGroups.FirstOrDefault(m => m.TicketId == this.TicketId);
                if (curGroup != null)
                {
                    HasGroup = true;
                    Group = new TaskGroup()
                   {
                       AssignedDepartments = curGroup.AssignedDepartments.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList(),
                       AssignedGroups = curGroup.AssignedGroups.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList(),
                       GroupTaskId = curGroup.GroupTaskId,
                       TicketId = curGroup.TicketId,
                       AssignedUsers = curGroup.AssignedUsers.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList(),
                       UserStatuses = curGroup.UserStatuses.Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries).ToList()
                   };
                    MembershipRepository repository = new MembershipRepository();
                    var curIds = curGroup
                        .AssignedUsers
                        .Split(new char[]{','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(m => Guid.Parse(m)).ToList();

                    var users = repository.GetUserByIds(curIds);
                    Group.AssignedUserNames = curIds.Select(m => users.FirstOrDefault(n => n.UserId == m).FirstName + " " + users.FirstOrDefault(n => n.UserId == m).LastName).ToList();
                }
                else
                    Group = new TaskGroup();
            }
        }

        public void UpdateGroup() {
            if (HasGroup)
            {
                using (TicketEntities context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites))
                {
                    if (this.Group.GroupTaskId == 0)
                        CreateGroup();
                    else
                    {
                        var existGroup = context.TicketGroups.FirstOrDefault(m => m.GroupTaskId == this.Group.GroupTaskId);
                        existGroup.AssignedUsers = string.Join(",", Group.AssignedUsers);
                        existGroup.AssignedGroups = string.Join(",", Group.AssignedGroups);
                        existGroup.AssignedDepartments = string.Join(",", Group.AssignedDepartments);
                        existGroup.UserStatuses =string.Join(",",  Group.UserStatuses);
                        context.SaveChanges();
                    }
                }
            }
        }
    }
}
