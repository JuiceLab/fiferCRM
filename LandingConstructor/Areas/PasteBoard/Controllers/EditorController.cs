using ConstructorLandingContext;
using ConstructorRepositories;
using PasteBoardModel.EditorModel;
using SiteConstructor.Models;
using SiteModel;
using SiteModel.EditorModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SiteConstructor.Areas.PasteBoard.Controllers
{
    public class EditorController : Controller
    {
    
        public ActionResult SiteInfo(Guid siteId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            var model = repository.GetPasteboard4Edit(siteId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult SiteInfo(PasteBoardEditModel model)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[model.ProjectId]);
            repository.UpdatePasteboard4Edit(model);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PageSlider(Guid siteId, int pageId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            SliderEditModel model = repository.GetSliderEditPage(pageId);
            return PartialView(model);
        }

        public ActionResult PageGallery(Guid siteId, int pageId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            List<GalleryItem> model = repository.GetGalleryItems(pageId);
            return PartialView(model);
        }
       
        public ActionResult SlideEdit(Guid siteId, int itemId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            SliderItem model = repository.GetSliderItem(itemId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult SlideEdit(Guid siteId, int pageId, SliderItem model)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            repository.UpdateSliderItem(model, pageId);
            return RedirectToAction("PageSlider", new { pageId = pageId, siteId = siteId });
        }

        public ActionResult DeleteSlide(int itemId, Guid siteId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            int pageId;
            var path4Delete = Server.MapPath("~" + repository.DeleteImage(itemId, out pageId));
            if (System.IO.File.Exists(path4Delete))
                System.IO.File.Delete(path4Delete);
            return RedirectToAction("PageSlider", new { pageId = pageId, siteId = siteId });

        }

        public ActionResult ImageEdit(Guid siteId, int itemId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            GalleryItem model = repository.GetGalleryItem(itemId);
            return PartialView(model);
        }

        [HttpPost]
        public ActionResult ImageEdit(Guid siteId, int pageId, GalleryItem model)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            repository.UpdateGalleryItem(model, pageId);
            return RedirectToAction("PageGallery", new { pageId = pageId, siteId = siteId });
        }
        
        public ActionResult DeleteImage(int itemId, Guid siteId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            int pageId;
            var path4Delete = Server.MapPath("~" + repository.DeleteImage(itemId, out pageId));
            if (System.IO.File.Exists(path4Delete))
                System.IO.File.Delete(path4Delete);
           
            var item = new GalleryItem(){
                 Path = path4Delete
            };

            if (System.IO.File.Exists(item.FullSizePath))
                System.IO.File.Delete(item.FullSizePath);
            return RedirectToAction("PageGallery", new { pageId = pageId, siteId = siteId });
        }

        public ActionResult AddGallery(Guid siteId, int pageId, bool isSlider)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            PasteBoardSetting setting = repository.GetPasteboard(siteId);
            string siteImagesPath = Server.MapPath("~/Galleries/pasteboard/" + setting.HomePageUrl);
            if (!Directory.Exists(siteImagesPath))
                Directory.CreateDirectory(siteImagesPath);
            repository.UpdatePageGallery(siteId, pageId, "/Galleries/pasteboard/" + setting.HomePageUrl, isSlider);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SitePage(Guid siteId, int? pageId, int? parentId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            PasteBoardPageEditModel model = repository.GetPasteboardPage(siteId, pageId, parentId);
            return PartialView(model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult SitePage(PasteBoardPageEditModel model)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[model.ProjectId]);
            repository.UpdatePasteboardPage(model, (Guid)Membership.GetUser().ProviderUserKey);
            return Json(new { url = "http://" + HttpContext.Request.Url.Authority +"/"+ SiteSettingsWrapper.Instance.SiteIds.FirstOrDefault(mbox=>mbox.Value == model.ProjectId).Key + "/" + model.Anchor + ".html" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult NoticeImage(Guid siteId, int pageId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            string path = repository.GetNoticeImage(siteId, pageId);
            ViewBag.PageId = pageId;
            return PartialView("NoticeImage", path);
        }

        public ActionResult RemoveGallery(Guid siteId, int pageId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            var images4Delete = repository.TryRemoveGallery(pageId)
                .Select(m => Server.MapPath("~" + m));
            foreach (var path4Delete in images4Delete)
            {

                if (System.IO.File.Exists(path4Delete))
                    System.IO.File.Delete(path4Delete);

                var item = new GalleryItem()
                {
                    Path = path4Delete
                };

                if (System.IO.File.Exists(item.FullSizePath))
                    System.IO.File.Delete(item.FullSizePath);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RemovePage(Guid siteId, int pageId)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            var images4Delete = repository.TryRemoveGallery(pageId)
                .Select(m => Server.MapPath("~" + m));
            foreach (var path4Delete in images4Delete)
            {

                if (System.IO.File.Exists(path4Delete))
                    System.IO.File.Delete(path4Delete);

                var item = new GalleryItem()
                {
                    Path = path4Delete
                };

                if (System.IO.File.Exists(item.FullSizePath))
                    System.IO.File.Delete(item.FullSizePath);
            }
            repository.RemovePage(pageId);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult NoticeImage(Guid siteId, int pageId, string photoPath)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            PasteBoardSetting setting = repository.GetPasteboard(siteId);
            repository.UpdateNoticePage(pageId,"/Galleries/pasteboard/" + setting.HomePageUrl + "/" + photoPath);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult IsExistAnchor(Guid siteId,int pageId, string anchor)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            bool isExist = repository.IsExistAnchor(siteId, pageId, anchor);
            return Json(new { isExist = isExist }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveCss(Guid siteId, string css)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            repository.UpdateCss(siteId, css);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateGeo(Guid siteId, string latitude, string longitude)
        {
            PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
            repository.UpdateGeo(siteId, latitude, longitude);
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
    }
}