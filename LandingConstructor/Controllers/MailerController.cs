using CompanyRepositories;
using EnumHelper.Mailer;
using MailService;
using SiteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SiteConstructor.Controllers
{
    public class MailerController : Controller
    {
        // GET: Mailer
        public ActionResult SendFeedbackMail(Guid siteId, FeedbackModel model)
        {
            CustomerMailer mailer = new CustomerMailer();
            CompanyRepository repository = new CompanyRepository();
            var site  = repository.GetSiteProject(siteId);
            mailer.DoNotify<CustomerMail>(site.CreatedBy, string.Format("Клиент:{3}. Сообщение:{0}. Контактые данные: тел. {1}, e-mail {2}", model.Text, model.Phone, model.Mail, model.ClientName), CustomerMail.Feedback);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}