using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorLandingContext
{
    public partial class PasteboardEntities
    {
        public PasteboardEntities(string conStr)
            : base(conStr)
        { }
    }
}
