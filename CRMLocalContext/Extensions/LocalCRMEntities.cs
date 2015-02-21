using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMLocalContext
{
    public partial class LocalCRMEntities
    {
        public LocalCRMEntities(string connectionString)
            : base(connectionString)
        { }
    }
}
