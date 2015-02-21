using System;
using System.Activities.DurableInstancing;
using System.Activities.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WFActivities.Extension
{
    /// <summary>
    /// PersistenceParticipant for save and resume workflow with WFCommand bookmarks
    /// </summary>
    public class BookmarksParticipant : PersistenceParticipant
    {
        public const string IdWFName = "IdWF";
        public const string BookmarksName = "BookmarksName";

        public readonly XNamespace Xns = XNamespace.Get("FiferCRM/");

        public string IdWF { get; set; }
        public List<string> Bookmarks { get; set; }

        public XName IdWFXName
        {
            get
            {
                return Xns.GetName(IdWFName);
            }
        }

        public XName BookmarksXName
        {
            get
            {
                return Xns.GetName(BookmarksName);
            }
        }

        public BookmarksParticipant(string xns)
        {
            Xns = xns;
            Bookmarks = new List<string>();
        }

        public void Promote(SqlWorkflowInstanceStore instanceStore, string promotionName)
        {
            instanceStore.Promote(promotionName, new List<XName> { IdWFXName }, new List<XName> { BookmarksXName });
        }

        protected override void CollectValues(out IDictionary<XName, object> readWriteValues, out IDictionary<XName, object> writeOnlyValues)
        {
            writeOnlyValues = null;
            readWriteValues = new Dictionary<XName, object> { { IdWFXName, this.IdWF }, { BookmarksXName, this.Bookmarks } };
        }

        protected override void PublishValues(IDictionary<XName, object> readWriteValues)
        {
            object loadedData;
            if (readWriteValues.TryGetValue(IdWFXName, out loadedData))
            {
                IdWF = (string)loadedData;
            }

            if (readWriteValues.TryGetValue(BookmarksXName, out loadedData))
            {
                Bookmarks = (List<string>)loadedData;
            }
        }
    }
}
