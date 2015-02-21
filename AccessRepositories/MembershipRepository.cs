using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using UserContext;
using EntityRepository;
using AccessModel;
using AuthService.FiferMembership;
using CompanyModel;
using Settings;
using ExtensionHelpers;

namespace AccessRepositories
{
    public class MembershipRepository : BaseAccessRepository, IDisposable
    {
        public MembershipRepository()
            : base() { }

        public string GeneratePass(int length)
        { 
            string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            string res = "";
            Random rnd = new Random();
            while (0 < length--)
                res += valid[rnd.Next(valid.Length)];
            return res;
        }

        public bool IsExistLogin(string login)
        {
            return !Context.Users.Any(x => x.Login == login);
        }

        public User CreateUser(CompanyRegisterModel model, out string pass)
        {
            pass =  GeneratePass(6);
            var login = GenerateLogin(model.FirstName, model.LastName, model.CompanyName);
            return CreateUser(new RegisterModel()
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Login = login,
                Pass = pass,
                Phone = model.Phone,
                Pass_repeat = pass
            });
        }

        private string GenerateLogin(string firstName, string lastName, string company)
        {
            var trFirstName =  TransliterationHelper.Front(firstName);
            var trlastName = TransliterationHelper.Front(lastName);
            var trCompany = TransliterationHelper.Front(company);
            var loginName = trCompany.Substring(0, 5) + trFirstName.Substring(0, 1).ToUpper() + trlastName.Substring(0, 1).ToUpper();
            if (Context.Users.Any(m => m.Login == loginName))
            {
                int index = 1;
                var tmpLogin = loginName + index;
                while (Context.Users.Any(m => m.Login == tmpLogin))
                {
                    index++;
                    tmpLogin = loginName + index;
                }
                loginName = tmpLogin;
            }
            return loginName;
        }


        public User CreateUser(RegisterModel register)
        {
            var result = Guid.Empty;
            User user = new User();
            if (IsExistLogin(register.Login))
            {
                register.Salt = FiferMembershipProvider.CreateSalt();
                user = new User()
                {
                    UserId = Guid.NewGuid(),
                    Login = register.Login,
                    Phone = register.Phone,
                    FirstName = register.FirstName,
                    LastName = register.LastName,
                    Pass = register.HashPassword,
                    Mail = register.Email,
                    Salt = register.Salt
                };
                Context.Users.Add(user);
                Context.SaveChanges();

                result = user.UserId;
                //MailEnvelope model = new MailEnvelope()
                //{
                //    Subj = "ЭМПРАНА. Приветствуем нового пользователя ",
                //    Body = string.Format("Уважаемый пользователь! Ваши данные для авторизации. Логин:{0}, Пароль:{1}", user.Login, register.Pass),
                //    Recipients = new RecipientList() { Emails = new string[] { user.Mail } },
                //    HiddenRecipients = new RecipientList() { Emails = new string[] { user.Mail } },
                //    Template = null,
                //    TemplateType = null
                //};
                //ApiSupportHepler.GetApiRequest<MailEnvelope, MailEnvelope>(model, "Messenger.Mail");
                //return RedirectToAction("Index", "User",  new { userId = user.UserId});
            }
            return user;
        }

        public Guid CreateEmployeeUser(EmployeeRegisterModel register)
        {
             var result = Guid.Empty;
             if (IsExistLogin(register.Login))
             {
                 register.Salt = FiferMembershipProvider.CreateSalt();
                 User user = new User()
                 {
                     UserId = Guid.NewGuid(),
                     Login = register.Login,
                     FirstName = register.FirstName,
                     LastName = register.LastName,
                     Pass = register.HashPassword,
                     Mail = register.Email,
                     Phone = register.Phone,
                     Salt = register.Salt
                 };
                 Context.Users.Add(user);
                 Context.SaveChanges();

                 result = user.UserId;
             }
             return result;
        }

        public void UpdateUser(User user)
        {
            Context.UpdateUnit(user);
        }

