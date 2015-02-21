using CompanyRepositories;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using FinanceModel;
using FinanceRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.Finances.Controllers
{
    public class PaymentActsController : BaseFiferController
    {
        // GET: Finances/PaymentActs
        public ActionResult Index()
        {
            CompanyPaymentsWrapModel model = new CompanyPaymentsWrapModel(_userId);
            model.Menu = GetMenu("Акты работ");
            return View(model);
        }

        public ActionResult PaymentsList(Guid companyId)
        {
            CompanyPaymentsWrapModel model = new CompanyPaymentsWrapModel(_userId, companyId);
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult Edit(int? actId)
        {
            FinanceBaseRepository financeRepository = new FinanceBaseRepository(_userId);
            CompletionActEditModel model = financeRepository.GetCompletionAct(actId);
       
        

            CRMCustomerRepository customerRepository = new CRMCustomerRepository(_userId);
            var companies = customerRepository.GetCompanies4Act();
            ViewBag.Companies = companies;
            ViewBag.Payments = financeRepository.GetPaymentsByIds(model.Payments, Guid.Parse(companies.FirstOrDefault().Value));
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            model.CustomerGuid = repository.GetCustomers(Guid.Parse(companies.FirstOrDefault().Value)).FirstOrDefault().UserId;
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Edit(CompletionActEditModel model)
        {
            FinanceBaseRepository financeRepository = new FinanceBaseRepository(_userId);
            financeRepository.AddOrUpdateAct(model, _userId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult PaymentActDetailsTable(string payments, string companyGuid)
        {
            FinanceBaseRepository financeRepository = new FinanceBaseRepository(_userId);
            var model = new CompletionActEditModel() { StrPayments = payments };
            ViewBag.Payments = financeRepository.GetPaymentsByIds(model.Payments, Guid.Parse(companyGuid));
            return PartialView(model);
        }
    }
}