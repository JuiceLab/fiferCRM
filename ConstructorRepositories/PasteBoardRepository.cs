using ConstructorLandingContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityRepository;
using CompanyContext;
using Settings;
using EnumHelper.WebSite;
using Interfaces.ConstructorBase;
using PasteBoardModel.EditorModel;
using SiteModel;
using SiteModel.EditorModel;
using System.Globalization;
using ExtensionHelpers;

namespace ConstructorRepositories
{
    public class PasteBoardRepository : BaseConstructorRepository
    {
        private string _defaultTemplate = "_DefaultPasteBoardLayout.chtml";
        private const string _fishWords = "Товарищи! рамки и место обучения кадров влечет за собой процесс внедрения и модернизации дальнейших направлений развития. С другой стороны начало повседневной работы по формированию позиции играет важную роль в формировании существенных финансовых и административных условий.Повседневная практика показывает, что новая модель организационной деятельности позволяет оценить значение позиций, занимаемых участниками в отношении поставленных задач. Таким образом рамки и место обучения кадров представляет собой интересный эксперимент проверки модели развития. Идейные соображения высшего порядка, а также постоянный количественный рост и сфера нашей активности влечет за собой процесс внедрения и модернизации позиций, занимаемых участниками в отношении поставленных задач. Равным образом сложившаяся структура организации представляет собой интересный эксперимент проверки позиций, занимаемых участниками в отношении поставленных задач. Разнообразный и богатый опыт сложившаяся структура организации требуют определения и уточнения направлений прогрессивного развития. С другой стороны постоянное информационно-пропагандистское обеспечение нашей деятельности требуют от нас анализа соответствующий условий активизации.Товарищи! консультация с широким активом позволяет выполнять важные задания по разработке дальнейших направлений развития. Не следует, однако забывать, что рамки и место обучения кадров в значительной степени обуславливает создание существенных финансовых и административных условий. С другой стороны постоянный количественный рост и сфера нашей активности позволяет оценить значение модели развития.Не следует, однако забывать, что новая модель организационной деятельности требуют от нас анализа модели развития. Равным образом новая модель организационной деятельности обеспечивает широкому кругу (специалистов) участие в формировании существенных финансовых и административных условий. Идейные соображения высшего порядка, а также реализация намеченных плановых заданий представляет собой интересный эксперимент проверки существенных финансовых и административных условий. Идейные соображения высшего порядка, а также сложившаяся структура организации способствует подготовки и реализации соответствующий условий активизации. Таким образом рамки и место обучения кадров играет важную роль в формировании новых предложений. Таким образом сложившаяся структура организации требуют определения и уточнения направлений прогрессивного развития. Не следует, однако забывать, что рамки и место обучения кадров требуют определения и уточнения дальнейших направлений развития. Равным образом новая модель организационной деятельности способствует подготовки и реализации соответствующий условий активизации. Идейные соображения высшего порядка, а также постоянный количественный рост и сфера нашей активности в значительной степени обуславливает создание соответствующий условий активизации.";
        public PasteboardEntities PasteboardContext { get; set; }

        public PasteBoardRepository(string dbSource)
            :base(dbSource)
        {
            PasteboardContext = new PasteboardEntities(AccessSettings.LoadSettings().LocalPasteboardEntities.Replace("fifer_pasteboard", dbSource));
        }

