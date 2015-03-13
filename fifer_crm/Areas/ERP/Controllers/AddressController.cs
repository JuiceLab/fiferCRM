using CompanyModel;
using CompanyRepositories;
using CRMRepositories;
using fifer_crm.Controllers;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.ERP.Controllers
{
    [Authorize, CRMLogAttribute]
    public class AddressController : BaseFiferController
    {
        CompanyRepository _repository = new CompanyRepository();

        [HttpPost]
        [DisplayName("Сохранение адресных данных компании")]
        public ActionResult Edit(CompanyAddressViewModel model)
        {
            CRMRepository crmRepository = new CRMRepository();
            var city = crmRepository.GetCity(model.CityGuid);
            model.City = city.Prefix + " " + city.Name; 
            _repository.UpdateAddress(model, _userId);
            return RedirectToAction("IndexCompany", "Company", new { Area = "ERP" });
        }
    }
}