using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserContext;

namespace AuthService.FiferMembership
{
    public class RemoteMembershipService
    {
        public static string CheckRemoteAccess(Guid remoteToken, string ip)
        {
            string userName = string.Empty;
            using (UserEntities entities = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var user = entities.Users.FirstOrDefault(m => m.RemoteToken == remoteToken);
                if (user != null && user.RemoteToken.HasValue && (string.IsNullOrEmpty(user.AvailableIps) || user.AvailableIps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Contains(ip)))
                {
                    userName = user.Login;
                    user.RemoteToken = Guid.NewGuid();
                    entities.SaveChanges();
                }
            }
            return userName;
        }

        public static bool IsIpPass(string userName, string ip)
        {
            bool result = true;
            using (UserEntities entities = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var user = entities.Users.FirstOrDefault(m => m.Login == userName);
                 result =  string.IsNullOrEmpty(user.AvailableIps) 
                     || user.AvailableIps.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Contains(ip);
            }
            return result;
        }


        public static Guid? TokenPass(string userName)
        {
            Guid? result = null;
            using (UserEntities entities = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var user = entities.Users.FirstOrDefault(m => m.Login == userName);
                result = user.RemoteToken;
            }
            return result;
        }
    }
}
