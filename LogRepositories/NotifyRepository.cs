using EnumHelper;
using LogContext;
using NotifyModel;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskModel;
using EntityRepository;

namespace LogRepositories
{
    public class NotifyRepository : BaseLogRepository, IDisposable
    {
        private const int lastView = 3;
        public NotifyRepository()
            : base() 
        { }
        
        public void CreateEmployeeDeafultNotifySettings(Guid userId)
        {
            var listener = new Listener()
            {
                UserID = userId
            };

            Context.Listeners.Add(listener);
            Context.SaveChanges();

            foreach (var item in Context.Notifies)
            {
                Context.Notify4Listener.Add(new Notify4Listener()
                {
                    IsAvailable = true,
                    C_ListenerId = listener.ListenerId,
                    C_NotifyId = item.NotifyId,
                    FromMe = true,
                    ToMe = true,
                    UnderMe = true
                });
            }
            Context.SaveChanges();
        }

        public IEnumerable<MessageViewModel> GetNotifies(Guid userId)
        {
            var notifyProfile = Context.Listeners.FirstOrDefault(m => m.UserID == userId).Notify4Listener.Select(m => m.Notify4ListenerId);
            return Context.NotifyHistories
                                   .Where(m => notifyProfile.Contains(m.C_Notify4ListenerId) )
                                   .OrderByDescending(m => m.HistoryItemId)
                                   .Take(lastView)
                                   .ToList()
                                   .Select(m => new MessageViewModel()
                                   {
                                       Created = m.DateCreated,
                                       UserId = m.InitByUserId.HasValue ? m.InitByUserId.Value : Guid.Empty,
                                       Msg = m.Text,
                                       Type = (byte)MsgType.Msg,
                                       IsViewed = m.IsViewed
                                   });
        }

        public List<MessageViewModel> GetUnViewedNotifies(Guid userId)
        {
            var notifyProfile = Context.Listeners.FirstOrDefault(m => m.UserID == userId).Notify4Listener.Select(m => m.Notify4ListenerId);
            return Context.NotifyHistories
                                   .Where(m => notifyProfile.Contains(m.C_Notify4ListenerId) && !m.IsViewed)
                                   .OrderByDescending(m => m.HistoryItemId)
                                   .Take(3)
                                   .ToList()
                                   .Select(m => new MessageViewModel()
                                   {
                                       NotifyId = m.HistoryItemId,
                                       Created = m.DateCreated,
                                       UserId = m.InitByUserId.HasValue ? m.InitByUserId.Value : Guid.Empty,
                                       Msg = m.Text,
                                       Type = (byte)MsgType.Msg,
                                       IsLocal = false
                                   })
                                   .ToList();
        }

        public void SetLastViewed(Guid userId)
        {
            var notifyProfile = Context.Listeners.FirstOrDefault(m => m.UserID == userId).Notify4Listener.Select(m=>m.Notify4ListenerId);
            foreach (var item in  Context.NotifyHistories
                                    .Where(m => notifyProfile.Contains(m.C_Notify4ListenerId) && !m.IsViewed)
                                    .OrderByDescending(m=>m.HistoryItemId)
                                    .Take(lastView))
            {
                item.IsViewed = true;
            }
            Context.SaveChanges();
        }

        public IList<Notify4Listener> GetUserNotifies(Guid userId)
        {
            return Context.Listeners
                .FirstOrDefault(m => m.UserID == userId)
                .Notify4Listener
                .ToList();
        }

        public void UpdateUserNotifies(UserSettings settings, int userId)
        {
            foreach (var item in settings.NotifyProfile)
            {
                var curSetting = Context.Notify4Listener.FirstOrDefault(m => m.C_ListenerId == userId && m.C_NotifyId == item.NotifyId);
                if (curSetting != null)
                {
                    curSetting.FromMe = item.FromMe;
                    curSetting.ToMe = item.ToMe;
                    curSetting.UnderMe = item.UnderMe;
                    curSetting.IsAvailable = item.IsActive;
                }
            }
            Context.SaveChanges();
        }

        public void SetViewedNotify(int notifyItemId)
        {
            Context.GetUnitById<NotifyHistory>(notifyItemId).IsViewed = true;
            Context.SaveChanges();
        }
    }
}
