﻿using CompanyModel;
using CRMRepositories;
using fifer_crm.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.GeoLocation.Controllers
{
    public class LegalEntityController : BaseFiferController
    {
        CRMRepository _repository = new CRMRepository();

        // GET: GeoLocation/LegalEntity
        public ActionResult LegalAddress(int companyId)
        {
            CRMLocalRepository localRepository = new CRMLocalRepository(_userId);
            ViewBag.Company = companyId;
            IEnumerable<CompanyAddressViewModel> model = localRepository.GetLocations(companyId);
           var distr = _repository.GetDistricts();
            ViewBag.Cities = _repository.GetCitiesSelectItems(null, model.FirstOrDefault().CityGuid);
            ViewBag.Districts = distr;
            return PartialView(model);
        }

        public ActionResult LegalAddressEdit(int companyId, int? addrId)
        {
            CRMLocalRepository localRepository = new CRMLocalRepository(_userId);
            CompanyAddressViewModel model = localRepository.GetLocation(companyId, addrId);
            var distr = _repository.GetDistricts();
            int? distrId = null;
            if (model.CityGuid != Guid.Empty)
            {
                distrId = _repository.GetDistrictByCity(model.CityGuid);
                distr.FirstOrDefault(m => m.Value == distrId.ToString()).Selected = true;
            }
            ViewBag.Cities = _repository.GetCitiesSelectItems(distrId, model.CityGuid);
            ViewBag.Districts = distr;

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult AddressEdit(CompanyAddressViewModel model)
        {
            CRMLocalRepository localRepository = new CRMLocalRepository(_userId);
            localRepository.UpdateLegalAddress(model);
            return RedirectToAction("LegalAddress", new { companyId = model.CompanyId });
        }
    }
}