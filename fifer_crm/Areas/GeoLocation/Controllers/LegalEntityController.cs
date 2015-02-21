using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Areas.GeoLocation.Controllers
{
    public class LegalEntityController : Controller
    {
        // GET: GeoLocation/LegalEntity
        public ActionResult LegalAddress(Guid companyId)
        {
            return PartialView();
        }
    }
}