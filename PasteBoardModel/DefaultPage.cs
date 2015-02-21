using CompanyContext;
using Interfaces.ConstructorBase;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBoardModel
{
    public class DefaultPage : IPage
    {
        public int PageId { get; set; }
        public Guid? UserId { get; set; }
        public Guid SiteId { get; set; }

        public bool IsSocialRepostItems { get; set; }

        public bool IsHtml { get; set; }

        public string BackgroundImage { get; set; }

        public string H1 { get; set; }

        public string Content { get; set; }

        public string LayoutName { get; set; }

        public string CssPostfix { get; set; }

        public ISiteHeader Header { get; set; }

        public ISiteFooter Footer { get; set; }

        public ISeoItem SeoItem { get; set; }

        public IList<ISiteMenuItem> Menu { get; set; }

    }
}
