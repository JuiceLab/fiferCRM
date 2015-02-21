using CompanyModel;
using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using FinanceModel;
using FinanceRepositories;
using Interfaces.CRM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fifer_crm.Models
{
    public class CompanyPaymentsWrapModel: ICompanyInfo
    {
        public Guid UserId { get; set; }
        public string CompanyName { get; set; }
        public Dictionary<string, IEnumerable<IMenuItem>> Menu { get; set; }
        public CompanyViewModel Company { get; set; }

        public IEnumerable<PaymentViewModel> Payments { get; set; }

        public IEnumerable<CRMCompanyViewModel> PaymentCompanies { get; set; }

        public Dictionary<Guid, List<CompletionActModel>> CompletionActs { get; set; }

        public CompanyPaymentsWrapModel()
        {
            UserId = Guid.Empty;
        }

        public CompanyPaymentsWrapModel(Guid userId, Guid? companyId = null)
        {
            UserId = userId;
            CompanyRepository repository = new CompanyRepository();
            Company = repository.GetShortCompanyInfoByUserId(userId);
            CRMLocalRepository crmRepository = new CRMLocalRepository(userId);

            PaymentCompanies = crmRepository.GetCompanies();

            FinanceBaseRepository financeRepository = new FinanceBaseRepository(userId);
            Payments = financeRepository.GetPayments(DateTime.Now.AddMonths(-6), companyId);
            CompletionActs = financeRepository.GetCompletionActs(DateTime.Now.AddMonths(-3), companyId);
        }

        public int CompanyId { get { return Company.CompanyId; } set { Company.CompanyId = value; } }

        public string Logo { get { return Company.Logo; } set { Company.Logo = value; } }

        public string Name { get { return Company.PublicCompanyName; } set { Company.PublicCompanyName = value; } }
    }
}
