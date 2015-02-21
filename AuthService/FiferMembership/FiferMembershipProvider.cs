using AccessModel;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using UserContext;

namespace AuthService.FiferMembership
{
    public class FiferMembershipProvider: MembershipProvider
    {
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

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != string.Empty)
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var user = GetUser(username, true);

            if (user == null)
            {
                var userObj = new User { Login = username, Pass = GetMd5Hash(password), Mail = email };

                using (var usersContext = new UserEntities(AccessSettings.LoadSettings().UserEntites))
                {
                    usersContext.Users.Add(userObj);
                    usersContext.SaveChanges();
                }

                status = MembershipCreateStatus.Success;

                return GetUser(username, true);
            }
            status = MembershipCreateStatus.DuplicateUserName;

            return null;
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            throw new NotImplementedException();
        }

        public override bool EnablePasswordReset
        {
            get { throw new NotImplementedException(); }
        }

        public override bool EnablePasswordRetrieval
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var usersContext = new UserEntities(AccessSettings.LoadSettings().UserEntites);
            MembershipUserCollection result = new MembershipUserCollection();
            var items = usersContext.Users.ToList().Select(m => new MembershipUser("FiferMembershipProvider", m.Login, m.UserId, m.Mail,
                                                            string.Empty, string.Empty,
                                                            true, false, DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.Now, DateTime.Now));
            foreach (var item in items)
            {
                result.Add(item);
            };
            totalRecords = items.Count();
            return result;
        }

        public override int GetNumberOfUsersOnline()
        {
            using (var usersContext = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                return usersContext.Users.Where(m => m.IsOnline).Count();
            }
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            var usersContext = new UserEntities(AccessSettings.LoadSettings().UserEntites);
            var user = usersContext.Users.FirstOrDefault(m => m.Login == username);
            if (user != null)
            {
                var memUser = new MembershipUser("FiferMembershipProvider", username, user.UserId, user.Mail,
                                                            string.Empty, string.Empty,
                                                            true, false, DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.Now, DateTime.Now);
                return memUser;
            }
            return null;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            var usersContext = new UserEntities(AccessSettings.LoadSettings().UserEntites);
            Guid id = (Guid)providerUserKey;
            var user = usersContext.Users.FirstOrDefault(m => m.UserId == id);
            if (user != null)
            {
                var memUser = new MembershipUser("FiferMembershipProvider", user.Login, user.UserId, user.Mail,
                                                            string.Empty, string.Empty,
                                                            true, false, DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.MinValue,
                                                            DateTime.Now, DateTime.Now);
                return memUser;
            }
            //else
            //{
            //    var app = usersContext.Applications.FirstOrDefault(m => m.AppGuid == id);
            //    if (app != null)
            //    {
            //        var memApp = new MembershipUser("FiferMembershipProvider", app.Description, app.AppGuid, app.AppUrl,
            //                                                                   string.Empty, string.Empty,
            //                                                                   true, false, DateTime.MinValue,
            //                                                                   DateTime.MinValue,
            //                                                                   DateTime.MinValue,
            //                                                                   DateTime.Now, DateTime.Now);
            //        return memApp;
            //    }
            //}
            return null;
        }

        public override string GetUserNameByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 10; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { throw new NotImplementedException(); }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { throw new NotImplementedException(); }
        }

        public override string PasswordStrengthRegularExpression
        {
            get {return  string.Empty; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return false; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotImplementedException();
        }

        public override bool ValidateUser(string username, string password)
        {

            var md5Hash = GetMd5Hash(password);
            using (var usersContext = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var requiredUser = usersContext.Users.FirstOrDefault(m => m.Login == username && !m.IsBlocked);
                if (requiredUser != null)
                {
                    RegisterModel model = new RegisterModel()
                    {
                        Salt = requiredUser.Salt,
                        Pass = password
                    };
                    return (model.HashPassword == requiredUser.Pass);
                }
                else
                    return false;
            }
        }

        public static string GetMd5Hash(string value)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(value);
            bs = x.ComputeHash(bs);
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToLower());
            }
            string password = s.ToString();
            return password;
        }

        public static string CreateSalt()
        {
            Random rnd = new Random();
            int size = rnd.Next(4, 10);
            //Generate a cryptographic random number.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[size];
            rng.GetBytes(buff);

            // Return a Base64 string representation of the random number.
            return Convert.ToBase64String(buff);
        }
    }
}
