using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportContext
{
    public partial class TicketEntities : DbContext
    {
        public TicketEntities(string connection)
            : base(connection)
        {
        }
    }
}
