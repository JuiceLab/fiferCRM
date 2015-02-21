using EnumHelper;
using Interfaces.CRM;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFActivities.Extension;

namespace WFActivities.Common
{
    /// <summary>
    /// CodeActivity for change status any IStatus inheritance
    /// </summary>
    public sealed class ChangeByteStatus : CodeActivity
    {
        //Obj for change status
        public InArgument<IStatus> QueryItem { get; set; }
        //Predefined enum status
        public InArgument<byte> Status { get; set; }
        //Any other status
        public InArgument<string> CustomStatus { get; set; }

        //Participiant for set Bookmarks
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.RequireExtension<BookmarksParticipant>();
            base.CacheMetadata(metadata);
        }

        //Change status execute
        protected override void Execute(CodeActivityContext context)
        {
            var regParticipant = context.GetExtension<BookmarksParticipant>();

            byte status = Status.Get(context);
            string customStatus = CustomStatus.Get(context);

            IStatus queryItem = QueryItem.Get(context);
            if (queryItem != null && (status != 0 || !string.IsNullOrEmpty(customStatus)))
            {
                regParticipant.IdWF = queryItem.ChangeStatus(status, customStatus).ToString();
            }
        }
    }
}
