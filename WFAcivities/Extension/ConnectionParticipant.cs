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
    /// PersistenceParticipant for save connection string Id
    /// </summary>
    public class ConnectionParticipant : PersistenceParticipant
    {
        public const string DestinationName = "connectionId";

        public readonly XNamespace Xns = XNamespace.Get("EmpranaConnection/");

        public int ConnectionId { get; set; }

        public XName DestinationXName
        {
            get
            {
                return Xns.GetName(DestinationName);
            }
        }

        public ConnectionParticipant(string xns)
        {
            Xns = xns;
        }

        public void Promote(SqlWorkflowInstanceStore instanceStore, string promotionName)
        {
            instanceStore.Promote(promotionName, new List<XName> { DestinationXName }, null);
        }

        protected override void CollectValues(out IDictionary<XName, object> readWriteValues, out IDictionary<XName, object> writeOnlyValues)
        {
            writeOnlyValues = null;
            readWriteValues = new Dictionary<XName, object> { { DestinationXName, this.ConnectionId } };
        }

        protected override void PublishValues(IDictionary<XName, object> readWriteValues)
        {
            object loadedData;
            if (readWriteValues.TryGetValue(DestinationXName, out loadedData))
            {
                ConnectionId = (int)loadedData;
            }
        }
    }
}
