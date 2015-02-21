using AccessRepositories;
using CompanyRepositories;
using ContentSearchService;
using CRMModel;
using CRMRepositories;
using fifer_crm.Controllers;
using fifer_crm.Models;
using FilterModel;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TaskModel;

namespace fifer_crm.Areas.CRM.Controllers
{
    [Authorize, CRMLogAttribute]
    public class LegalEntityController : BaseFiferController
    {

        CRMRepository _repository = new CRMRepository();

        public ActionResult Index()
        {
            CRMWrapViewModel model = new CRMWrapViewModel(_userId);
            model.Menu = GetMenu("Компании");
            return View(model);
        }

        [HttpGet]
        public ActionResult GetHistory(Guid companyId)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            ViewBag.Name = repository.GetCompany(companyId).LegalName;
            List<MessageViewModel> model = repository.GetModifyLog(companyId);

            StaffRepository staffRepository = new StaffRepository();
            var photos = staffRepository.GetEmployeesPhoto(_userId);

            MembershipRepository membershipRepository = new MembershipRepository();
            var names = membershipRepository.GetUserByIds(model.Select(m=>m.UserId).Distinct());
            foreach (var item in model)
            {
                if (item.UserId != Guid.Empty)
                {
                    var owner = names.FirstOrDefault(m => m.UserId == item.UserId);
                    item.Title = owner != null ? string.Format("{0} {1}", owner.FirstName, owner.LastName) : string.Empty;
                    item.IconPath = photos[item.UserId];               
                }
            }
            return PartialView(model);
        }
        public ActionResult Edit(int? legalEnitityId)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            CRMCompanyEditModel model = repository.GetCompanyEditModel(legalEnitityId);
            InitSubordinatedUsers();
            ViewBag.Services = _repository.GetServices(new List<int>());
            
            CustomerSearch searchService = new CustomerSearch(_userId);
            
            Guid cityGuid;
            LegalEntitySearch search = searchService.GetDefaultLegalSearch(out cityGuid);
            var distr = _repository.GetDistricts();
            var distrId = _repository.GetDistrictByCity(cityGuid);
            if(distrId.HasValue)
                search.DistrictId = distrId.Value;
            distr.FirstOrDefault(m => m.Value == distrId.ToString()).Selected = true;
            
            ViewBag.Cities = _repository.GetCitiesSelectItems(search.DistrictId, cityGuid);
            ViewBag.Districts = distr;
            
            if (!legalEnitityId.HasValue)
            {
                return PartialView("NoveltyLegal", search);
            }
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult NoveltyLegal()
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);

            CRMCompanyEditModel model = repository.GetCompanyEditModel(null);

            StaffRepository staffRepository = new StaffRepository();
            ViewBag.Assign = staffRepository.GetSubordinatedUsers(_userId);
            ViewBag.Services = _repository.GetServices(new List<int>());
           
            
            return PartialView("Edit", model);
        }


        [HttpPost]
        public ActionResult NoveltyLegalEdit(LegalEntitySearch search)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);

            CRMCompanyEditModel model = repository.GetCompanyEditModel(null);
            model.Details[0].INN = search.INN.HasValue? search.INN.Value : 0;
            model.Details[0].KPP = search.KPP.HasValue ? search.KPP.Value : 0;
            model.Details[0].OGRN = search.OGRN.HasValue ? search.OGRN.Value : 0;
            model.Details[0].RS = search.RS.HasValue ? search.RS.Value : 0;

            model.LegalName = search.CompanyName;
            model.Sites = search.WebSite;
            model.Phones = search.Phone;
            model.Mails = search.EMail;

            StaffRepository staffRepository = new StaffRepository();
            ViewBag.Assign = staffRepository.GetSubordinatedUsers(_userId);
            ViewBag.Services = _repository.GetServices(new List<int>());
            ViewBag.Districts = _repository.GetDistricts();

            CustomerSearch searchService = new CustomerSearch(_userId);
            Guid cityGuid;
            LegalEntitySearch searchLegal = searchService.GetDefaultLegalSearch(out cityGuid);
            ViewBag.Cities = _repository.GetCitiesSelectItems(searchLegal.DistrictId, cityGuid);
            return PartialView("Edit", model);
        }


        [HttpPost]
        public ActionResult Edit(CRMCompanyEditModel model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);

            repository.UpdateCompany(model, _userId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Assign(int companyId)
        {
            InitSubordinatedUsers();
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            CRMCompanyEditModel model = repository.GetCompanyEditModel(companyId);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult Assign(CRMCompanyEditModel model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            StaffRepository staffRepository = new StaffRepository();
           var names =  staffRepository.GetSubordinatedUsers(_userId).ToDictionary(m=>Guid.Parse(m.Value), m=>m.Text);
           repository.ChangeAssign(model, _userId, names);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ChangeStatus(int companyId)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            CRMCompanyEditModel model = repository.GetCompanyEditModel(companyId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ChangeStatus(CRMCompanyEditModel model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            repository.ChangeStatus(model, _userId);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult EditComment(int companyId)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            CRMCompanyEditModel model = repository.GetCompanyEditModel(companyId);

            return PartialView(model);
        }

        [HttpPost]
        public ActionResult EditComment(CRMCompanyEditModel model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            repository.UpdateComment(model, _userId);
            return Json(new { status = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult CopyLocal(Guid companyId)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);
            repository.CopyCompanyLocal(companyId, _userId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LegalSearch(LegalEntitySearch model)
        {
            CRMLocalRepository repository = new CRMLocalRepository(_userId);

            IEnumerable<LegalEntitySearch> items = repository.GetLegalCompanyPreviews(model);
            return PartialView(items);
        }
    }
}