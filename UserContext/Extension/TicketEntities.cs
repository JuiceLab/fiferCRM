using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserContext
{
    public partial class UserEntities : DbContext
    {
        public UserEntities(string connection)
            : base(connection)
        {
        }
    }
}
