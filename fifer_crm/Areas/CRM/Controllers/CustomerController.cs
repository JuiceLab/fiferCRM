using AccessRepositories;
using AuthService.AuthorizeAttribute;
using CompanyModel;
using CompanyRepositories;
using CRMModel;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskModel;

namespace fifer_crm.Areas.CRM.Controllers
{
    [Authorize, CRMLogAttribute]
    public class CustomerController : BaseFiferController
    {
        [DisplayName("Загрузка списка сотрудников ю.л.")]
        public ActionResult CustomersList(Guid? companyId)
        {
            CRMCustomerRepository crmRepository = new CRMCustomerRepository(_userId);
            IList<CustomerViewModel> model = crmRepository.GetCustomers(companyId);

            List<Guid> ids = new List<Guid>();
            ids.AddRange(model.Where(m => m.AssignedBy.HasValue).Select(m => m.AssignedBy.Value).Distinct());
            ViewBag.CompanyId = companyId; 
            MembershipRepository membershipRepository = new MembershipRepository();
            var names = membershipRepository.GetUserByIds(ids.Distinct());
            foreach (var item in model)
            {
                var owner = names.FirstOrDefault(m => m.UserId == item.AssignedBy);
                item.AssignedName = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
            }
            return PartialView(model);
        }

        [HttpGet]
        [DisplayName("Загрузка истории взаимодейсвтия с физ. лицом")]
        public ActionResult GetHistory(Guid customerId)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            var customer = repository.GetCustomerByGuid(customerId);
            ViewBag.Name = customer.FirstName + " " + customer.LastName;
            List<MessageViewModel> model = repository.GetModifyLog(customerId);

            StaffRepository staffRepository = new StaffRepository();
            var photos = staffRepository.GetEmployeesPhoto(_userId);

            MembershipRepository membershipRepository = new MembershipRepository();
            var names = membershipRepository.GetUserByIds(model.Select(m => m.UserId).Distinct());
            foreach (var item in model)
            {
                if (item.UserId != Guid.Empty)
                {
                    var owner = names.FirstOrDefault(m => m.UserId == item.UserId);
                    item.Title = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
                    item.IconPath = photos[item.UserId];
                }
            }
            return PartialView(model);
        }

        [DisplayName("Загрузка формы редактирования клиента")]
        public ActionResult Edit(int? customerId, bool? isLegal)
        {
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            CustomerEditModel model = repository.GetCustomerEditModel(customerId);

            StaffRepository staffRepository = new StaffRepository();
            ViewBag.Assign = staffRepository.GetSubordinatedUsers(_userId);
            var items = new List<SelectListItem>() { new SelectListItem() { Text = "Физ.лицо" } };
            if (!isLegal.HasValue || isLegal.Value)
            {
                items.AddRange(repository.GetSelectListCompanies());
                ViewBag.Companies = items;
                return PartialView(model);
            }
            else
            {
                model.Passport = repository.GetCustomerPassport(customerId);
                return PartialView("EditNonLegal", model); 
            }
        }
        [DisplayName("Загрузка формы редактирования клиента")]
        public ActionResult CustomerEditForm(int? customerId, Guid? companyId)
        {
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            CustomerEditModel model = repository.GetCustomerEditModel(customerId);

            StaffRepository staffRepository = new StaffRepository();
            ViewBag.Assign = staffRepository.GetSubordinatedUsers(_userId);
            var items = new List<SelectListItem>();
            items.Add(repository.GetSelectListCompanies()
                .FirstOrDefault(m=>m.Value == (model.CompanyId.HasValue? model.CompanyId.ToString() 
                    : companyId.Value.ToString())));
            if (items.Count == 1)
                items[0].Selected = true;
            ViewBag.Companies = items;
   
            return PartialView(model);
        }
        [HttpPost]
        [DisplayName("Сохранение данных клиента")]
        public ActionResult Edit(CustomerEditModel model)
        {
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            repository.UpdateCustomer(model, _userId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }


        [DisplayName("Загрузка паспортных данных клиента")]
        public ActionResult EditPassportCustomer(Guid customerId)
        {
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            var model = repository.GetCustomerPassport(customerId);
            CRMRepository crmRepository = new CRMRepository();
            var distr = crmRepository.GetDistricts();
            if (model.CityGuid.HasValue && model.CityGuid != Guid.Empty)
            {
                var distrId = crmRepository.GetDistrictByCity(model.CityGuid.Value);
                distr.FirstOrDefault(m => m.Value == distrId.ToString()).Selected = true;
            }

            ViewBag.Cities = crmRepository.GetCitiesSelectItems(null, model.CityGuid);
            ViewBag.Districts = distr;
            return PartialView(model);
        }

        [HttpPost]
        [DisplayName("Сохранение паспортных данных клиента")]
        public ActionResult EditPassportCustomer(PassportViewModel model, List<string> scanPaths)
        {
            if (scanPaths !=null)
                model.ScanPath = scanPaths;
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            repository.UpdatePassportCustomer(model, _userId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [DisplayName("Загрузка списка услуг клиента")]
        public ActionResult CompanyServices(int customerId)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            IEnumerable<SelectListItem> model = repository.GetSerivces4CustomerCompany(customerId); 
            return PartialView(model);
        }

        [HttpGet]
        [DisplayName("Загрузка формы назначения клиента за сотрудником")]
        public ActionResult Assign(int? customerId)
        {
            InitSubordinatedUsers();
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            CustomerEditModel model = repository.GetCustomerEditModel(customerId);

            StaffRepository staffRepository = new StaffRepository();
            ViewBag.Assign = staffRepository.GetSubordinatedUsers(_userId);

            return PartialView(model);
        }

        [HttpPost]
        [DisplayName("Сохранение сотрудника, закрепленного за клиентом")]
        public ActionResult Assign(CustomerEditModel model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            StaffRepository staffRepository = new StaffRepository();
            var names = staffRepository.GetSubordinatedUsers(_userId).ToDictionary(m => Guid.Parse(m.Value), m => m.Text);
            repository.ChangeAssign(model, _userId, names);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [DisplayName("Загруза формы изменения статуса клиента")]
        public ActionResult ChangeStatus(int customerId)
        {
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            CustomerEditModel model = repository.GetCustomerEditModel(customerId);
            return PartialView(model);
        }

        [HttpPost]
        [DisplayName("Сохранение статуса клиента")]
        public ActionResult ChangeStatus(CustomerEditModel model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            repository.ChangeStatus(model, _userId);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [DisplayName("Загрузка комментария по клиенту")]
        public ActionResult EditComment(int customerId)
        {
            CRMCustomerRepository repository = new CRMCustomerRepository(_userId);
            CustomerEditModel model = repository.GetCustomerEditModel(customerId);

            return PartialView(model);
        }

        [HttpPost]
        [DisplayName("Сохранение комментария по клиенту")]
        public ActionResult EditComment(CustomerEditModel model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            repository.UpdateComment(model, _userId);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }
    }
}