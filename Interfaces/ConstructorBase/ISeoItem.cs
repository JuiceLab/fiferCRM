using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ConstructorBase
{
    public interface ISeoItem
    {
        int SeoItemId { get; set; }
        string Title { get; set; }
        string Keywords { get; set; }
        string Description { get; set; }

        string RenderMetaTags(string defaultTitle);
    }
}
