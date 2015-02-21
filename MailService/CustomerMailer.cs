using EnumHelper.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnumHelper;
using AccessRepositories;

namespace MailService
{
    public class CustomerMailer: GeneralMailer
    {
        public CustomerMailer()
            : base()
        {
            _codePrefix = "CUS";
        }

        public override string DoNotify<T>(Guid objId, string bodyText, T mailType)
        {
            string result = string.Empty;
            if (typeof(T).IsEnum)
            {
                switch ((byte)(object)mailType)
                {
                    case (byte)CustomerMail.Feedback:
                        UpdateMailName(CustomerMail.Feedback.GetStringValue());
                        break;
                    default: break;
                }
                MembershipRepository repository = new MembershipRepository();
                var user = repository.GetUserById(objId);
                result = SendBodyText(new string[] { user.Mail }, "Fifer-Site: " + CustomerMail.Feedback.GetStringValue(), bodyText);
            }
            return result;
        }
    }
}
