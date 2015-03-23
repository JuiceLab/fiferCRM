using CompanyRepositories;
using CRMRepositories;
using fifer_crm.Controllers;
using FilterModel;
using FinanceRepositories;
using Interfaces.Finance;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.Workspace.Controllers
{
    public class FinanceController : BaseFiferController
    {
        [Authorize, CRMLogAttribute]
        // GET: Workspace/Finance
        [DisplayName("Главная страница финансовго отдела")]
        public ActionResult Index(FinanceFilter filter)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            IFinanceStatInteractive financeStat = repository.GetFinanceStatInteractive(_userId, filter);
            financeStat.Menu = GetMenu("Финансовая активность");
            
            CompanyRepository companyRepository = new CompanyRepository();
            companyRepository.UpdateCompanyInfoByUserId(_userId, financeStat);
            return View(financeStat);
        }
    }
}