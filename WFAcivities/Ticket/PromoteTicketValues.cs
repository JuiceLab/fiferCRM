using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WFActivities.Extension;

namespace WFActivities.Ticket
{
    /// <summary>
    /// CodeActivity for save Workflow identifier
    /// </summary>
    public sealed class PromoteTicketValues : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> IdName { get; set; }

        //Participiant for current bookmarks
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.RequireExtension<BookmarksParticipant>();
            base.CacheMetadata(metadata);
        }

        // set workflow identifier to bookmark participiant
        protected override void Execute(CodeActivityContext context)
        {
            var regParticipant = context.GetExtension<BookmarksParticipant>();
            regParticipant.IdWF = this.IdName.Get(context);
        }
    }
}
