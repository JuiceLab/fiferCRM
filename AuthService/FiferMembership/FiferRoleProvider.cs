using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using UserContext;
using System.Data.Entity;
using System.Data.Linq;
using Settings;

namespace AuthService.FiferMembership
{
    public class FiferRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            UserEntities _dbAccess = new UserEntities(AccessSettings.LoadSettings().UserEntites);
            return _dbAccess.Users.FirstOrDefault(m => m.Login == username)
                .UserRoles
                .Any(m => m.Role.Name == roleName && m.IsAvailable);
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            UserEntities _dbAccess = new UserEntities(AccessSettings.LoadSettings().UserEntites);
            return _dbAccess.Roles.Select(m => m.Name).ToArray();
        }

        public override string[] GetRolesForUser(string login)
        {
            UserEntities _dbAccess = new UserEntities(AccessSettings.LoadSettings().UserEntites);
            return _dbAccess.Users.FirstOrDefault(m => m.Login == login)
                .UserRoles
                .Where(m => m.IsAvailable)
                .Select(m => m.Role.Name)
                .Distinct()
                .ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}
