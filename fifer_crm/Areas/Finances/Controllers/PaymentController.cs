using CompanyRepositories;
using CRMRepositories;
using EnumHelper.Payment;
using fifer_crm.Controllers;
using FinanceModel;
using FinanceRepositories;
using Interfaces.Finance;
using LogService.FilterAttibute;
using ReportService.Finance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.Finances.Controllers
{
    [Authorize, CRMLogAttribute]
    public class PaymentController : BaseFiferController
    {
        // GET: Finances/Payment
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult PaymentEdit(int? paymentId, byte type, int? companyId=null)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            PaymentEditModel model = repository.GetPaymentModel(paymentId, type, companyId);
            model.PaymentDetails = repository.GetDetails(paymentId);
            model.OwnerId = _userId;

            CRMLocalRepository localRepository = new CRMLocalRepository(_userId);
            ViewBag.PaymentType = type;
            if(type == (byte)PaymentType.Expense)
            {
                ViewBag.Services = localRepository.GetExpenseServices();
                return PartialView("ExpenseEdit", model);
            }

            if (!paymentId.HasValue)
            {
                StaffRepository staffRepository = new StaffRepository();
                var users = staffRepository.GetSubordinatedUsers(_userId);

                ViewBag.Customers = localRepository.GetCustomers4Subordinate(users.Select(m => Guid.Parse(m.Value)), companyId);
                ViewBag.Services = localRepository.GetServices(new List<int>(), true);
            }
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult PaymentUpdateStatus(PaymentEditModel model, byte statusId)
        {
            model.StatusId = statusId;
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            repository.ChangePaymentModelState(model);
            if (model.StatusId == (byte)PaymentStatus.WaitPays)
            {
                CRMRepository baseRepository = new CRMRepository();
                Guid[] guids = baseRepository.GetCustomerAndCompany(_userId);
                string baseName = baseRepository.GetCompanyBase(model.InvoicedInfo.CustomerInfo.UserId);
                if(!string.IsNullOrEmpty(baseName))
                    repository.CreatePaymentInvoice(guids, baseName, model);
            }
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult PaymentEdit(PaymentEditModel model)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            repository.UpdatePaymentModel(model);
            return Json(new { result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExpenseEdit(PaymentEditModel model)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            repository.UpdateExpenseModel(model);
            return RedirectToAction("Index", "Finance", new { Area = "Workspace" });
        }

        [HttpPost]
        public ActionResult PaymentDetailsTable(PaymentEditModel model, int serviceId, int? qty)
        {
            FinanceBaseRepository repository = new FinanceBaseRepository(_userId);
            CRMLocalRepository localRepository = new CRMLocalRepository(_userId);
            ViewBag.Services = localRepository.GetServices(new List<int>(), true);
            var item = repository.GetServiceDetail(serviceId, qty.HasValue ? qty.Value : 1);
            var existItem = model.PaymentDetails.FirstOrDefault(m => m.Name == item.Name && m.Cost == item.Cost);
            if(existItem != null)
                existItem.Qty = existItem.Qty + item.Qty;
            else
                model.PaymentDetails.Add(item);
            return PartialView(model);
        }

        public ActionResult GetPaymentExcel(int paymentId, byte type)
        {
            ExcelReporter reporter = new ExcelReporter(_userId);
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=Расчетный счет.xlsx");
            return new FileContentResult(
                    reporter.GetPayment(Server.MapPath("~/private/excel-template/payment.default.xlsx"), paymentId, type),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}