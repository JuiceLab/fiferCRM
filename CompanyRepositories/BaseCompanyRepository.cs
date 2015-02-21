using CompanyContext;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyRepositories
{
    public class BaseCompanyRepository : IDisposable
    {
        public CompanyEntities Context { get; set; }
        public CompanyViewEntities ViewContext { get; set; }

        public BaseCompanyRepository()
        {
            Context = new CompanyEntities(AccessSettings.LoadSettings().CompanyEntites);
            ViewContext = new CompanyViewEntities(AccessSettings.LoadSettings().CompanyViewsEntites);
        }

        protected int GetCompanyId(Guid userId)
        {
            return ViewContext.UserInDepartmentView
              .FirstOrDefault(m => m.UserId == userId)
              .C_CompanyId;
        }
        public void Dispose()
        {
            Context.Database.Connection.Close();
            Context.Dispose();

            ViewContext.Database.Connection.Close();
            ViewContext.Dispose();
        }
    }
}
