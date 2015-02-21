using CompanyModel;
using CompanyRepositories;
using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fifer_crm.Models
{
   public class WebSiteWrapModel : ICompanyInfo
    {
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }
        public CompanyViewModel Company { get; set; }
        public IEnumerable<SiteProjectModel> Sites { get; set; }
        public WebSiteWrapModel()
        {
            UserId = Guid.Empty;
        }

        public WebSiteWrapModel(Guid userId)
        {
            UserId = userId;
            CompanyRepository repository = new CompanyRepository();
            Company = repository.GetShortCompanyInfoByUserId(userId);
            Sites = repository.GetSiteProjects(Company.CompanyId);
        }

        public int CompanyId { get { return Company.CompanyId; } set { Company.CompanyId = value; } }

        public string Logo { get { return Company.Logo; } set { Company.Logo = value; } }

        public string Name { get { return Company.PublicCompanyName; } set { Company.PublicCompanyName = value; } }
    }
}
