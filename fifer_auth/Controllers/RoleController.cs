using AccessRepositories;
using AuthService.AuthorizeAttribute;
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
    public class RoleController : Controller
    {
        MembershipRepository _repository = new MembershipRepository();

        public ActionResult IndexRole(int roleId)
        {
            Role role = new Role();
            using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                role = access.Roles.FirstOrDefault(m => m.RoleId == roleId);
                ViewBag.Users = role.UserRoles.Select(m => m.User).ToList();
            }
            return View(role);
        }

        [HttpPost]
        public ActionResult IndexRole(Role role)
        {
            using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                access.Roles.Add(role);
                access.SaveChanges();
            }
            return RedirectToAction("IndexRole", new { roleId = role.RoleId });
        }

        [HttpPost]
        public ActionResult UserInRole(int roleId, List<string> usersInRole)
        {
            _repository.UpdateRoleUserAccess(roleId, usersInRole.Select(m => Guid.Parse(m)));
            return RedirectToAction("IndexRole", new { roleId = roleId });
        }

        public ActionResult IndexFuncRole(int roleId)
        {
            FunctionalRole role = new FunctionalRole();
            using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                ViewBag.Role = access.FunctionalRoles.FirstOrDefault(m => m.RoleId == roleId);
            }
            List<SelectListItem> result  =  _repository.GetRoles4FuncRoles(roleId);
            return View(result);
        }

        [HttpPost]
        public ActionResult IndexFuncRole(FunctionalRole role)
        {
            using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                role.IsAvailable = true;
                access.FunctionalRoles.Add(role);
                access.SaveChanges();
            }
            return RedirectToAction("IndexFuncRole", new { roleId = role.RoleId });
        }

        public ActionResult Role4FuncRole(int roleId, List<int> roles)
        {
            _repository.UpdateFuncRoleProfile(roleId, roles);
            return RedirectToAction("IndexFuncRole", new { roleId = roleId });
        }

        public ActionResult RoleModal()
        {
            return PartialView(new Role());
        }

        public ActionResult FuncRoleModal()
        {
            return PartialView(new FunctionalRole());
        }

        public ActionResult FuncRoleByUser(Guid userId)
        {
            ViewBag.User = userId;

            List<SelectListItem> result = _repository.GetFuncRoles(userId);
            return PartialView(result);
        }
                        
        [HttpPost]
        public ActionResult FuncRoleByUser(Guid userId, List<int> roles)
        {
            _repository.UpdateUserFuncRoleAccess(userId, roles);
            return RedirectToAction("Index", "User", new { userId = userId });
        }

        public ActionResult RoleByUser(Guid userId)
        {
            ViewBag.User = userId;

            List<SelectListItem> result = _repository.GetRoles(userId);
            return PartialView(result);
        }

        [HttpPost]
        public ActionResult RoleByUser(Guid userId, List<int> roles)
        {
            _repository.UpdateUserRoleAccess(userId, roles);
            return RedirectToAction("Index", "User", new { userId = userId });
        }

        public ActionResult RulesInRole(int roleId, List<Rule4Role> relations)
        {
            _repository.UpdateRuleRelations(roleId, relations);
            return RedirectToAction("IndexRole", new { roleId = roleId });
        }

    }
}