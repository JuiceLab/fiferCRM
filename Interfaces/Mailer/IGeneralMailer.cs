using Mvc.Mailer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Mailer
{
    public interface IGeneralMailer : ISimpleMailer
    {
        MvcMailMessage GeneralEmail(string body, string[] addrs, string subj);
    }
}
