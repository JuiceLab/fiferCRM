using AuthService.AuthorizeAttribute;
using CompanyModel;
using CompanyRepositories;
using fifer_crm.Models;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace fifer_crm.Controllers
{
    [Authorize, CRMLogAttribute]
    public class DepartmentController : BaseFiferController
    {
        DepartmentRepository _repository = new DepartmentRepository();

        [AuthorizeFuncRole(Profile = "Руководитель"), DisplayName("Страница отделов")]
        public ActionResult IndexDepartments()
        {
            DepartmentRepository repository = new DepartmentRepository();
            var model = new CompanyWrapViewModel(_userId);
            model.CompanyName = repository.GetCompanyName(model.UserId);
            model.Departments = repository.GetDepartments(model.UserId);
            model.Positions = repository.GetPositions(model.UserId);
            model.WorkGroups = repository.GetGroups(model.CompanyId);
            model.Menu = GetMenu("Отделы");
            return View(model);
        }
        // GET: Department
        [DisplayName("Данные отдела компании")]
        public ActionResult DepartmentEdit(int? departmentId)
        {
            var model = _repository.GetDepartment(departmentId);
            ViewBag.Groups = _repository.GetGroups4Company(_userId, model.Groups);
            return PartialView(model);
        }

        [HttpPost]
        [DisplayName("Сохранение данных отдела компании")]
        public ActionResult DepartmentEdit(DepartmentViewModel model)
        {
            _repository.AddOrUpdateDepartment(model, _userId);
            return RedirectToAction("IndexDepartments");
        }

        [DisplayName("Данные должности в компании")]
        public ActionResult PositionEdit(int? positionId)
        {
            var userId = _userId;
            ViewBag.AvailableDepartments = _repository.GetDepartments(userId);
            ViewBag.AvailablePositions = _repository.GetItemPositions(userId, positionId);

            return PartialView(_repository.GetPosition(positionId));
        }

        [HttpPost]
        [DisplayName("Сохранение данных о должности в компании")]
        public ActionResult PositionEdit(PositionViewModel model)
        {

            _repository.AddOrUpdatePosition(model, _userId);
            return RedirectToAction("IndexDepartments");
        }

        [DisplayName("Данные рабочей группы в компании")]
        public ActionResult GroupEdit(int? groupId)
        {
            var userId = _userId;
            return PartialView(_repository.GetGroup(groupId));
        }

        [HttpPost]
        [DisplayName("Сохранение данных о рабочей группе в компании")]
        public ActionResult GroupEdit(WorkGroupViewModel model)
        {
            _repository.AddOrUpdateGroup(model,_userId);
            return RedirectToAction("IndexDepartments");
        } 
    }
}