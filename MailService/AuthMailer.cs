using EnumHelper.Mailer;
using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumHelper;
using AccessRepositories;

namespace MailService
{
    public class AuthMailer : GeneralMailer
    {
        public AuthMailer()
            : base()
        {
            _codePrefix = "REG";
        }

        public override string DoNotify<T>(Guid objId, string bodyText, T mailType)
        {
            string result = string.Empty;
            if (typeof(T).IsEnum)
            {
                switch ((byte)(object)mailType)
                {
                    case (byte)AuthMail.NewCustomer:
                        UpdateMailName(AuthMail.NewCustomer.GetStringValue());
                        break;
                    case (byte)AuthMail.NewEmployee:
                        UpdateMailName(AuthMail.NewEmployee.GetStringValue());
                        break;
                    case (byte)AuthMail.ResetPass:
                        UpdateMailName(AuthMail.ResetPass.GetStringValue());
                        break;
                    default: break;
                }
                MembershipRepository repository = new MembershipRepository();
                var user = repository.GetUserById(objId);
                result = SendBodyText(new string[] { user.Mail }, "Fifer-CRM: " + AuthMail.NewCustomer.GetStringValue(), bodyText);
            }
            return result;
        }
    }
}
