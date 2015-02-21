﻿using CRMRepositories;
using fifer_crm.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.GeoLocation.Controllers
{
    public class DistrictController : BaseFiferController
    {
        [AllowAnonymous]
        // GET: GeoLocation/District
        public ActionResult GetCities(int distrId)
        {
            CRMRepository crmRepository = new CRMRepository();
            var model = crmRepository.GetCitiesSelectItems(distrId);
            return PartialView(model);
        }
    }
}