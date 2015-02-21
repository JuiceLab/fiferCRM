using ConstructorLandingContext;
using ConstructorRepositories;
using ImageHelper;
using SiteConstructor.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace SiteConstructor.Areas.PasteBoard.Controllers
{
    public class ImageLoaderController : Controller
    {
        private int defaultBigWidth = 760;
        private int defaultBigHeight = 560;

        internal class PhotoSaveConfig
        {
            public string ConfigName { get; set; }
            public string Directory { get; set; }
            public int PhotoWidth { get; set; }
            public int PhotoHeight { get; set; }
            public PhotoSaveConfig()
            {
            }

            public PhotoSaveConfig(string configName, string directory, int photoWidth, int photoHeight)
            {
                this.ConfigName = configName;
                this.Directory = directory;
                this.PhotoWidth = photoWidth;
                this.PhotoHeight = photoHeight;
            }

        }

        private List<PhotoSaveConfig> Configuration = new List<PhotoSaveConfig>()
        {
            new PhotoSaveConfig("slide","",  960, 300),
            new PhotoSaveConfig("image","",  150, 200),
            new PhotoSaveConfig("notice","", 405, 405),
        };
        public ActionResult SaveCroped(string filename, string strStream, int pageId, Guid siteId, byte typeId)
        {
            if (!string.IsNullOrEmpty(strStream))
            {
                var config = Configuration[typeId];
                var base64Data = Regex.Match(strStream, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var imgType = Regex.Match(strStream, @"data:image/(?<type>.+?),(?<data>.+)").Groups["type"].Value;
                var binData = Convert.FromBase64String(base64Data);
                PasteBoardRepository repository = new PasteBoardRepository(SiteSettingsWrapper.Instance.SiteSources[siteId]);
                string path = repository.GetGalleryNewImagePath(pageId);
                if (!path.Contains("/"))
                {
                    PasteBoardSetting setting = repository.GetPasteboard(siteId);
                    string siteImagesPath = Server.MapPath("~/Galleries/pasteboard/" + setting.HomePageUrl);
                    if (!Directory.Exists(siteImagesPath))
                        Directory.CreateDirectory(siteImagesPath);
                    path = "/Galleries/pasteboard/" + setting.HomePageUrl + "/" + path;
                }
                imgType = imgType.Substring(0, imgType.IndexOf(";"));
                string longpath = Server.MapPath("~" + path + "." + imgType);
                var bigName = Server.MapPath("~" + path + "_big." + imgType);

                Stream data = null;
                using (MemoryStream stream = new MemoryStream(binData))
                {
                    ImageResizer imr = new ImageResizer(config.PhotoWidth, config.PhotoHeight);
                    data = imr.Resize(stream, longpath, System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);
                    data.Seek(0, SeekOrigin.Begin);
                    if (typeId == 1)
                    {
                        imr = new ImageResizer(defaultBigWidth, defaultBigHeight);
                        data = imr.Resize(stream, bigName, System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);
                    }
                }
                return new JsonResult()
                {
                    Data = new { url = System.IO.Path.GetFileName(longpath) },
                    ContentType = "text/html",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                return new JsonResult()
                {
                    Data = new { url = "" },
                    ContentType = "text/html",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
        }
    }
}