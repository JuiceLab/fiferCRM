using Microsoft.Owin;
using NotifyEventModel;
using NotifyEventModel.Binder;
using NotifyModel;
using NotifyService.EventHandlers;
using NotifyService.Observers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

[assembly: OwinStartup(typeof(fifer_crm.Startup))]
namespace fifer_crm
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            ModelBinders.Binders.Add(typeof(UserSettings), new UserSettingsBinder());
            
            var item = NotifySingletonWrapper.Instance;
            TicketEventsObserver.Instance.Observers = new List<IObserver<NotifyObserveItem>>();

            NotifyObserver monitor = new NotifyObserver();
            monitor.Subscribe(TicketEventsObserver.Instance);
        }
    }
}
