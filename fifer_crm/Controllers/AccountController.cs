using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AccessModel;
using AccessRepositories;
using System.Web.Routing;
using AuthService.FiferMembership;
using CompanyRepositories;
using System.Web.Security;
using CRMRepositories;
using System.Net;
using XMLModel.Sypexgeo;
using XMLModel;
using System.Text;
using MailService;
using EnumHelper.Mailer;
using LogRepositories;
using UserContext;
using Settings;

namespace fifer_crm.Controllers
{
    public class AccountController : Controller
    {
        MembershipRepository _repository = new MembershipRepository();
        CompanyRepository _companyRepository = new CompanyRepository();

        IFormsAuthenticationService FormsService { get; set; }
        IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        // GET: Account
        public ActionResult SignIn()
        {
            //using (WebClient wc = new WebClient() { Encoding = Encoding.UTF8 })
            //{
            //    var result = wc.DownloadString("http://api.sypexgeo.net/xml/123.45.67.89");
            //    int inx = result.IndexOf("<city>");
            //    var item = XMLBase.FromXML<XMLCity>(result.Substring(inx, result.IndexOf("</city>") - inx + 7));
            //}
            return View(new LoginViewModel());
        }

        [HttpPost]
        public ActionResult SignIn(LoginViewModel model)
        {
            //todo create temp page for many account 4 one email
            if (MembershipService.ValidateUser(model.Email, model.Password))
            {
                FormsService.SignIn(model.Email, model.RememberMe);
                try
                {
                    StaffRepository repository = new StaffRepository();
                    MembershipRepository membershipRepository = new MembershipRepository();
                    var user = membershipRepository.Context.Users.FirstOrDefault(m => m.Login == model.Email || m.Mail == model.Email);
                    if (user != null)
                        repository.SetLoginEmployee(user.UserId, ControllerContext.HttpContext.Request.UserHostAddress, true);
                    //CRMLocalRepository crmRepository = new CRMLocalRepository(user.UserId);
                    //crmRepository.CreateCompanyDB();
                }
                catch
                { 
                }
                return RedirectToAction("IndexEmployee", "Ordinary", new { Area="Workspace"});
            }
            return RedirectToAction("SignIn");
        }

        public ActionResult Logout()
        {
            FormsService.SignOut();
            StaffRepository repository = new StaffRepository();
            repository.SetLoginEmployee((Guid)Membership.GetUser().ProviderUserKey, ControllerContext.HttpContext.Request.UserHostAddress, false);
            return RedirectToAction("SignIn");            
        }

        [HttpGet]
        public ActionResult SignUp(bool? isExistLogin, CompanyRegisterModel existModel)
        {
            var model = existModel != null ? existModel : new CompanyRegisterModel() { IsLegalAddress = true };
            ViewBag.IsExist = isExistLogin;
            CRMRepository crmRepository = new CRMRepository();
            var districts = crmRepository.GetDistricts();
            ViewBag.Districts = districts;
            model.AvailableCities = crmRepository.GetCitiesSelectItems(int.Parse(districts.FirstOrDefault(m=>!string.IsNullOrEmpty(m.Value)).Value));
            return View(model);
        }

        [HttpPost]
        public ActionResult SignUp(CompanyRegisterModel model)
        {
            model.City = model.City.Trim().First().ToString().ToUpper() + String.Join("", model.City.Trim().Skip(1));
            string pass = string.Empty;
            var user = _repository.CreateUser(model, out pass);
            int companyId = 0;
            CRMRepository baseRepository = new CRMRepository();
            var city = baseRepository.GetCity(int.Parse(model.City));
            string companyDb = _companyRepository.CreateCompany(model, user.UserId, city.Name, out companyId);
           
            CRMLocalRepository repository = new CRMLocalRepository(user.UserId);
            repository.CreateCompanyDB();
            repository.AddCompanyRecord(model, companyId, user.UserId);
            
          
            
            CRMAccessRepository accessRepository = new  CRMAccessRepository();
            accessRepository.UpdateUserFuncRole(user.UserId, "Руководитель", true);
            
            NotifyRepository notifyRepository = new NotifyRepository();
            notifyRepository.CreateEmployeeDeafultNotifySettings(user.UserId);

            FormsService.SignIn(user.Login, false);

            AuthMailer mailer = new AuthMailer();
            mailer.DoNotify(user.UserId, string.Format("Спасибо за регистрацию. Данные вашей учетной записи: Логин: {0}, Пароль: {1} . ", user.Login, pass), AuthMail.NewCustomer);
            
            return RedirectToAction("IndexCompany", "Company", new { Area = "ERP" });
        }

        public ActionResult ResetPass(string login, string pass)
        {
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var guid = User.Identity.IsAuthenticated? 
                    (Guid)Membership.GetUser().ProviderUserKey 
                    : Guid.Empty;
                var user = guid == Guid.Empty? 
                    context.Users.FirstOrDefault(m=>m.Login == login || m.Mail == login) 
                    : context.Users.FirstOrDefault(m => m.UserId == guid);
                if (string.IsNullOrEmpty(pass))
                {
                    pass = _repository.GeneratePass(8);
                }
                MembershipService.ChangePassword(user.Login, pass, pass);
                AuthMailer mailer = new AuthMailer();
                mailer.DoNotify<AuthMail>(user.UserId, string.Format("Ваш новый пароль для сайта bizinvit.ru : {0}", pass), AuthMail.ResetPass);
            }
            return new JsonResult() { Data = new { data = true }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}