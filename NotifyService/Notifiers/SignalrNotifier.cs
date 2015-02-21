using LogContext;
using LogRepositories;
using NotifyModel;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyService.Notifiers
{
    public class SignalrNotifier : IEventNotifier
    {
        public Guid UserId { get; set; }

        public void DoNotify(NotifyObserveItem notify, IEnumerable<int> users)
        {
            foreach (var item in users)
            {
                CreateNotifyItem(notify, item);
            }
        }
        public void LogingNotify(NotifyObserveItem notify, int status)
        {
        }

        private void CreateNotifyItem(NotifyObserveItem item, int userNotify)
        {
            using (LogEntities context = new LogEntities(AccessSettings.LoadSettings().LogEntites))
            {
                var notify = context.Notifies.FirstOrDefault(m => m.NotifyId == item.NotifyId);
                context.NotifyHistories.Add(new NotifyHistory()
                {
                    Text = string.Format("{0} :{1}", notify.Name, item.AppendMsg),
                    InitByUserId = item.FromUserId,
                    C_Notify4ListenerId = userNotify,
                    IsViewed = false,
                    ObjectId = item.ObjectId
                });
                context.SaveChanges();
            }
        }

       
    }
}
