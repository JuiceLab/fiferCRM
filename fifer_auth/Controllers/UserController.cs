using AccessModel;
using AccessRepositories;
using AuthService.AuthorizeAttribute;
using AuthService.FiferMembership;
using MailService;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using UserContext;

namespace fifer_auth.Controllers
{
   [AuthorizeFuncRole(Profile = "Бог системы")]
    public class UserController : Controller
    {
        MembershipRepository _repository = new MembershipRepository();

        IFormsAuthenticationService FormsService { get; set; }
        IMembershipService MembershipService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (FormsService == null) { FormsService = new FormsAuthenticationService(); }
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }
        
        public ActionResult Index(Guid userId)
        {
            var user = _repository.GetUserById(userId);
            ViewBag.LastLog = _repository.GetLastLog(userId);
            return View(user);
        }

        [HttpGet]
        public ActionResult ProfileCart(Guid usedId)
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ProfileCart(User user)
        {
            _repository.UpdateUser(user);
            return RedirectToAction("Index", new { userId = user.UserId });
        }

        public ActionResult UserModal()
        {
            return PartialView();
        }

        public ActionResult Register(RegisterModel model)
        {
            var id = _repository.CreateUser(model);
            return Json(new { id = id }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BlockUser(Guid userId, bool isBlock)
        {
            User user = new User();
            using (UserEntities access = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                user = access.Users.FirstOrDefault(mbox => mbox.UserId == userId);
                user.IsBlocked = isBlock;
                access.SaveChanges();
                if (isBlock)
                    _repository.SetLogout(userId);
            }
            return new JsonResult() { Data = new { result = true }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult KickUser(Guid userId)
        {
            return new JsonResult() { Data = new { data = true }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult ResetPass(Guid userId, string pass)
        {
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var user = context.Users.FirstOrDefault(m => m.UserId == userId);
                if (string.IsNullOrEmpty(pass))
                {
                    pass = _repository.GeneratePass(8);
                }
                MembershipService.ChangePassword(user.Login, pass, pass);
                GeneralMailer mailer = new GeneralMailer();
                mailer.GeneralEmail("test", new string[] { user.Mail }, "test");
            }
            return new JsonResult() { Data = new { data = true }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}