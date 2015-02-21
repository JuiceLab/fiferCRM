﻿using AccessRepositories;
using CompanyRepositories;
using CRMRepositories;
using fifer_crm.Models;
using LogRepositories;
using LogService.FilterAttibute;
using NotifyEventModel;
using NotifyModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TaskModel;

namespace fifer_crm.Controllers
{
    [Authorize, CRMLogAttribute]
    public class NotifyController : BaseFiferController
    {
        NotifyRepository repository = new NotifyRepository();

        [DisplayName("Страница уведомлений")]
        public ActionResult IndexNotifies()
        {

            var model = new TaskWrapViewModel(_userId);
            model.Notifies = repository.GetNotifies(_userId);
            model.Menu = GetMenu("Уведомления");
            return View(model);
        }

        [DisplayName("Страница редакитрования уведомлений")]
        public ActionResult NotifySettingsEdit()
        {
            var userSettings = NotifySingletonWrapper.Instance.GetUserById(_userId);
            userSettings.UserId = _userId;
            return PartialView(userSettings.NotifyProfile);
        }

        [HttpPost]
        [DisplayName("Обновление настроек уведомлений")]
        public ActionResult NotifySettingsEdit(UserNotifyModel[] profile) 
        {
            var settings = new UserSettings() {  NotifyProfile = profile };
            repository.UpdateUserNotifies(settings, NotifySingletonWrapper.Instance.Users[_userId]);
            settings.RefreshUserNotifies(NotifySingletonWrapper.Instance.Host4Refresh, _userId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }


        // GET: Notify
        public ActionResult GetLastNotify()
        {
            NotifyRepository repository = new NotifyRepository();
            List<MessageViewModel> messages = repository.GetUnViewedNotifies(_userId);
            
            LocalNotifyRepository localRepository = new LocalNotifyRepository(_userId);
            messages.AddRange(localRepository.GetNotifies(_userId));

            MembershipRepository accessRepository = new MembershipRepository();
            var users = accessRepository.GetUserByIds(messages.Select(m => m.UserId).Distinct());

            StaffRepository companyRepository = new StaffRepository();
           var photos = companyRepository.GetEmployeesPhoto(_userId);
            
            foreach (var item in messages)
            {
                var user = users.FirstOrDefault(m => m.UserId == item.UserId);
                item.Title = user.FirstName + " " + user.LastName;
                if(photos.ContainsKey(item.UserId))
                    item.IconPath = photos[item.UserId];
            }
            return PartialView(messages);
        }

        public ActionResult SetLastNotifyViewed()
        {
            NotifyRepository repository = new NotifyRepository();
            repository.SetLastViewed(_userId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SetNotifyViewed(int notifyItemId, bool isLocal)
        {
            if (isLocal)
            {
                LocalNotifyRepository repository = new LocalNotifyRepository(_userId);
                repository.SetViewedNotify(notifyItemId);
            }
            else
            {
                NotifyRepository notifyRepository = new NotifyRepository();
                notifyRepository.SetViewedNotify(notifyItemId);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}