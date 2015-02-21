using AccessModel;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using UserContext;

namespace AuthService.FiferMembership
{
    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;

        public AccountMembershipService()
            : this(null)
        {
        }

        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public int MinPasswordLength
        {
            get
            {
                return _provider.MinRequiredPasswordLength;
            }
        }

        public bool ValidateUser(string userName, string password)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Пустовато будет.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Пустовато будет.", "password");

            return _provider.ValidateUser(userName, password);
        }

        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Пустовато будет.", "userName");
            if (String.IsNullOrEmpty(password)) throw new ArgumentException("Пустовато будет.", "password");
            if (String.IsNullOrEmpty(email)) throw new ArgumentException("Пустовато будет.", "email");

            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            if (String.IsNullOrEmpty(userName)) throw new ArgumentException("Пустовато будет.", "userName");
            if (String.IsNullOrEmpty(oldPassword)) throw new ArgumentException("Пустовато будет.", "oldPassword");
            if (String.IsNullOrEmpty(newPassword)) throw new ArgumentException("Пустовато будет.", "newPassword");

            // The underlying ChangePassword() will throw an exception rather
            // than return false in certain failure scenarios.
            try
            {
                MembershipUser currentUser = _provider.GetUser(userName, true /* userIsOnline */);
                using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
                {
                    RegisterModel model = new RegisterModel() { Pass = newPassword };
                    model.Salt = FiferMembershipProvider.CreateSalt();
                    var user = access.Users.FirstOrDefault(m => m.Login == userName);
                    user.Pass = model.HashPassword;
                    user.Salt = model.Salt;
                    access.SaveChanges();
                }
                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
            catch (MembershipPasswordException)
            {
                return false;
            }
        }
    }
}
