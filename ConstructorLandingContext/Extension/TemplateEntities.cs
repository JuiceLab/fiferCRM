using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorLandingContext
{
    public partial class TemplateEntities
    {
        public TemplateEntities(string conStr)
            : base(conStr)
        { }
    }
}
