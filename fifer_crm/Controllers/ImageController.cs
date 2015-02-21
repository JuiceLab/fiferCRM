using ImageHelper;
using LogService.FilterAttibute;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace fifer_crm.Controllers
{
    [Authorize, CRMLogAttribute]
    public class ImageController : Controller
    {
        private int defaultBigWidth = 340;
        private int defaultBigHeight = 280;

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
            new PhotoSaveConfig("company","/private/company/",  100, 100),
            new PhotoSaveConfig("employee","/private/person/",  150, 200),
            new PhotoSaveConfig("passport","/private/passport/", 300, 200),
            new PhotoSaveConfig("customer","/private/customer/",  150, 200),

        };
        // GET: Image
        [DisplayName("Загрузка изображения")]
        public ActionResult SaveCroped(string filename, string strStream, int companyId, string type)
        {
            PhotoSaveConfig config = Configuration.FirstOrDefault(m => m.ConfigName == type);
            if (string.IsNullOrEmpty(filename))
            {
                Random rnd = new Random();
                filename = config.ConfigName + "_" + Guid.NewGuid().ToString().Replace("-",string.Empty) + ".jpg";
            }
            if (!string.IsNullOrEmpty(strStream))
            {
                var base64Data = Regex.Match(strStream, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
                var binData = Convert.FromBase64String(base64Data);
                var bigName = string.Format("{0}-big{1}", Path.GetFileNameWithoutExtension(filename), Path.GetExtension(filename));
                string longpath = ImageResizer.GenerateFileName(bigName, Server.MapPath("~" + config.Directory));
                Stream data = null;
                using (MemoryStream stream = new MemoryStream(binData))
                {
                    var imgId = Guid.NewGuid();
                    longpath = longpath.Replace("_big", "-big");
                    ImageResizer imr = new ImageResizer(defaultBigWidth, defaultBigHeight);
                    data = imr.Resize(stream, longpath, System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);
                    data.Seek(0, SeekOrigin.Begin);
                    longpath = longpath.Replace("-big", string.Empty);
                    imgId = Guid.NewGuid();
                    imr = new ImageResizer(config.PhotoWidth, config.PhotoHeight);
                    data = imr.Resize(stream, longpath, System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic);
                    data.Seek(0, SeekOrigin.Begin);
                }
                if (type == "passport")
                {
                    return PartialView("ScanImg", config.Directory + System.IO.Path.GetFileName(longpath));
                }

                return new JsonResult()
                {
                    Data = new { url = config.Directory + System.IO.Path.GetFileName(longpath) },
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