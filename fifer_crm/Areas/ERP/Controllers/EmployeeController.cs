using AccessModel;
using AccessRepositories;
using AuthService.AuthorizeAttribute;
using CompanyModel;
using CompanyRepositories;
using EnumHelper;
using EnumHelper.Mailer;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogRepositories;
using LogService.FilterAttibute;
using MailService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace fifer_crm.Areas.ERP.Controllers
{
    [Authorize, CRMLogAttribute]
    public class EmployeeController : BaseFiferController
    {
        StaffRepository _repository = new StaffRepository();
        CRMAccessRepository _crmAccessRepository = new CRMAccessRepository();
        MembershipRepository _accessRepository = new MembershipRepository();

        [AuthorizeFuncRole(Profile = "Руководитель"), DisplayName("Страница сотрудников")]
        public ActionResult IndexEmployees()
        {
            StaffRepository repository = new StaffRepository();
            BaseLogRepository logRepository = new BaseLogRepository();
            var model = new CompanyWrapViewModel(_userId);
            ViewBag.Profile = model.UserPhoto;

            model.CompanyName = repository.GetCompanyName(model.UserId);
            model.Employees = repository.GetEmployees(model.UserId);
            model.EmployeesActions = logRepository.GetUserLogActions(model.Employees.Select(m => m.UserId), DateTime.Now.Date.AddDays(-1));
            foreach (var item in model.EmployeesActions)
            {
                var employee = model.Employees.FirstOrDefault(m => m.UserId == item.UserId);
                item.Title = employee.FirstName + " " + employee.LastName;
                item.IconPath = employee.PhotoPath;
            }
            model.Positions = repository.GetPositions(model.UserId);
            model.Menu = GetMenu("Сотрудники");
            return View(model);
        }

        [AuthorizeFuncRole(Profile = "Руководитель"), DisplayName("Страница посещений")]

        public ActionResult IndexVisiting()
        {
            StaffRepository repository = new StaffRepository();

            var model = new CompanyWrapViewModel()
            {
                Company = repository.GetCompanyByUserId(_userId)
            };
            model.Employees = repository.GetEmployees(_userId);
            model.TimeSheetEmployees = repository.GetTimesheetEmployees(model.Employees);
            model.Menu = GetMenu("Посещаемость");

            return View(model);
        }

        // GET: Employee
        [DisplayName("Редактирование сотрудника")]
        public ActionResult EmployeeEdit(int? employeeId)
        {
            ViewBag.Positions = _repository.GetPositions(_userId);
            if (employeeId.HasValue)
            {
                var model = _repository.GetEmployee(employeeId);
                _crmAccessRepository.UpdateUserContact(model);
                var roles  = _crmAccessRepository.GetFuncRoles4Employee(model.UserId);
                model.FuncRoleId = roles.Any(m=>m.Selected) ?
                    Convert.ToInt32(roles.FirstOrDefault(m=>m.Selected).Value)
                    :0;
                ViewBag.Groups = _repository.GetGroups4Company(_userId, model.WorkGroups);
                ViewBag.FuncRoles4CRM = roles;
                return PartialView(model);
            }
            else
            {
                ViewBag.FuncRoles4CRM = _crmAccessRepository.GetFuncRoles4Employee(Guid.Empty);
                return PartialView("NoveltyEmployee", new EmployeeRegisterModel());
            }
        }

        [DisplayName("Новый сотрудник")]
        [HttpPost]
        public ActionResult NoveltyEmployee(EmployeeRegisterModel employee)
        {
            Guid userId = _accessRepository.CreateEmployeeUser(employee);
            _repository.AddEmployee(employee, userId);
            _crmAccessRepository.SetFuncRole(userId, employee.FuncRoleId);
            
            NotifyRepository notifyRepository = new NotifyRepository();
            notifyRepository.CreateEmployeeDeafultNotifySettings(userId);
           
            AuthMailer mailer = new AuthMailer();
            mailer.DoNotify(userId, string.Format("Открыт доступ для нового сотрудника в системе <a href='http://bizinvit.ru/'>Fifer-CRM</a>. Данные вашей учетной записи: Логин: {0}, Пароль: {1} .  ", employee.Login, employee.Pass), AuthMail.NewEmployee);

            return RedirectToAction("IndexEmployees");
        }

        [HttpPost]
        [DisplayName("Сохранение данных сотрудника")]
        public ActionResult EmployeeEdit(EmployeeViewModel employee)
        {
            _repository.UpdateEmployee(employee);
            _accessRepository.UpdateUser(employee);
            _crmAccessRepository.SetFuncRole(employee.UserId, employee.FuncRoleId);
            return RedirectToAction("IndexEmployees");
        }

        [HttpPost]
        [DisplayName("Сохранение данных ключевого сотрудника")]
        public ActionResult EditKeyEmployee(CompanyEmployeeViewModel model)
        {
            _repository.UpdateEmployee(model);
            return RedirectToAction("IndexCompany", "Company", new { Area = "ERP" });
        }

        [DisplayName("Паспортные данные сотрудника")]
        public ActionResult EmployeePassportEdit(int? employeeId)
        {
            return PartialView(_repository.GetEmployeePassport(employeeId));
        }

        [HttpPost]
        [DisplayName("Сохранение паспортных данных сотрудника")]
        public ActionResult EmployeePassportEdit(PassportViewModel passport, List<string> scanPaths)
        {
            if (scanPaths != null)
                passport.ScanPath = scanPaths;
            passport.ScanPath = scanPaths;
            _repository.AddOrUpdateEmployeePassport(passport);
            return RedirectToAction("IndexEmployees");
        }

        [DisplayName("Ограничение доступа сотрудника")]
        public ActionResult EmployeeLockEdit(int employeeId)
        {
            var employee = _repository.GetEmployee(employeeId);
            ViewBag.Employee = employee.FirstName + " " + employee.LastName;
            return PartialView(_accessRepository.GetEmployeeLocks(employee.UserId));
        }

        [DisplayName("Применение ограничение доступа сотрудника")]
        public ActionResult EmployeeLockEdit(UserLockModel model)
        {
            _accessRepository.UpdateEmployeeLock(model);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [DisplayName("Перерыв")]        
        public ActionResult SetTimeBreak(string comment, TimeBreakType type,  bool isStart)
        {
            CompanyRepository repository = new CompanyRepository();
            _repository.SetTimeBreak(comment, type, isStart, _userId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        
    }
}