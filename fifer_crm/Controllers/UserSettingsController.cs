using LogRepositories;
using NotifyModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace fifer_crm.Controllers
{
    public class UserSettingsController : Controller
    {
        // GET: UserSettings
        [DisplayName("Настройки пользователя")]
        public ActionResult SettingsModal()
        {
            NotifyRepository repository = new NotifyRepository();
            UserSettings model = new  UserSettings();
            repository.GetUserNotifies((Guid)Membership.GetUser().ProviderUserKey);
            return View();
        }
    }
}