using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccessModel;
using AccessRepositories;
using System.Web.Routing;
using AuthService.FiferMembership;
using CompanyRepositories;
using SignalrService.Hub;
using Microsoft.AspNet.SignalR;

namespace fifer_crm.Controllers
{
    public class CallbackController : Controller
    {
        public ActionResult SupportTicketCallback(string owner, string msg)
        {
            AdminHub hub = new AdminHub();
            hub.TicketUpdated(GlobalHost.ConnectionManager.GetHubContext<AdminHub>(), Guid.Parse(owner), msg);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TaskTicketCallback(string owner, string msg)
        {
            AdminHub hub = new AdminHub();
            hub.TaskUpdated(GlobalHost.ConnectionManager.GetHubContext<AdminHub>(), Guid.Parse(owner), msg);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CallTaskCallback(string owner, string msg)
        {
            AdminHub hub = new AdminHub();
            hub.CallTaskUpdated(GlobalHost.ConnectionManager.GetHubContext<AdminHub>(), Guid.Parse(owner), msg);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MeetingTaskCallback(string owner, string msg)
        {
            AdminHub hub = new AdminHub();
            hub.MeetingTaskUpdated(GlobalHost.ConnectionManager.GetHubContext<AdminHub>(), Guid.Parse(owner), msg);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        
    }
}