using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserContext;

namespace AccessRepositories
{
    public class BaseAccessRepository : IDisposable
    {
        public UserEntities Context { get; set; }

        public BaseAccessRepository()
        {
            Context = new UserEntities(AccessSettings.LoadSettings().UserEntites);
        }
       
        public void Dispose()
        {
            Context.Database.Connection.Close();
            Context.Dispose();
        }

    }
}