        public void CreateCompanyDB(Guid siteGuid, string dbSource, string cityName)
        {

            Template template;
            using (TemplateEntities templateContext = new TemplateEntities(AccessSettings.LoadSettings().TemplateEntities))
            {
                template = templateContext.Templates.FirstOrDefault(m => m.Type == (byte)SiteType.PasteBoard);
            }
            using (CompanyEntities companyContext = new CompanyEntities(AccessSettings.LoadSettings().CompanyEntites))
            {
                var site = companyContext.WebSites.FirstOrDefault(m => m.SiteGuid == siteGuid);
                var addr = site.Company.Address;
                using (PasteboardEntities context = new PasteboardEntities(AccessSettings.LoadSettings().LocalPasteboardEntities.Replace("fifer_pasteboard", site.DbSource)))
                {
                    context.Database.Create();
                    var settings = new PasteBoardSetting()
                    {
                        CompanyName = site.Company.CompanyName,
                        Address = addr.Street + " " + addr.Number,
                        Name = site.SiteName,
                        HomePageUrl = site.Url,
                        Phone = site.Company.Address.Phones,
                        CompanyTesis = string.Empty,
                        CssCustom = "default.css",
                        IconPath = "default.ico",
                        SiteGuid = siteGuid,
                        TemplateGuid = template.TemplateGuid,
                        BackgroundImage = string.Empty,
                        TemplateName = "Default",
                        CityName = cityName,
                        Latitiude = addr.Latitiude,
                        Longitude = addr.Longitude
                    };
                    context.InsertUnit(settings);
                  
                    var indexSeo = new SeoItem()
                    {
                        Anchor = "index",
                        Title = "Главная"
                    };
                    AddOrUpdateSeoItem(indexSeo);
                    var page = new PasteBoardPage()
                    {
                        IsAvailable = true,
                        IsMenuItem = true,
                        C_SettingsId = settings.SiteSettingsId,
                        C_SeoItemId = indexSeo.SeoItemId,
                        CreatedBy = site.CreatedBy,
                        Created = DateTime.Now,
                        PageContent = _fishWords,
                        IsHtml = false,
                        IsSocialRepost = false
                    };
                    AddOrUpdatePage(page);

                    var aboutSeo = new SeoItem()
                    {
                        Anchor = "about",
                        Title = "О компании"
                    };
                    AddOrUpdateSeoItem(aboutSeo);
                    var pageAbout = new PasteBoardPage()
                    {
                        IsAvailable = true,
                        IsMenuItem = true,
                        C_SettingsId = settings.SiteSettingsId,
                        C_SeoItemId = aboutSeo.SeoItemId,
                        CreatedBy = site.CreatedBy,
                        Created = DateTime.Now,
                        PageContent = _fishWords,
                        IsHtml = false,
                        IsSocialRepost = true
                    };
                    AddOrUpdatePage(pageAbout);

                    var infoSeo = new SeoItem()
                    {
                        Anchor = "info",
                        Title = "Информация"
                    };
                    AddOrUpdateSeoItem(infoSeo);
                    var infoPage = new PasteBoardPage()
                    {
                        IsAvailable = true,
                        IsMenuItem = true,
                        C_SettingsId = settings.SiteSettingsId,
                        C_SeoItemId = infoSeo.SeoItemId,
                        CreatedBy = site.CreatedBy,
                        Created = DateTime.Now,
                        PageContent = string.Empty,
                        IsHtml = false,
                        IsSocialRepost = false
                    };
                    AddOrUpdatePage(infoPage);

                    var contactSeo = new SeoItem()
                    {
                        Anchor = "contact",
                        Title = "Контакты"
                    };

                    AddOrUpdateSeoItem(contactSeo);
                    var pageContact = new PasteBoardPage()
                    {
                        IsAvailable = true,
                        IsMenuItem = true,
                        C_SettingsId = settings.SiteSettingsId,
                        C_SeoItemId = contactSeo.SeoItemId,
                        CreatedBy = site.CreatedBy,
                        Created = DateTime.Now,
                        PageContent = _fishWords,
                        IsHtml = false,
                        IsSocialRepost = true
                    };
                    AddOrUpdatePage(pageContact);
                }
            }
        }

        public PasteBoardPage GetPage(Guid siteGuid, string anchor)
        {
            return  PasteboardContext.PasteBoardPages.FirstOrDefault(m=>m.PasteBoardSetting.SiteGuid == siteGuid && m.SeoItem.Anchor == anchor);
        }

        public PasteBoardPage GetPage(int pageId)
        {
            return PasteboardContext.GetUnitById<PasteBoardPage>(pageId);
        }

