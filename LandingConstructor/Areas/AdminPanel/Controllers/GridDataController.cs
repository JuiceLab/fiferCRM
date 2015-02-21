using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ConstructorRepositories;

namespace SiteConstructor.Areas.AdminPanel.Controllers
{
    [Authorize]
    public class GridDataController : Controller
    {
        private GridDataRepository _repository;

        public ActionResult GetLandings([DataSourceRequest] DataSourceRequest request)
        {
            _repository = new GridDataRepository(string.Empty);
            var domain = HttpContext.Request.Url.Authority;
            return Json(_repository.GetPagesTable(domain).ToDataSourceResult(request));
        }
	}
}