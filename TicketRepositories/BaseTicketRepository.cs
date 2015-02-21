using Settings;
using SupportContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TicketRepositories
{
    public class BaseTicketRepository : IDisposable
    {
        protected const int _highPriority = 10;
        public TicketEntities Context { get; set; }

        public BaseTicketRepository()
        {
            Context = new TicketEntities(AccessSettings.LoadSettings().TicketEntites);
        }

        public void Dispose()
        {
            Context.Database.Connection.Close();
            Context.Dispose();
        }
    }
}
