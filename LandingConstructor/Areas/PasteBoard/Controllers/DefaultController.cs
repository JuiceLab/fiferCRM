using ConstructorRepositories;
using ConstructorWizards;
using SiteConstructor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SiteConstructor.Areas.PasteBoard.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult DefaultPage(string site, string anchor)
        {
            try
            {
                if (string.IsNullOrEmpty(anchor))
                {
                    anchor = "index";
                }
                var siteId = SiteSettingsWrapper.Instance.SiteIds[site];
                PasteBoardWizard wizard = new PasteBoardWizard(SiteSettingsWrapper.Instance.SiteSources[siteId]);
                var page = wizard.GetPage(site, anchor);
                if (User.Identity.IsAuthenticated)
                {
                    page.UserId = (Guid)Membership.GetUser().ProviderUserKey;
                }
                ViewBag.Domain = site;
                ViewBag.CssCategories = wizard.GetCssCategories(SiteSettingsWrapper.Instance.SiteTemplates[siteId]);
                return View(page);
            }
            catch
            { }
            return View();
        }

        public ActionResult SliderPage(string site, string anchor)
        {
            try
            {
                var siteId = SiteSettingsWrapper.Instance.SiteIds[site];
                PasteBoardWizard wizard = new PasteBoardWizard(SiteSettingsWrapper.Instance.SiteSources[siteId]);
                var page = wizard.GetSliderPage(site, anchor);
                if (User.Identity.IsAuthenticated)
                {
                    page.UserId = (Guid)Membership.GetUser().ProviderUserKey;
                }
                ViewBag.Domain = site;
                ViewBag.CssCategories = wizard.GetCssCategories(SiteSettingsWrapper.Instance.SiteTemplates[siteId]);
                return View(page);
            }
            catch
            { }
            return View();
        }

        public ActionResult GalleryPage(string site, string anchor)
        {
            try
            {
                var siteId = SiteSettingsWrapper.Instance.SiteIds[site];
                PasteBoardWizard wizard = new PasteBoardWizard(SiteSettingsWrapper.Instance.SiteSources[siteId]);
                var page = wizard.GetGalleryPage(site, anchor);
                if (User.Identity.IsAuthenticated)
                {
                    page.UserId = (Guid)Membership.GetUser().ProviderUserKey;
                }
                ViewBag.Domain = site;
                ViewBag.CssCategories = wizard.GetCssCategories(SiteSettingsWrapper.Instance.SiteTemplates[siteId]);
                return View(page);
            }
            catch
            { }
            return View();
        }

        public ActionResult ContactPage(string site, string anchor)
        {
            try
            {
                var siteId = SiteSettingsWrapper.Instance.SiteIds[site];
                PasteBoardWizard wizard = new PasteBoardWizard(SiteSettingsWrapper.Instance.SiteSources[siteId]);
                var page = wizard.GetContactPage(site, anchor);
                if (User.Identity.IsAuthenticated)
                {
                    page.UserId = (Guid)Membership.GetUser().ProviderUserKey;
                }
                ViewBag.Domain = site;
                ViewBag.CssCategories = wizard.GetCssCategories(SiteSettingsWrapper.Instance.SiteTemplates[siteId]);
                return View(page);
            }
            catch
            { }
            return View();
        }

        public ActionResult NoticesPage(string site, string anchor)
        {
            try
            {
                var siteId = SiteSettingsWrapper.Instance.SiteIds[site];
                PasteBoardWizard wizard = new PasteBoardWizard(SiteSettingsWrapper.Instance.SiteSources[siteId]);
                var page = wizard.GetNoticesPage(site, anchor);
                if (User.Identity.IsAuthenticated)
                {
                    page.UserId = (Guid)Membership.GetUser().ProviderUserKey;
                }
                ViewBag.Domain = site;
                ViewBag.CssCategories = wizard.GetCssCategories(SiteSettingsWrapper.Instance.SiteTemplates[siteId]);
                return View(page);
            }
            catch
            { }
            return View();
        }
    }
}