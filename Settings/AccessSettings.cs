using ExtensionHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace Settings
{
    /// <summary>
    /// This class save settings of dbcontext connection strings and transport uri
    /// </summary>
    [XmlRootAttribute("AccessSettings", Namespace = "", IsNullable = false)]
    public class AccessSettings
    {
#if DEBUG
        private const string XML_FILE = "access_settings_debug.xml";
#else
        private const string XML_FILE = "access_settings.xml";
#endif
        private string _companyEntities = "";
        private string _companyViewsEntites = "";
        private string _crmEntities = "";
        private string _templateEntities = "";
        
        private string _localCrmEntities = "";
        private string _localLandingEntities = "";
        private string _localPasteboardEntities = "";

        private string _ticketEntites = "";
        private string _userEntites = "";
        private string _logEntities = "";
        private string _instanceStore = "";

        public AccessSettings()
        {
        }

        public AccessSettings(string companyEntites, string crmEntities, string localCrmEntities, string viewsEntites, string ticketEntites, string accessEntites, string logEntites, string instanceStore)
        {
            _crmEntities = crmEntities;
            _localCrmEntities = localCrmEntities;
            _companyEntities = companyEntites;
            _companyViewsEntites = viewsEntites;
            _ticketEntites = ticketEntites;
            _userEntites = accessEntites;
            _logEntities = logEntites;
            _instanceStore = instanceStore;
        }

        /// <summary>
        /// Saves file settings changes from this property values
        /// </summary>
        public void Save()
        {
            string root = System.AppDomain.CurrentDomain.BaseDirectory;
            ObjectXMLSerializer<AccessSettings>.Save(this, root + XML_FILE);
            HttpRuntime.Cache.Remove("AccessSettings");
        }


        /// <summary>
        /// Loads Property values from access_settings.xml in root site
        /// </summary>
        /// <returns></returns>
        private static AccessSettings Load()
        {
            string root = System.AppDomain.CurrentDomain.BaseDirectory;
            return ObjectXMLSerializer<AccessSettings>.Load(root + XML_FILE);
        }

        /// <summary>
        /// Loads the settings from cache or access_settings.xml.
        /// </summary>
        /// <returns></returns>
        public static AccessSettings ReloadSettings()
        {
            return HttpRuntime.Cache.Update<AccessSettings>("AccessSettings", ShopSettingsLock, () => Load(), DateTime.Now.AddDays(1));
        }

        /// <summary>
        /// Loads the settings from cache or access_settings.xml.
        /// </summary>
        /// <returns></returns>
        public static AccessSettings LoadSettings()
        {
            return HttpRuntime.Cache.Get<AccessSettings>("AccessSettings", ShopSettingsLock, () => Load(), DateTime.Now.AddDays(1));
        }

        public string CrmEntites { get { return _crmEntities; } set { _crmEntities = value; } }
        public string CrmEntitesDemo
        {
            get { 
                return _crmEntities; 
            } 

            set { 
                if(!string.IsNullOrEmpty(value))
                    _crmEntities = value; 
            }
        }
        public string CompanyEntites { get { return _companyEntities; } set { _companyEntities = value; } }
        public string TemplateEntities { get { return _templateEntities; } set { _templateEntities = value; } }
        public string LocalCrmEntites { get { return _localCrmEntities; } set { _localCrmEntities = value; } }
        public string LocalLandingEntities { get { return _localLandingEntities; } set { _localLandingEntities = value; } }
        public string LocalPasteboardEntities { get { return _localPasteboardEntities; } set { _localPasteboardEntities = value; } }
        public string CompanyViewsEntites { get { return _companyViewsEntites; } set { _companyViewsEntites = value; } }
        public string InstanceStore { get { return _instanceStore; } set { _instanceStore = value; } }
        public string LogEntites { get { return _logEntities; } set { _logEntities = value; } }
        public string TicketEntites { get { return _ticketEntites; } set { _ticketEntites = value; } }
        public string UserEntites { get { return _userEntites; } set { _userEntites = value; } }
      
        public static object ShopSettingsLock = new object();
    }
}