        public PasteBoardEditModel GetPasteboard4Edit(Guid siteGuid)
        {
            var pasteBoard = PasteboardContext.PasteBoardSettings.FirstOrDefault(m => m.SiteGuid == siteGuid);

            PasteBoardEditModel model = new PasteBoardEditModel()
            {
                ProjectId = siteGuid,
                Mail = pasteBoard.Mail,
                Phone = pasteBoard.Phone,
                Name = pasteBoard.Name,
                CityName = pasteBoard.CityName,
                Address = pasteBoard.Address,
                CompanyName = pasteBoard.CompanyName,
                Thesis = pasteBoard.CompanyTesis,
                HeaderImagePath = pasteBoard.HeaderImagePath,
                LogoPath = pasteBoard.IconPath
            };
            return model;
        }

        public void UpdatePasteboard4Edit(PasteBoardEditModel pasteBoard)
        {
            var model = PasteboardContext.PasteBoardSettings.FirstOrDefault(m => m.SiteGuid == pasteBoard.ProjectId);
            model.Mail = pasteBoard.Mail;
            model.Phone = pasteBoard.Phone;
            model.Name = pasteBoard.Name;
            model.CityName = pasteBoard.CityName;
            model.Address = pasteBoard.Address;
            model.CompanyName = pasteBoard.CompanyName;
            model.CompanyTesis = pasteBoard.Thesis;
            model.HeaderImagePath = pasteBoard.HeaderImagePath;
            model.IconPath = pasteBoard.LogoPath;
            PasteboardContext.SaveChanges();
        }

        public int AddOrUpdateSeoItem(SeoItem item)
        {

            if (item.SeoItemId == 0)
            {
                PasteboardContext.InsertUnit(item);
            }
            else
            {
                PasteboardContext.UpdateUnit(item);
            }
            return item.SeoItemId;
        }

        public void AddOrUpdatePage(PasteBoardPage page)
        {

            if (page.PasteBoardPageId == 0)
            {
                PasteboardContext.InsertUnit(page);
            }
        }

        public PasteBoardPageEditModel GetPasteboardPage(Guid siteId, int? pageId, int? parentId)
        {
            PasteBoardPageEditModel model = new PasteBoardPageEditModel()
            {
                ProjectId = siteId,
                ParentId = parentId,
                PositionId = !parentId.HasValue ? new Nullable<int>()
                :PasteboardContext.PasteBoardPages.Any(m => m.PositionId.HasValue && m.C_ParentPageId == parentId.Value)?
                    PasteboardContext.PasteBoardPages.Where(m => m.PositionId.HasValue && m.C_ParentPageId == parentId.Value).Max(m => m.PositionId) + 1 
                : 0
            };
            if(pageId.HasValue)
            {
                var existPage = PasteboardContext.PasteBoardPages.FirstOrDefault(m => m.PasteBoardSetting.SiteGuid == siteId && m.PasteBoardPageId == pageId.Value);
                model.PageId = pageId.Value;
                model.Notice = existPage.Notice;
                model.Anchor = existPage.SeoItem.Anchor;
                model.Title = existPage.SeoItem.Title;
                model.Text = existPage.PageContent;
                model.Description = existPage.SeoItem.Description;
                model.Keywords = existPage.SeoItem.Keywords;
                model.IsSocialRepost = existPage.IsSocialRepost;
                model.PositionId = existPage.PositionId;
            }
            return model;
        }

        public void UpdatePasteboardPage(PasteBoardPageEditModel model, Guid userId)
        {
            if (model.PageId == 0)
            {
                SeoItem seoItem = new SeoItem()
                {
                    Title = model.Title,
                    Anchor = model.Anchor,
                    Description = model.Description,
                    Keywords = model.Keywords
                };
                PasteboardContext.InsertUnit(seoItem);
                PasteboardContext.InsertUnit(new PasteBoardPage()
                {
                    C_SettingsId = PasteboardContext.PasteBoardSettings.FirstOrDefault(m => m.SiteGuid == model.ProjectId).SiteSettingsId,
                    IsHtml = false,
                    IsAvailable = true,
                    IsMenuItem = true,
                    PageContent = model.Text,
                    CreatedBy = userId,
                    Notice = model.Notice,
                    Created = DateTime.Now,
                    IsSocialRepost = model.IsSocialRepost,
                    C_SeoItemId = seoItem.SeoItemId,
                    C_ParentPageId = model.ParentId,
                    PositionId = model.PositionId
                });
            }
            else
            {
                var existPage = PasteboardContext.PasteBoardPages.FirstOrDefault(m => m.PasteBoardSetting.SiteGuid == model.ProjectId && m.PasteBoardPageId == model.PageId);
                existPage.SeoItem.Title = model.Title;
                if (existPage.SeoItem.Anchor != "index" && existPage.SeoItem.Anchor != "contact" && existPage.SeoItem.Anchor != "info")
                {
                    existPage.SeoItem.Anchor = model.Anchor;
                }
                else
                {
                    model.Anchor = existPage.SeoItem.Anchor;
                }
                existPage.SeoItem.Description = model.Description;
                existPage.SeoItem.Keywords = model.Keywords;
                existPage.IsSocialRepost = model.IsSocialRepost;
                existPage.PageContent = model.Text;
                existPage.Notice = model.Notice;
                PasteboardContext.SaveChanges();
            }
        }

