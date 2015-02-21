using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Mailer
{
    public interface ISimpleMailer
    {
        int MailRegisterId { get; set; }
        string RegisterCode { get; set; }
        string DoNotify<T>(Guid objId, string bodyText, T mailType) where T : struct, IConvertible;
     }
}
