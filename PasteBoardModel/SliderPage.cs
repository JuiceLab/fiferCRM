using Interfaces.ConstructorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteBoardModel
{
    public class SliderPage : ISliderPage
    {
        public SliderPage(IPage page)
        {
            PageId = page.PageId;
            UserId = page.UserId;
            SiteId = page.SiteId;
            IsSocialRepostItems = page.IsSocialRepostItems;
            H1 = page.H1;
            Content = page.Content;
            CssPostfix = page.CssPostfix;
            LayoutName = page.LayoutName;
            BackgroundImage = page.BackgroundImage;
            IsHtml = page.IsHtml;
            Header = page.Header;
            Footer = page.Footer;
            SeoItem = page.SeoItem;
            Menu = page.Menu;

        }

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

        public IList<ISliderItem> SliderItems { get; set; }

        
    }
}