        public bool IsExistAnchor(Guid siteId, int pageId, string anchor)
        {
            return PasteboardContext.PasteBoardPages
                .Any(m => m.PasteBoardPageId != pageId
                && m.PasteBoardSetting.SiteGuid == siteId
                && m.SeoItem.Anchor == anchor);
        }

        public PasteBoardSetting GetPasteboard(Guid siteId)
        {
            return PasteboardContext.PasteBoardSettings.FirstOrDefault(m => m.SiteGuid == siteId);
        }

        public void UpdatePageGallery(Guid siteId, int pageId, string siteImagesPath, bool isSlider)
        {
            var page = PasteboardContext.PasteBoardPages.FirstOrDefault(m => m.PasteBoardSetting.SiteGuid == siteId && m.PasteBoardPageId == pageId);

            if (!page.C_GalleryId.HasValue)
            {
                Gallery gallery = gallery = new Gallery()
                            {
                                IsActive = true,
                                IsSlider = isSlider,
                                ItemsPath = siteImagesPath,
                                Name = page.SeoItem.Title
                            };
                PasteboardContext.InsertUnit(gallery);
                page.C_GalleryId = gallery.GalleryId;
                if (isSlider)
                {
                    PasteboardContext.InsertUnit(new Image()
                    {
                        C_GalleryId = gallery.GalleryId,
                        Path = "/assets/pasteboard/content/demoslide/sl1.jpg",
                        Title = "Demo3"
                    });
                    PasteboardContext.InsertUnit(new Image()
                    {
                        C_GalleryId = gallery.GalleryId,
                        Path = "/assets/pasteboard/content/demoslide/sl2.jpg",
                        Title = "Demo1"
                    });
                    PasteboardContext.InsertUnit(new Image()
                    {
                        C_GalleryId = gallery.GalleryId,
                        Path = "/assets/pasteboard/content/demoslide/sl3.png",
                        Title = "Demo3"
                    });
                }
            }
            else
            {
                page.Gallery.IsSlider = isSlider;
                page.Gallery.IsActive = !page.Gallery.IsActive;
            }
            PasteboardContext.SaveChanges();
        }

        public List<INoticeItem> GetPageNotices4Parent(int pageId)
        {
            return PasteboardContext.PasteBoardPages.Where(m => m.C_ParentPageId == pageId)
                .ToList()
                .Select(m => new NoticeItem()
                {
                    PageId = m.PasteBoardPageId,
                    PositionId = m.PositionId.Value,
                    NoticeContent = m.Notice,
                    NoticeImagePath = m.PhotoPath,
                    Title = m.SeoItem.Title,
                    UrlRedirect = m.SeoItem.Anchor, 
                })
                .Cast<INoticeItem>()
                .ToList();
        }

        public void UpdateCss(Guid siteId, string css)
        {
            PasteboardContext.PasteBoardSettings.FirstOrDefault(m => m.SiteGuid == siteId).CssCustom = css;
            PasteboardContext.SaveChanges();
        }

