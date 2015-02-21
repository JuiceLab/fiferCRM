using ConstructorLandingContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorRepositories
{
    public class LandingRepository : BaseConstructorRepository
    {
        public LandingRepository(string connStr)
            : base(connStr)
        {
        }
        public bool IsExistAllias(string allias)
        {
            return Context.LandingPageViews.Any(m => m.Allias == allias);
        }
    }
}
