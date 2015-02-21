using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface INoticeItem
    {
        int PageId { get; set; }
        int PositionId { get; set; }
        string Title { get; set; }
        string NoticeImagePath { get; set; }
        string NoticeContent { get; set; }
        string UrlRedirect { get; set; }
    }
}