        public SliderEditModel GetSliderEditPage(int pageId)
        {
            var page = GetPage(pageId);

            return new SliderEditModel()
            {
                PageId = pageId,
                SliderItems = page.Gallery.Images
                               .ToList()
                               .Select(m => new SliderItem()
                               {
                                   ItemId = m.ImageId,
                                   Title = m.Title,
                                   IsBlank = m.C_SlideSettingId.HasValue ? m.SlideSetting.IsBlank : false,
                                   PhotoPath = m.C_SlideSettingId.HasValue ? m.Gallery.ItemsPath + "/" + m.Path : m.Path,
                                   Pos = m.C_SlideSettingId.HasValue ? m.SlideSetting.Position : 1,
                                   RedirectAnchor = m.C_SlideSettingId.HasValue ? m.SlideSetting.UrlRedirect : ""
                               })
                            .ToList()
            };
        }

        public SliderItem GetSliderItem(int itemId)
        {
            SliderItem model = new SliderItem();
            if (itemId != 0)
            {
                var image = PasteboardContext.GetUnitById<Image>(itemId);
                model.ItemId = image.ImageId;
                model.Title = image.Title;
                model.IsBlank = image.C_SlideSettingId.HasValue ? image.SlideSetting.IsBlank : false;
                model.PhotoPath = image.Path;
                model.Pos = image.C_SlideSettingId.HasValue ? image.SlideSetting.Position : 1;
                model.RedirectAnchor = image.C_SlideSettingId.HasValue ? image.SlideSetting.UrlRedirect : "";
            };
            return model;
        }

        public void UpdateSliderItem(SliderItem model, int pageId)
        {
            if (model.ItemId != 0)
            {
                var image = PasteboardContext.GetUnitById<Image>(model.ItemId);
                image.Title = model.Title;
                image.Path = model.PhotoPath;
                image.Alt = model.Title;
                if (!image.C_SlideSettingId.HasValue)
                {

                    SlideSetting setting = new SlideSetting()
                    {
                        IsBlank = model.IsBlank,
                        Position = model.Pos,
                        UrlRedirect = model.RedirectAnchor
                    };
                    PasteboardContext.InsertUnit(setting);
                    image.C_SlideSettingId = setting.SlideSettingsId;
                }
                else
                {
                    image.SlideSetting.IsBlank = model.IsBlank;
                    image.SlideSetting.Position = model.Pos;
                    image.SlideSetting.UrlRedirect = model.RedirectAnchor;
                }
                PasteboardContext.SaveChanges();
            }
            else
            {
                SlideSetting setting = new SlideSetting()
                {
                    IsBlank = model.IsBlank,
                    Position = model.Pos,
                    UrlRedirect = model.RedirectAnchor
                };
                PasteboardContext.InsertUnit(setting);
                PasteboardContext.InsertUnit(new Image()
                {
                    Title = model.Title,
                    Alt = model.Title,
                    Path = model.PhotoPath,
                    C_GalleryId = PasteboardContext.GetUnitById<PasteBoardPage>(pageId).C_GalleryId.Value,
                    C_SlideSettingId = setting.SlideSettingsId
                });
            }
        }

        public string DeleteImage(int itemId, out int pageId)
        {
            
            var image = PasteboardContext.Images.FirstOrDefault(m => m.ImageId == itemId);
            string path4delete = image.Gallery.ItemsPath + "/" + image.Path;
            pageId = image.Gallery.PasteBoardPages.FirstOrDefault().PasteBoardPageId;
            if (image.C_SlideSettingId.HasValue)
                PasteboardContext.DeleteUnit(image.SlideSetting);
            PasteboardContext.DeleteUnit(image);
            return path4delete;
        }

        public string GetGalleryNewImagePath(int pageId)
        {
            var page = PasteboardContext.GetUnitById<PasteBoardPage>(pageId);
            var gallery = page.Gallery;
            var uniquieImageName = string.Empty;
            if (gallery != null)
            {
                string name = TransliterationHelper.Front(gallery.Name);
                var countImages = gallery.Images.Where(m => m.Path.StartsWith(name)).Count();
                var tmpName = name + (countImages + 1);
                uniquieImageName = tmpName;
                int i = 0;
                while (gallery.Images.Any(m => m.Path.StartsWith(tmpName)))
                {
                    tmpName = uniquieImageName + i;
                    i++;
                }
                return gallery.ItemsPath + "/" + tmpName;
            }
            else
                return page.SeoItem.Anchor;
        }

