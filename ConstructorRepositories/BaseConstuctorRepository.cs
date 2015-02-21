using ConstructorLandingContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorRepositories
{
    public class BaseConstructorRepository : IDisposable
    {
        public LandingEntities Context { get; set; }

        public BaseConstructorRepository(string connStr)
        { 
        }

        public void Dispose()
        {
            Context.Database.Connection.Close();
            Context.Dispose();

        }
    }
}
