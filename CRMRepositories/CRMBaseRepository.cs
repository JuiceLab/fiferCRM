using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using CRMContext;
using Settings;
using AccessModel;
using EntityRepository;
using EnumHelper.CRM;
using CompanyContext;

namespace CRMRepositories
{
    public class CRMBaseRepository : IDisposable
    {
        protected const string _defaultLocalDB = "fifer_localcrm_0";
        public CompanyEntities Context { get; set; }

        public CRMBaseRepository()
        {
            Context = new CompanyContext.CompanyEntities(AccessSettings.LoadSettings().CompanyEntites);
        }     

        public void Dispose()
        {
            Context.Database.Connection.Close();
            Context.Dispose();
        }

        
    }
}
