using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface INoticesPage : IPage
    {
        List<INoticeItem> Notices { get; set; }
    }
}
