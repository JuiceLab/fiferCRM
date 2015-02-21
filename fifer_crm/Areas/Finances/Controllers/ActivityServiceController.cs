using CompanyRepositories;
using CRMRepositories;
using fifer_crm.Controllers;
using FinanceModel;
using FinanceRepositories;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.Finances.Controllers
{
    [Authorize, CRMLogAttribute]
    public class ActivityServiceController : BaseFiferController
    {
        // GET: Finances/ActivityService
        public ActionResult Index()
        {
            FinanceBaseRepository repository = new  FinanceBaseRepository(_userId);
            FinanceClause model = repository.GetFinanceClause();
            model.Menu = GetMenu("Статьи доходов и расходов");
            CompanyRepository companyRepository = new CompanyRepository();
            ViewBag.IsRegisterCompany = companyRepository.GetShortCompanyInfoByUserId(_userId).Guid.HasValue;

            companyRepository.UpdateCompanyInfoByUserId(_userId, model);
            return View(model);
        }

        [HttpGet]
        public ActionResult ActivityServiceEdit(int? serviceId)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            ActivityServiceItem model = repository.GetActivityService(serviceId);
            if (!serviceId.HasValue)
            {
                CRMLocalRepository crmRepository = new CRMLocalRepository(_userId);

                var services = new List<SelectListItem>(){
                     new SelectListItem(){Text ="Выберите услугу"}
                };
                services.AddRange(crmRepository.GetServices(new List<int>()));
                ViewBag.Services = services;
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ActivityServiceEdit(ActivityServiceItem model)
        {
            CompanyRepository companyRepository = new CompanyRepository();
            var companyGuid = companyRepository.GetShortCompanyInfoByUserId(_userId).Guid;
            if (!companyGuid.HasValue)
                throw new Exception("Компания не зарегистирована в общей базе компаний. Для решения проблемы обратитесь в службу поддержки");
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            repository.UpdateActivityService(model, companyGuid);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ExpenseEdit(int? expenseId)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            SelectListItem model = repository.GetExpense(expenseId);
            return PartialView(model);
        }

        public ActionResult ExpenseEdit(SelectListItem model)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            repository.UpdateExpense(model);
            return RedirectToAction("Index");
        }
    }
}