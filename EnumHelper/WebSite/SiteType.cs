using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnumHelper.WebSite
{
    public enum SiteType : byte
    {
        [StringValue("Визитка")]
        PasteBoard=1,
        [StringValue("Лэндинг")]
        Landing = 2,
        [StringValue("Магазин")]
        Store = 3
    }
}
