using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NotifyModel
{
    public class UserSettings
    {
        public int ListenerId { get; set; }
        public Guid UserId { get; set; }
        public UserNotifyModel[] NotifyProfile { get; set; }

        public UserSettings()
        {
            NotifyProfile = new UserNotifyModel[0];
        }

        public void RefreshUserNotifies(List<string> hosts, Guid userId)
        {
            using (WebClient wc = new WebClient())
            {
                try
                {
                    foreach (var item in hosts)
                    {
                        wc.DownloadString(item + "/Reload/RefreshNotifies?userId=" + userId);
                    }
                }catch
                { }
            }
        }
    }
}
