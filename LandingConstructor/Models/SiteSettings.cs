using CompanyContext;
using PatternTemplate;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteConstructor.Models
{
    public class SiteSettingsWrapper
    {
        private SiteSettingsWrapper()
        {
        }

        public static SiteSettings Instance
        {
            get
            {
                return Singleton<SiteSettings>.Instance;
            }
        }

        public static SiteSettings ResetValues
        {
            get
            {
                return Singleton<SiteSettings>.ReloadInstance;
            }
        }
    }
   
    public class SiteSettings
    {
        public Dictionary<Guid, string> SiteSources { get; set; }
        public Dictionary<string, Guid> SiteIds { get; set; }

        public Dictionary<Guid, byte> SiteTypes { get; set; }
        public Dictionary<Guid, string> SiteTemplates { get; set; }

        private SiteSettings()
        {
            using (CompanyEntities context = new CompanyEntities(AccessSettings.LoadSettings().CompanyEntites))
            {
                SiteIds = context.WebSites
                  .ToDictionary(m => m.Url, m => m.SiteGuid);
                SiteSources = context.WebSites
                    .ToDictionary(m => m.SiteGuid, m => m.DbSource);
                SiteTypes = context.WebSites
                  .ToDictionary(m => m.SiteGuid, m => m.Type);
                SiteTemplates = context.WebSites
                 .ToDictionary(m => m.SiteGuid, m => m.TemplateName);
            }
        }
    }
}
