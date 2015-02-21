using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserContext;
using EntityRepository;
using AuthService.AuthorizeAttribute;
using Settings;

namespace fifer_auth.Controllers
{
   [AuthorizeFuncRole(Profile = "Бог системы")]
    public class RuleController : Controller
    {
        [HttpPost]
        public ActionResult IndexRule(Rule rule)
        {
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                context.InsertUnit(rule);
            }
            return RedirectToAction("IndexRules", "Home");
        }

        public ActionResult RuleModal(int? ruleId)
        {
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                ViewBag.Rules = context.Rules
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = m.Name,
                        Value = m.RuleId.ToString()
                    }).ToList();
            }
            return PartialView();
        }

        public ActionResult Rule4RoleModal(int roleId)
        {
            using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                ViewBag.Rules = access.Rules
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = m.Name,
                        Value = m.RuleId.ToString()
                    })
                    .ToList();
            }
            return PartialView(new Rule4Role() { RoleId = roleId});
        }

        [HttpPost]
        public ActionResult Rule4Role(Rule4Role model)
        {
            using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                access.InsertUnit(model);
            }
            return RedirectToAction("IndexRole", "Role", new { roleId = model.RoleId });
        }

        public ActionResult Rules4Role(int roleId)
        {
            var model = new List<Rule4Role>();
            ViewBag.RoleId = roleId;
            using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var rules =access.Roles.FirstOrDefault(m => m.RoleId == roleId).Rule4Role;
                model = rules.ToList();
                ViewBag.Names = model
                    .Select(m => new SelectListItem() { Text = m.Rule.Name, Value = m.RuleId.ToString() })
                    .ToList();
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult RulesInRole(int roleId, List<Rule4Role> items)
        {
            return RedirectToAction("IndexRole", "Role", new { roleId = roleId });
        }

    }
}