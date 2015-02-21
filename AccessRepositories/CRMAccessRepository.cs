using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using UserContext;
using System.Web.Mvc;
using CompanyModel;

namespace AccessRepositories
{
    public class CRMAccessRepository : BaseAccessRepository, IDisposable
    {
        MembershipRepository _repository = new MembershipRepository();
        public CRMAccessRepository() 
            : base() { 
        }

        public void UpdateUserFuncRole(Guid userId, string funcRoleName, bool isEnable)
        {
            var role = Context.FunctionalRoles.FirstOrDefault(m => m.Name == funcRoleName);
            if (role != null)
            {
                var user = Context.GetUnitById<User>(userId);
                if (user.UserFunctionalRoles.Any(m => m.C_FunctionalRoleId == role.RoleId))
                {
                    user.UserFunctionalRoles
                        .FirstOrDefault(m => m.C_FunctionalRoleId == role.RoleId)
                        .IsAvailable = isEnable;
                    Context.SaveChanges();
                }
                else
                    Context.InsertUnit(new UserFunctionalRole()
                    {
                        C_UserId = user.UserId,
                        C_FunctionalRoleId = role.RoleId,
                        IsAvailable = isEnable
                    });
                if (isEnable)
                    _repository.UpdateUserFuncRoleAccess(userId, new List<int>() { role.RoleId });
            }
        }

        public void SetFuncRole(Guid userId, int funcRoleId)
        {
            var user = Context.GetUnitById<User>(userId);
            var isExistRole = false;
            foreach (var item in user.UserFunctionalRoles.Where(m => m.FunctionalRole.IsCRM))
            {
                if (item.C_FunctionalRoleId == funcRoleId)
                {
                    item.IsAvailable = true;
                    isExistRole = true;
                }
                else
                    item.IsAvailable = false;
            }
            if (!isExistRole)
                Context.UserFunctionalRoles.Add(
                    new UserFunctionalRole()
                    {
                        IsAvailable = true,
                        C_UserId = userId,
                        C_FunctionalRoleId = funcRoleId
                    });
            Context.SaveChanges();
        }

        public List<SelectListItem> GetFuncRoles4Employee(Guid userId)
        {
            var currentRoleId = userId == Guid.Empty 
                || !Context.Users.Any(m=>m.UserId == userId) ? 
                0 : Context.GetUnitById<User>(userId).UserFunctionalRoles.FirstOrDefault(m=>m.FunctionalRole.IsCRM).C_FunctionalRoleId;
            var model = Context.FunctionalRoles
                 .Where(m => m.IsCRM)
                 .ToList()
                 .Select(m => new SelectListItem()
                 {
                     Text = m.Name,
                     Value = m.RoleId.ToString(),
                     Selected = m.RoleId == currentRoleId
                 }).ToList();
            model.Insert(0, new SelectListItem() { Text = "Укажите уровень доступа сотрудника" });
            return model;
        }

        public void UpdateUserContact(EmployeeViewModel model)
        {
            var user  = Context.GetUnitById<User>(model.UserId);
            model.Phone = user.Phone;
            model.Mail = user.Mail;
        }
    }
}
