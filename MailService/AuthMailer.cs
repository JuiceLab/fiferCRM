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
                UpdateMailName(((AuthMail)(object)mailType).GetStringValue());
                MembershipRepository repository = new MembershipRepository();
                var user = repository.GetUserById(objId);
                result = SendBodyText(new string[] { user.Mail }, "Fifer-CRM: " + ((AuthMail)(object)mailType).GetStringValue(), bodyText);
            }
            return result;
        }
    }
}