        public void UpdateGalleryItem(GalleryItem model, int pageId)
        {
            if (model.ItemId != 0)
            {
                var image = PasteboardContext.GetUnitById<Image>(model.ItemId);
                image.Title = model.Title;
                image.Path = model.Path;
                image.Alt = model.Title;
                PasteboardContext.SaveChanges();
            }
            else
            {
                PasteboardContext.InsertUnit(new Image()
                 {
                     Title = model.Title,
                     Alt = model.Title,
                     Path = model.Path,
                     C_GalleryId = PasteboardContext.GetUnitById<PasteBoardPage>(pageId).C_GalleryId.Value,
                 });
            }
        }

        public GalleryItem GetGalleryItem(int itemId)
        {
            GalleryItem model = new GalleryItem();
            if (itemId != 0)
            {
                var image = PasteboardContext.GetUnitById<Image>(itemId);
                model.ItemId = image.ImageId;
                model.Title = image.Title;
                model.Path = image.Path;
            };
            return model;
        }

        public List<GalleryItem> GetGalleryItems(int pageId)
        {
            var page = GetPage(pageId);

            return page.Gallery.Images
                               .ToList()
                               .Select(m => new GalleryItem()
                               {
                                   ItemId = m.ImageId,
                                   Title = m.Title,
                                   Path = m.Gallery.ItemsPath + "/"  + m.Path
                               })
                            .ToList();
        }

        public void UpdateGeo(Guid siteId, string latitude, string longitude)
        {
            IFormatProvider provider = new CultureInfo("en-US");
            if (!latitude.Contains("."))
            {
                latitude = latitude.Substring(0, 2) + "." + latitude.Substring(2);
            }
            if (!longitude.Contains("."))
            {
                longitude = longitude.Substring(0, 2) + "." + longitude.Substring(2);
            }
           var settings= PasteboardContext.PasteBoardSettings.FirstOrDefault(m => m.SiteGuid == siteId);
           settings.Latitiude = Convert.ToDouble(latitude, provider);
           settings.Longitude = Convert.ToDouble(longitude, provider);
           PasteboardContext.SaveChanges();
        }

        public string GetNoticeImage(Guid siteId, int pageId)
        {
            return PasteboardContext.PasteBoardPages
                .FirstOrDefault(m => m.PasteBoardPageId == pageId && m.PasteBoardSetting.SiteGuid == siteId)
                .PhotoPath;
        }

        public void UpdateNoticePage(int pageId, string photoPath)
        {
            PasteboardContext.PasteBoardPages
                    .FirstOrDefault(m => m.PasteBoardPageId == pageId).PhotoPath = photoPath;
            PasteboardContext.SaveChanges();
        }

        public IEnumerable<string> TryRemoveGallery(int pageId)
        {
            IEnumerable<string> result = new List<string>();
            var page = PasteboardContext.GetUnitById<PasteBoardPage>(pageId);
            if (page.C_GalleryId.HasValue && page.Gallery.Images.Count > 0 )
            {
                result = page.Gallery.Images.ToList().Select(m=>m.Gallery.ItemsPath + "/"  + m.Path).ToList();
            }
            PasteboardContext.DeleteUnits(page.Gallery.Images.Where(m=>m.C_SlideSettingId.HasValue).Select(m => m.SlideSetting));
            PasteboardContext.DeleteUnits(PasteboardContext.Images.Where(m=>m.C_GalleryId == page.Gallery.GalleryId));
            PasteboardContext.DeleteUnit(PasteboardContext.GetUnitById<Gallery>(page.Gallery.GalleryId));
            return result;
        }

        public void RemovePage(int pageId)
        {
            var page = PasteboardContext.GetUnitById<PasteBoardPage>(pageId);
            PasteboardContext.DeleteUnit(page.SeoItem);
            PasteboardContext.DeleteUnit(PasteboardContext.GetUnitById<PasteBoardPage>(pageId));
        }
    }
}
