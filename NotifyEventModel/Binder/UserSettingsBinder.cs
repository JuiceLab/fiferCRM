using LogContext;
using LogRepositories;
using NotifyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace NotifyEventModel.Binder
{
    public class UserSettingsBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var user = Membership.GetUser();
            if (user == null)
                return new UserSettings();
            var userId = user.ProviderUserKey.ToString();

            UserSettings model = controllerContext.HttpContext.Session["UserSettings"] != null ?
                 (UserSettings)controllerContext.HttpContext.Session["UserSettings"]
                 : GetSettings((Guid)user.ProviderUserKey);

            controllerContext.HttpContext.Session["UserSettings"] = model;

            return model;
        }

        private UserSettings GetSettings(Guid userId)
        {
            NotifyRepository repository = new NotifyRepository();
            IEnumerable<Notify4Listener> notifies =  repository.GetUserNotifies(userId);

            UserSettings model = new UserSettings()
            {
                ListenerId = notifies.FirstOrDefault().C_ListenerId,
                NotifyProfile = notifies.Select(m => new UserNotifyModel
                {
                    IsActive = m.IsAvailable,
                    NotifyId = m.C_NotifyId
                }).ToArray()
            };

            return model;
        }
    }
}
