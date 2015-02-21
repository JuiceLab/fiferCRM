using AccessRepositories;
using AuthService.AuthorizeAttribute;
using CompanyModel;
using CompanyRepositories;
using fifer_crm.Models;
using Interfaces.CRM;
using LogRepositories;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TicketRepositories;

namespace fifer_crm.Controllers
{
    [Authorize, CRMLogAttribute]
    public class HomeController : BaseFiferController
    {

        [DisplayName("Главная страница")]
        public ActionResult Index()
        {
           return RedirectToAction("IndexEmployee", "Ordinary", new { Area = "Workspace" });
        }   
    }
}