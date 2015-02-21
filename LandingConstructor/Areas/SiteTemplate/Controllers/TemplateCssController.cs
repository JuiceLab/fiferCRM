using ConstructorWizards;
using SiteConstructor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteConstructor.Areas.SiteTemplate.Controllers
{
    public class TemplateCssController : Controller
    {
        // GET: SiteTemplate/TemplateCss
        public ActionResult GetCategoryCss(Guid siteId, int? categoryId)
        {
            if (categoryId.HasValue)
            {
                PasteBoardWizard wizard = new PasteBoardWizard(SiteSettingsWrapper.Instance.SiteSources[siteId]);
                return PartialView(wizard.GetCssChoice(categoryId.Value));
            }
            else
                return PartialView(new List<SelectListItem>());
        }
    }
}