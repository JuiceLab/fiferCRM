using Interfaces.ConstructorBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteModel
{
    public class SiteSeoItem : ISeoItem
    {
        public int SeoItemId { get; set; }

        public string Title { get; set; }

        public string Keywords { get; set; }

        public string Description { get; set; }

        public string RenderMetaTags(string defaultTitle)
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrEmpty(Title))
                Title = defaultTitle;
            sb.AppendLine("<title>" + Title.Trim() + "</title>");
            if (string.IsNullOrEmpty(Description) == false)
            {
                sb.AppendLine("<meta name=\"description\" content=\"" + Description + "\" />");
            }
            if (string.IsNullOrEmpty(Keywords) == false)
            {
                sb.AppendLine("<meta name=\"keywords\" content=\"" + Keywords + "\" />");
            }
            return sb.ToString();
        }
    }
}
