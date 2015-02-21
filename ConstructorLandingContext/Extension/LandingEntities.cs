using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorLandingContext
{
    public partial class LandingEntities
    {
        public LandingEntities(string conStr)
            : base(conStr)
        { }
    }
}
