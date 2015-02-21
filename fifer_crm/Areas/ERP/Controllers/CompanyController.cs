using AuthService.AuthorizeAttribute;
using CompanyModel;
using CompanyRepositories;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using LogRepositories;
using LogService.FilterAttibute;
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
    public class CompanyController : BaseFiferController
    {
        CompanyRepository _repository = new CompanyRepository();
        
        [AuthorizeFuncRole(Profile = "Руководитель"), DisplayName("Страница компании")]
        public ActionResult IndexCompany()
        {
            DepartmentRepository repository = new DepartmentRepository();

            var model = new CompanyWrapViewModel()
            {
                Company = repository.GetCompanyByUserId(_userId)
            };
            model.Positions = repository.GetPositions(model.Company.CompanyId);
            model.Menu = GetMenu("Данные компании");
            CRMRepository crmRepository = new CRMRepository();
            var city = crmRepository.GetCompanyCity(model.Company.Guid);
            ViewBag.Cities = crmRepository.GetCitiesSelectItems(null, city.CityGuid);
            return View(model);
        }

        [HttpPost]
        [DisplayName("Сохранение данных компании")]
        public ActionResult Edit(CompanyViewModel model)
        {
            _repository.UpdateCompany(model, _userId);
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            repository.UpdateCompanyLegals(model.CompanyId, _userId);
            return RedirectToAction("IndexCompany");
        }
    }
}