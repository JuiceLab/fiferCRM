using LogRepositories;
using NotifyEventModel;
using NotifyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Controllers
{
    public class ReloadController : Controller
    {
        // GET: Reload
        public ActionResult RefreshNotifies(Guid userId)
        {
            if (NotifySingletonWrapper.Instance.Users.ContainsKey(userId))
            {
                var listenerId = NotifySingletonWrapper.Instance.Users[userId];
                NotifyRepository repository = new NotifyRepository();

                var settings = NotifySingletonWrapper.Instance.UserSettings.FirstOrDefault(m => m.ListenerId == listenerId);
                settings.NotifyProfile = repository.GetUserNotifies(userId)
                    .Select(m => new UserNotifyModel()
                    {
                        IsActive = m.IsAvailable,
                        NotifyId = m.C_NotifyId,
                        FromMe = m.FromMe,
                        ToMe = m.ToMe,
                        UnderMe = m.UnderMe,
                    }).ToArray();
            }
            return View();
        }
    }
}