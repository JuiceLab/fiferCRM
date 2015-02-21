using ConstructorLandingContext;
using EnumHelper.WebSite;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace SiteConstructor.Models
{
    public class ConstructorControllerFactory : DefaultControllerFactory
    {
        public override IController CreateController(RequestContext requestContext, string controllerName)
        {
            if (requestContext.RouteData.Values["site"] == null && !SiteSettingsWrapper.Instance.SiteIds.ContainsKey(controllerName))
                return base.CreateController(requestContext, controllerName);

            string site = SiteSettingsWrapper.Instance.SiteIds.ContainsKey(controllerName)?
                controllerName
                :requestContext.RouteData.Values["site"].ToString();
            
            var siteId = SiteSettingsWrapper.Instance.SiteIds[site];
            string type = ((SiteType)SiteSettingsWrapper.Instance.SiteTypes[siteId]).ToString();
            var controllerTemplate = SiteSettingsWrapper.Instance.SiteTemplates[siteId];
            if (requestContext.RouteData.Values["site"] == null)
            {
                requestContext.RouteData.Values["site"] = site;
                requestContext.RouteData.Values["controller"] = controllerTemplate;
            } 
            requestContext.RouteData.DataTokens.Add("area", type);
            var action = requestContext.RouteData.Values["anchor"] !=null?
                requestContext.RouteData.Values["anchor"].ToString()
                :string.Empty;
            requestContext.RouteData.Values["action"] = GetPageType(siteId, string.IsNullOrEmpty(action) ? "index" : action);
            Type controllerType = Type.GetType(string.Format("SiteConstructor.Areas.{0}.Controllers.{1}Controller", type, controllerTemplate));
            IController controller = Activator.CreateInstance(controllerType) as IController;
            return controller;
        }
        public override void ReleaseController(IController controller)
        {
            IDisposable dispose = controller as IDisposable;
            if (dispose != null)
            {
                dispose.Dispose();
            }
        }

        private string GetPageType( Guid siteId, string anchor)
        {
            string pageType = string.Empty;

            switch ((SiteType)SiteSettingsWrapper.Instance.SiteTypes[siteId])
            {
                case SiteType.PasteBoard:
                    pageType = GetPasteBoardPageType(siteId, anchor);
                    break;
                default:
                    break;
            }
            return pageType;
        }

        private string GetPasteBoardPageType(Guid siteId, string anchor)
        {
            string pageType = PageType.DefaultPage.ToString();

            var dbSource = SiteSettingsWrapper.Instance.SiteSources[siteId];
            using (PasteboardEntities context = new PasteboardEntities(AccessSettings.LoadSettings().LocalPasteboardEntities.Replace("fifer_pasteboard", dbSource)))
            {
                var page = context.PasteBoardPages.FirstOrDefault(m => m.PasteBoardSetting.SiteGuid == siteId && m.SeoItem.Anchor == anchor);
                if (page.SeoItem.Anchor == "info")
                    pageType = PageType.NoticesPage.ToString();
                else if (page.SeoItem.Anchor == "contact")
                    pageType = PageType.ContactPage.ToString();
                else if (page.C_GalleryId.HasValue && page.Gallery.IsActive)
                    pageType = page.Gallery.IsSlider ? PageType.SliderPage.ToString() : PageType.GalleryPage.ToString();
            }
            return pageType;
        }
    }
}
