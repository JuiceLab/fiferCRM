using LogContext;
using Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace LogService.FilterAttibute
{
    public class CRMLogAttribute : ActionFilterAttribute, IActionFilter, IExceptionFilter
    {
        void IActionFilter.OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                using (StreamReader reader = new StreamReader(filterContext.HttpContext.Request.InputStream))
                {
                    string text = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(text) && filterContext.ActionParameters != null)
                    {
                        StringBuilder formKey = new StringBuilder();

                        foreach (string key in filterContext.HttpContext.Request.Form)
                        {
                            formKey.Append("&" + key + "=" + filterContext.HttpContext.Request.Form[key]);
                        }

                        string form = formKey == null ? String.Empty : formKey.ToString();
                        text = string.Format("{0} {1}", string.Join("&", filterContext.ActionParameters.Select(p => p.Key + "=" + p.Value)), form);
                    }

                    var attribute = (filterContext.ActionDescriptor
                        .GetCustomAttributes(typeof(DisplayNameAttribute), false).Cast<DisplayNameAttribute>())
                        .FirstOrDefault();
                    if (Membership.GetUser() != null)
                    {
                        var action = new CRMAction()
                        {
                            Action = filterContext.ActionDescriptor.ActionName,
                            Controller = filterContext.Controller.ToString(),
                            TimeExecute = DateTime.UtcNow,
                            TimeResult = DateTime.UtcNow,
                            DurationExecuting = 0,
                            DurationResultExecuting = 0,
                            ClientTimestamp = filterContext.HttpContext.Timestamp,
                            RequestUrl = filterContext.HttpContext.Request.Url.AbsoluteUri,
                            ActionData = text,
                            UserId = (Guid)Membership.GetUser().ProviderUserKey,
                            ClientIP = HttpContext.Current.Request.UserHostAddress,
                            ActionDescription = attribute != null ? attribute.DisplayName : string.Empty,
                            ResponseCode = filterContext.HttpContext.Response.StatusCode != null ? filterContext.HttpContext.Response.StatusCode : 0,
                        };

                        if (filterContext.RouteData.Values["LogIdentifier"] == null)
                        {
                            filterContext.RouteData.Values.Add("LogIdentifier", action);
                        }
                    }
                }
            }
        }

        public void OnException(ExceptionContext filterContext)
        {
            var logItem = (CRMAction)filterContext.RouteData.Values["LogIdentifier"];
            if (logItem != null)
            {
                logItem.ResponseCode = filterContext.HttpContext.Response.StatusCode = new HttpException(null, filterContext.Exception).GetHttpCode();
                logItem.ErrorMessage = GetRecursiveInnerException(filterContext.Exception);
                ThreadPool.QueueUserWorkItem(delegate
                {
                    try
                    {
                        using (LogEntities logContext = new LogEntities())
                        {
                            var logItemSave = (CRMAction)filterContext.RouteData.Values["LogIdentifier"];

                            int maxIterations = 3;
                            CRMAction existItem = null;
                            do
                            {
                                Thread.Sleep(3000);
                                existItem = (CRMAction)logContext.CRMActions.Where(p => p.Id == logItemSave.Id);
                                maxIterations--;
                            } while (existItem == null || maxIterations > 0);
                        }
                    }
                    catch
                    {
                        if (filterContext.RouteData.Values["LogIdentifier"] != null)
                            filterContext.RouteData.Values.Remove("LogIdentifier");
                    }
                });
            }
        }

        void IActionFilter.OnActionExecuted(ActionExecutedContext filterContext)
        {
            var logItem = (CRMAction)filterContext.RouteData.Values["LogIdentifier"];
            if (logItem != null)
                logItem.DurationExecuting = (long)new TimeSpan(DateTime.UtcNow.Add(-new TimeSpan(logItem.TimeExecute.Ticks)).Ticks).TotalMilliseconds;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var logItem = (CRMAction)filterContext.RouteData.Values["LogIdentifier"];
            if (logItem != null)
                logItem.TimeResult = DateTime.UtcNow;
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                try
                {
                    using (LogEntities logContext = new LogEntities(AccessSettings.LoadSettings().LogEntites))
                    {
                        var logItem = (CRMAction)filterContext.RouteData.Values["LogIdentifier"];
                        if (logItem != null)
                        {
                            logItem.DurationResultExecuting = (long)new TimeSpan(DateTime.UtcNow.Add(-new TimeSpan(logItem.TimeResult.Value.Ticks)).Ticks).TotalMilliseconds;
                            logContext.CRMActions.Add(logItem);
                            logContext.SaveChanges();
                        }
                    }

                }
                catch
                {
                    filterContext.RouteData.Values.Remove("LogIdentifier");
                }
            });
        }

        private string GetRecursiveInnerException(Exception ex)
        {
            if (ex.InnerException != null)
                return ex.Message + " innerMsg: " + GetRecursiveInnerException(ex.InnerException);
            else
                return ex.Message + " Stack:" + ex.StackTrace;
        }
    }
}
