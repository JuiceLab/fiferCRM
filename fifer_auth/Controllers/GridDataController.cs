using AccessModel;
using AuthService.AuthorizeAttribute;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserContext;

namespace fifer_auth.Controllers
{
   [AuthorizeFuncRole(Profile = "Бог системы")]
    public class GridDataController : Controller
    {
        [HttpPost]
        public ActionResult GetUserGrid([DataSourceRequest] DataSourceRequest request, string funcRole)
        {
            DataSourceResult result = new DataSourceResult();
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var lastLog = context.AppCookies.OrderByDescending(m => m.CookieId)
                    .GroupBy(m => m.UserId)
                    .Select(m => new { userID = m.Key, lastLogin = m.Max(n => n.Created) })
                    .ToList();
                var user =string.IsNullOrEmpty(funcRole)?
                    context.Users
                    : context.Users.Where(m => m.UserFunctionalRoles.Any(n => n.FunctionalRole.Name == funcRole));

                result = user.ToList().OrderBy(m => m.Login).Select(m =>
                    new UserViewModel()
                    {
                        Login = m.Login,
                        FullName = m.FirstName + " " + m.LastName,
                        Mail = m.Mail,
                        Phone = m.Phone,
                        IsOnline = m.IsOnline && lastLog.Any(n => n.userID == m.UserId) &&
                            lastLog.FirstOrDefault(n => n.userID == m.UserId).lastLogin.Date.AddDays(1) >= DateTime.UtcNow,
                        UserId = m.UserId,
                        LastLogin = lastLog.Any(n => n.userID == m.UserId) ?
                            lastLog.FirstOrDefault(n => n.userID == m.UserId).lastLogin
                            : new DateTime()
                    }).ToDataSourceResult(request);
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetUserOnlineGrid([DataSourceRequest] DataSourceRequest request)
        {
            DataSourceResult result = new DataSourceResult();
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var datetime = DateTime.Now;
                var lastLog = context.AppCookies.Where(m => m.IsValid)
                    .GroupBy(m => m.UserId)
                    .Select(m => new { userID = m.Key, cookies = m.Select(n => new { app = n.Application.Description, created = n.Created, ip = n.IpAddress }), lastLogin = m.Max(n => n.Created) })
                    .ToList();
                var usersOnline = lastLog.Select(m => m.userID).ToList();
                var user = context.Users.Where(m => m.IsOnline);

                result = user.ToList().OrderBy(m => m.Login).Select(m =>
                    new UserViewModel()
                    {
                        Login = m.Login,
                        FullName = m.FirstName + " " + m.FirstName,
                        Mail = m.Mail,
                        Phone = m.Phone,
                        IsOnline = m.IsOnline && lastLog.Any(n => n.userID == m.UserId),
                        UserId = m.UserId,
                        LastLogin = lastLog.FirstOrDefault(n => n.userID == m.UserId).cookies.OrderByDescending(n => n.created).FirstOrDefault().created,
                    }).ToDataSourceResult(request);
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult GetFuncRoleGrid([DataSourceRequest] DataSourceRequest request)
        {
            List<FunctionalRole> roles = new List<FunctionalRole>();
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                roles = context.FunctionalRoles
                    .ToList()
                    .Select(m => new FunctionalRole() { RoleId = m.RoleId, Name = m.Name, IsAvailable = m.IsAvailable, Description = m.Description })
                    .ToList();
            }
            return Json(roles.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetRoleGrid([DataSourceRequest] DataSourceRequest request)
        {
            List<Role> roles = new List<Role>();
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                roles = context.Roles
                    .ToList()
                    .Select(m => new Role() { RoleId = m.RoleId, Name = m.Name, Description = m.Description })
                    .OrderBy(m => m.Name)
                    .ToList();
            }
            return Json(roles.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult GetRuleGrid([DataSourceRequest] DataSourceRequest request)
        {
            List<Rule> rules = new List<Rule>();
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                rules = context.Rules
                    .ToList()
                    .Select(m => new Rule() { RuleId = m.RuleId, Name = m.Name, RuleType = m.RuleType, Description = m.Description })
                    .ToList();
            }
            return Json(rules.ToDataSourceResult(request));
        }

    }
}