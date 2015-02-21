using Interfaces.ConstructorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteModel
{
    public class NoticeItem : INoticeItem
    {

        public int PageId { get; set; }
        public int PositionId { get; set; }

        public string Title { get; set; }

        public string NoticeImagePath { get; set; }

        public string NoticeContent { get; set; }

        public string UrlRedirect { get; set; }
    }
}
