using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface IPage
    {
        int PageId { get; set; }
        Guid? UserId { get; set; }
        Guid SiteId { get; set; }
        bool IsSocialRepostItems { get; set; }
        bool IsHtml { get; set; }
        string BackgroundImage { get; set; }
        string H1 { get; set; }
        string Content { get; set; }
        string LayoutName { get; set; }
        string CssPostfix { get; set; }
        ISiteHeader Header { get; set; }
        ISiteFooter Footer { get; set; }
        ISeoItem SeoItem { get; set; }
        IList<ISiteMenuItem> Menu { get; set; }
    }
}
