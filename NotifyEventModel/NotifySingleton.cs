using LogContext;
using LogRepositories;
using NotifyModel;
using PatternTemplate;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyEventModel
{
     public class NotifySingletonWrapper
    {
        private NotifySingletonWrapper()
        {
        }

        public static NotifySingleton Instance
        {
            get
            {
                return Singleton<NotifySingleton>.Instance;
            }
        }

        public static NotifySingleton ResetValues
        {
            get
            {
                return Singleton<NotifySingleton>.ReloadInstance;
            }
        }
    }
     public class NotifySingleton
     {
         public string MainHost { get; set; }
         public List<string> Host4Refresh { get; set; }
         public IList<Notify> Notifies { get; set; }
         public Dictionary<Guid, int> Users { get; set; }
         public IList<UserSettings> UserSettings { get; set; }

         public UserSettings GetUserById(Guid userId)
         {
             return Users.ContainsKey(userId) ?
                 UserSettings.FirstOrDefault(m => m.ListenerId == Users[userId])
                 : new UserSettings();
         }

         private NotifySingleton()
         {
             using (LogEntities context = new LogEntities(AccessSettings.LoadSettings().LogEntites))
             {
                 NotifyRepository repository = new NotifyRepository();
                 Notifies = context.Notifies.ToList();
                 Users = context.Listeners.ToDictionary(m => m.UserID, m => m.ListenerId);
                 UserSettings = context.Notify4Listener
                     .ToList()
                     .GroupBy(m => m.C_ListenerId)
                     .Select(m => new UserSettings()
                     {
                         ListenerId = m.Key,
                         NotifyProfile = m.Select(n => new UserNotifyModel()
                         {
                             IsActive = n.IsAvailable,
                             NotifyId = n.C_NotifyId,
                             FromMe = n.FromMe,
                             ToMe = n.ToMe,
                             UnderMe = n.UnderMe
                         }).ToArray()
                     }).ToList();
             }
#if (DEBUG)
             MainHost = "http://localhost:41252/";
             Host4Refresh = new List<string>    (){"http://localhost:41252/", "http://localhost:2271/"};
#else
             MainHost = "http://bizinvit.ru/";
             Host4Refresh = new List<string>() {"http://wf.bizinvit.ru/", "http://bizinvit.ru/" };

#endif    
         }
     }
}