        public void SetLogout(Guid userId)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            string content = string.Empty;
            var userMembership = Context.Users.FirstOrDefault(m => m.UserId == userId);
            //var rules = Context.UserProfiles
            //    .Where(m => m.C_UserId == userId)
            //    .SelectMany(m => m.Role.Rule4Role.Select(n => n.RuleId)).ToList();
            userMembership.IsOnline = false;

            var currentAuth = Context.AuthStats.FirstOrDefault(m => m.UserId == userId && m.StatusAuth);
            if (currentAuth != null)
            {
                currentAuth.LogoutDate = DateTime.UtcNow;
                currentAuth.StatusAuth = false;
            }
            Context.SaveChanges();
        }

        public void UpdateFuncRoleProfile(int roleId, List<int> baseRoleIds)
        {
            var role = Context.FunctionalRoles.FirstOrDefault(m => m.RoleId == roleId);
            if (baseRoleIds!=null && baseRoleIds.Count > 0)
                role.IncludeRoles= string.Join(",", baseRoleIds);
            else
                role.IncludeRoles = string.Empty;
            Context.SaveChanges();
           var userInRoleIds =  Context.UserFunctionalRoles.Where(m=>m.C_FunctionalRoleId == roleId).Select(m=>m.C_UserId).ToList();
           Parallel.ForEach(userInRoleIds, (id) =>
           {
               using (MembershipRepository repository = new MembershipRepository())
               {
                   repository.UpdateUserFuncRoleAccess(id, new List<int>() { roleId });
               }
           });
        }

        public void UpdateRoleRules(int roleId, List<int> rules)
        {
            var availableRules = Context.Rule4Role.Where(m => m.RoleId == roleId);
            if (rules != null)
            {
                var disableRules = availableRules.Where(m => !rules.Contains(m.RuleId));
                foreach (var item in disableRules)
                {
                    item.IsAvailable = false;
                }
                var existRules = availableRules.Where(m => rules.Contains(m.RuleId));
                foreach (var item in existRules)
                {
                    item.IsAvailable = true;
                }
                var noveltyRules = rules.Except(availableRules.Select(m => m.RuleId));
                foreach (var item in noveltyRules)
                {
                    Context.Rule4Role.Add(new Rule4Role()
                    {
                        RoleId = roleId,
                        RuleId = item,
                        IsAvailable = true
                    });
                }
            }
            else
                foreach (var item in availableRules)
                {
                    item.IsAvailable = false;
                }
            Context.SaveChanges();
        }

        public void UpdateRoleUserAccess(int roleId, IEnumerable<Guid> users)
        {
            var availableUsers = Context.UserRoles.Where(m => m.C_RoleId == roleId);
            if (availableUsers != null)
            {
                if (users == null || users.Count() == 0)
                {
                    foreach (var item in availableUsers)
                    {
                        item.IsAvailable = false;
                    }
                }
                else
                {
                    var disableRoles = availableUsers.Where(m => !users.Contains(m.C_UserId));
                    foreach (var item in disableRoles)
                    {
                        item.IsAvailable = false;
                    }
                    var existRoles = availableUsers.Where(m => users.Contains(m.C_UserId));
                    foreach (var item in existRoles)
                    {
                        item.IsAvailable = true;
                    }
                    var noveltyUsers = users.Except(availableUsers.Select(m => m.C_UserId));
                    foreach (var userId in noveltyUsers)
                    {
                        Context.UserRoles.Add(new UserRole()
                        {
                            C_UserId = userId,
                            C_RoleId = roleId,
                            IsAvailable = true
                        });
                    }
                }
            }
            else
                foreach (var item in availableUsers)
                {
                    item.IsAvailable = false;
                }
            Context.SaveChanges();
        }
       
        public void UpdateUserRoleAccess(Guid userId, List<int> roles, bool onlyActive = false)
        {
            var availableRoles = Context.UserRoles.Where(m => m.C_UserId == userId);
            if (roles != null)
            {
                if (!onlyActive)
                {
                    var disableRoles = availableRoles.Where(m => !roles.Contains(m.C_RoleId));
                    foreach (var item in disableRoles)
                    {
                        item.IsAvailable = false;
                    }
                }
                var existRoles = availableRoles.Where(m => roles.Contains(m.C_RoleId));
                foreach (var item in existRoles)
                {
                    item.IsAvailable = true;
                }
                var noveltyRoles = roles.Except(availableRoles.Select(m => m.C_RoleId));
                foreach (var item in noveltyRoles)
                {
                    Context.UserRoles.Add(new UserRole()
                    {
                        C_UserId = userId,
                        C_RoleId = item,
                        IsAvailable = true
                    });
                }
            }
            else
                foreach (var item in availableRoles)
                {
                    item.IsAvailable = false;
                }
            Context.SaveChanges();
        }

        public void UpdateUserFuncRoleAccess(Guid userId, List<int> roles)
        {
            var availableRoles = Context.UserFunctionalRoles.Where(m => m.C_UserId == userId);
            if (roles != null)
            {
                var disableRoles = availableRoles.Where(m => !roles.Contains(m.C_FunctionalRoleId));
                foreach (var item in disableRoles)
                {
                    item.IsAvailable = false;
                }
                var existRoles = availableRoles.Where(m => roles.Contains(m.C_FunctionalRoleId));
                foreach (var item in existRoles)
                {
                    item.IsAvailable = true;
                }
                var noveltyRoles = roles.Except(availableRoles.Select(m => m.C_FunctionalRoleId));
                foreach (var item in noveltyRoles)
                {
                    Context.UserFunctionalRoles.Add(new UserFunctionalRole()
                    {
                        C_UserId = userId,
                        C_FunctionalRoleId = item,
                        IsAvailable = true
                    });
                }
            }
            else
                foreach (var item in availableRoles)
                {
                    item.IsAvailable = false;
                }
            Context.SaveChanges();

            var profiles = Context.UserFunctionalRoles.Where(m => m.C_UserId == userId && m.IsAvailable).Select(m => m.FunctionalRole.IncludeRoles).ToList();
            var roleIds = string.Join(",", profiles.Where(m => !string.IsNullOrEmpty(m))).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(m => int.Parse(m))
                                    .Distinct()
                                    .ToList();

            //var roleIds = Context.Roles
            //    .Where(m => profileIds.Contains(m.RoleId))
            //    .SelectMany(m => m.Rule4Role)
            //    .Select(n => n.RoleId)
            //    .Distinct()
            //    .ToList();
            UpdateUserRoleAccess(userId, roleIds, true);
        }

        public User GetUserById(Guid id)
        {
            return Context.GetUnitById<User>(id);
        }

        public IEnumerable<User> GetUserByIds(IEnumerable<Guid> ids)
        {
            var users = Context.Users
                 .Where(m => ids.Contains(m.UserId))
                 .Select(m => new { id = m.UserId, login = m.Login, firstName = m.FirstName, lastName = m.LastName })
                 .ToList()
                 .Select(m => new User() { UserId = m.id, Login = m.login, FirstName = m.firstName, LastName = m.lastName })
                 .ToList();
           users.Add(new User() { UserId = Guid.Empty, Login = "Система" });
            return users;
        }

        public IEnumerable<User> GetAllUsers4FunctionalRole(string role)
        {
            return Context.Users.Where(m => m.UserFunctionalRoles.Any(n => n.FunctionalRole.Name == role && n.IsAvailable)).ToList();
        }

        public IEnumerable<SelectListItem> GetUser4FunctionalRoles(params string[] roles)
        {
            var selectList = Context.Users.AsQueryable();
            List<SelectListItem> funcRoleUsers = new List<SelectListItem>();
            Random rnd = new Random();
            foreach (var item in roles)
            {
                var users = selectList.Where(m => m.UserFunctionalRoles
                                                   .Any(n => n.FunctionalRole.Name == item && !n.User.IsBlocked && n.IsAvailable && n.FunctionalRole.IsAvailable)).ToList();
                if (users.Count > 0)
                    funcRoleUsers.Add(new SelectListItem() { Text = item, Value = users[rnd.Next(0, users.Count - 1)].UserId.ToString() });
                else
                    funcRoleUsers.Add(new SelectListItem() { Text = "Ответственный специалист не назначен", Value = Guid.Empty.ToString() });
            }
            return funcRoleUsers;
        }

        public bool IsUser4FunctionRole(Guid user, string role)
        {
            return Context.GetUnitById<User>(user)
                .UserFunctionalRoles
                .Any(m => m.FunctionalRole.Name == role && m.IsAvailable);
        }

        public IEnumerable<User> GetUser4Roles(params string[] roles)
        {
            var selectList = Context.Users.AsQueryable();
            List<User> roleUsers = new List<User>();
            Random rnd = new Random();
            foreach (var item in roles)
            {
                var users = selectList.Where(m => m.UserRoles
                                                   .Any(n => n.Role.Name == item && n.IsAvailable) 
                                                   && !m.IsBlocked)
                                                   .ToList();
                if (users.Count > 0)
                    roleUsers.AddRange(users);
            }
            return roleUsers;
        }

        public static string GetUserPhone(Guid userId)
        {
            var phone = string.Empty;
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                phone = context.GetUnitById<User>(userId).Phone;
            }
            return phone;
        }

        public static string GetUniqueKey(int length)
        {
            string guidResult = string.Empty;

            while (guidResult.Length < length)
            {
                // Get the GUID.
                guidResult += Guid.NewGuid().ToString().GetHashCode().ToString("x");
            }

            // Make sure length is valid.
            if (length <= 0 || length > guidResult.Length)
                throw new ArgumentException("Length must be between 1 and " + guidResult.Length);

            // Return the first length bytes.
            return guidResult.Substring(0, length);
        }

        public string GetLastLog(Guid userId)
        {
             return Context.AppCookies
                   .Any(m => m.UserId == userId) ? Context.AppCookies
                   .Where(m => m.UserId == userId)
                   .OrderByDescending(m => m.CookieId)
                   .FirstOrDefault().Created.ToString()
                   : "Никогда";
        }


        public List<SelectListItem> GetFuncRoles(Guid userId)
        {
            return new List<SelectListItem>(Context.FunctionalRoles
                   .ToList()
                   .Select(m => new SelectListItem()
                   {
                       Text = m.Name,
                       Value = m.RoleId.ToString(),
                       Selected = m.UserFunctionalRoles.Any(n => n.C_UserId == userId && n.IsAvailable)
                   }));
        }

        public List<SelectListItem> GetRoles(Guid userId)
        {
           return new List<SelectListItem>(Context.Roles
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = m.Name,
                        Value = m.RoleId.ToString(),
                        Selected = m.UserRoles.Any(n => n.C_UserId == userId && n.IsAvailable)
                    }));
        }

        public List<SelectListItem> GetRoles4FuncRoles(int roleId)
        {
            var funcRole = Context.GetUnitById<FunctionalRole>(roleId);
            var existRoles = !string.IsNullOrEmpty(funcRole.IncludeRoles)?
                funcRole.IncludeRoles
                        .Split(',')
                        .Select(m=>int.Parse(m.Trim()))
                        .ToList()
                : new List<int>();
            return new List<SelectListItem>(Context.Roles
                   .ToList()
                   .Select(m => new SelectListItem()
                   {
                       Text = m.Name,
                       Value = m.RoleId.ToString(),
                       Selected = existRoles.Any(n => n == m.RoleId)
                   }));
        }


        public void UpdateRuleRelations(int roleId, List<Rule4Role> relations)
        {
            foreach (var item in relations)
            {
                Context.UpdateUnit(item);
            }
        }

        public void UpdateUser(EmployeeViewModel employee)
        {
            var existUser = Context.GetUnitById<User>(employee.UserId);
            existUser.FirstName = employee.FirstName;
            existUser.LastName = employee.LastName;
            existUser.IsBlocked = employee.IsDismissed;
            existUser.Mail = employee.Mail;
            existUser.Phone = employee.Phone;
            Context.SaveChanges();
        }
    }
}
