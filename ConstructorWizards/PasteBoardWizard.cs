using CompanyRepositories;
using ConstructorLandingContext;
using ConstructorRepositories;
using Interfaces.ConstructorBase;
using PasteBoardModel;
using Settings;
using SiteModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ConstructorWizards
{
    public class PasteBoardWizard
    {
        private CompanyRepository _repository = new CompanyRepository();
        private PasteBoardRepository _pasteboardRepositroy = null;
       
        public PasteBoardWizard(string connStr)
        {
            _pasteboardRepositroy = new PasteBoardRepository(connStr);
        }

        public ISliderPage GetSliderPage(string site, string anchor)
        {
            IPage page = GetPage(site, anchor);
            ISliderPage result = new SliderPage(page)
            {
                SliderItems = GetSliders(page.PageId)
            };
            return result;
        }

        public IGalleryPage GetGalleryPage(string site, string anchor)
        {
            IPage page = GetPage(site, anchor);
            var existPage = _pasteboardRepositroy.GetPage(page.PageId);

            IGalleryPage result = new GalleryPage(page)
            {
                GalleryItems = GetGaleryItems(page.PageId),
                GalleryName = existPage.Gallery.Name,
                GalleryPath = existPage.Gallery.ItemsPath
            };
            return result;
        }

        private IList<IGalleryItem> GetGaleryItems(int pageId)
        {
            var existPage = _pasteboardRepositroy.GetPage(pageId);
            return existPage.Gallery.Images.Select(m => new GalleryItem()
            {
                 ItemId =m.ImageId,
                  Alt = m.Alt,
                    Path = m.Path,
                     Title = m.Title
            }).Cast<IGalleryItem>()
            .ToList();
        }
        private IList<ISliderItem> GetSliders(int pageId)
        {
            var page = _pasteboardRepositroy.GetPage(pageId);

            return page.Gallery.Images
                .ToList()
                .Select(m => new SliderItem()
             {
                 ItemId = m.ImageId,
                 Title = m.Title,
                 IsBlank = m.C_SlideSettingId.HasValue? m.SlideSetting.IsBlank : false,
                 PhotoPath = m.C_SlideSettingId.HasValue?  m.Gallery.ItemsPath + "/" + m.Path : m.Path,
                 Pos = m.C_SlideSettingId.HasValue? m.SlideSetting.Position : 1,
                 RedirectAnchor = m.C_SlideSettingId.HasValue? m.SlideSetting.UrlRedirect :""
             })
             .Cast<ISliderItem>()
             .ToList();
        }

        public IPage GetPage(string site, string anchor)
        {
            var webSite = _repository.GetSiteProject(site);
            var page = _pasteboardRepositroy.GetPage(webSite.SiteGuid, anchor);
            Template template = null;
            using (TemplateEntities entities = new TemplateEntities(AccessSettings.LoadSettings().TemplateEntities))
            {
                template = entities.Templates.FirstOrDefault(m => m.TemplateGuid == page.PasteBoardSetting.TemplateGuid);
            }
            IPage defaultPage = new DefaultPage()
            {
                PageId = page.PasteBoardPageId,
                SiteId = webSite.SiteGuid,
                Content = page.PageContent,
                CssPostfix = page.PasteBoardSetting.CssCustom,
                H1 = page.SeoItem.Title,
                SeoItem = GetSeo(page.SeoItem),
                IsHtml = page.IsHtml,
                LayoutName = template.LayoutName,
                Menu = GetMenu(page.PasteBoardSetting, anchor),
                Footer = GetFooter(page.PasteBoardSetting),
                Header = GetHeader(page.PasteBoardSetting),
                IsSocialRepostItems = page.IsSocialRepost
            };
            defaultPage.SeoItem.Title += string.Format(": {0} - {1}", webSite.SiteName, page.PasteBoardSetting.CityName);
            return defaultPage;
        }

        public INoticesPage GetNoticesPage(string site, string anchor)
        {
            IPage page = GetPage(site, anchor);
            INoticesPage noticesPage = new NoticesPage(page)
            {
                Notices = GetNoticesPage(page.PageId)
            };
            return noticesPage;
        }

        public IContactPage GetContactPage(string site, string anchor)
        {
            IPage page = GetPage(site, anchor);
            var existPage = _pasteboardRepositroy.GetPage(page.PageId);
            IContactPage noticesPage = new ContactPage(page)
            {
                Latitude = existPage.PasteBoardSetting.Latitiude,
                Longitude = existPage.PasteBoardSetting.Longitude,
            };
            return noticesPage;
        }

        private List<INoticeItem> GetNoticesPage(int pageId)
        {
          return  _pasteboardRepositroy.GetPageNotices4Parent(pageId);
        }

        private ISiteHeader GetHeader(PasteBoardSetting pasteBoardSetting)
        {
            return new SiteHeader()
            {
                HeaderImage = pasteBoardSetting.HeaderImagePath,
                LogoPath = pasteBoardSetting.IconPath,
                Name = pasteBoardSetting.Name,
                Phone = pasteBoardSetting.Phone,
                Thesis = pasteBoardSetting.CompanyTesis,
                CityName = pasteBoardSetting.CityName
            };
        }

        private ISiteFooter GetFooter(PasteBoardSetting pasteBoardSetting)
        {
            return new SiteFooter()
            {
                Address = pasteBoardSetting.Address,
                Mail = pasteBoardSetting.Mail,
                Name = pasteBoardSetting.Name,
                Phone = pasteBoardSetting.Phone
            };
        }

        private IList<ISiteMenuItem> GetMenu(PasteBoardSetting pasteBoardSetting, string curAnchor)
        {
            return pasteBoardSetting.PasteBoardPages.Where(m => m.IsAvailable && m.IsMenuItem)
                 .Select(m => new SiteMenuItem()
                 {
                     Icon = string.Empty,
                     IsCurrent = m.SeoItem.Anchor == curAnchor,
                     IsActive = true,
                     MenuItemId = m.PasteBoardPageId,
                     Name = m.SeoItem.Title,
                     Url = pasteBoardSetting.HomePageUrl + "/" + m.SeoItem.Anchor,
                     ParentMenuItemId = m.C_ParentPageId
                 })
                 .Cast<ISiteMenuItem>()
                 .ToList();
        }

        private ISeoItem GetSeo(SeoItem seoItem)
        {
            return new SiteSeoItem()
            {
                Title = seoItem.Title,
                Description = seoItem.Description,
                Keywords = seoItem.Keywords,
                SeoItemId = seoItem.SeoItemId
            };
        }


        public List<SelectListItem> GetCssChoice(int categoryId)
        {
            List<SelectListItem> items = new List<SelectListItem>();
            using (TemplateEntities entities = new TemplateEntities(AccessSettings.LoadSettings().TemplateEntities))
            {
                var category = entities.CssCategories.FirstOrDefault(m => m.CssCategoryId == categoryId );
                items.AddRange(category.CssStyles
                    .Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(m => new SelectListItem()
                {
                    Text = m.Trim(),
                    Value = m.Trim()
                }));
            }
            return items;
        }

        public List<SelectListItem> GetCssCategories(string templateName)
        {
            List<SelectListItem> items = new List<SelectListItem>() {  new SelectListItem(){ Text="Выберите категорию стилей"}};
            using (TemplateEntities entities = new TemplateEntities(AccessSettings.LoadSettings().TemplateEntities))
            {
                var template = entities.Templates.FirstOrDefault(m => m.Name == templateName);
                items.AddRange(template.CssCategories
                    .ToList()
                    .Select(m => new SelectListItem()
                    {
                        Text = m.CategoryName,
                        Value = m.CssCategoryId.ToString()
                    }));
            }
            return items;
        }

     
    }
}
