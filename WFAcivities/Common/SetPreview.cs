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
    /// CodeActivity for change preview state any IStatus inheritance
    /// </summary>
    public sealed class SetPreview : CodeActivity
    {
        //Obj for change preview state
        public InArgument<IStatus> QueryItem { get; set; }
        //Preview value 
        public InArgument<bool> PreviewVal { get; set; }

        //Participiant for set Bookmarks
        protected override void CacheMetadata(CodeActivityMetadata metadata)
        {
            metadata.RequireExtension<BookmarksParticipant>();
            base.CacheMetadata(metadata);
        }

        //Change preview state execute
        protected override void Execute(CodeActivityContext context)
        {
            var regParticipant = context.GetExtension<BookmarksParticipant>();

            bool val = PreviewVal.Get(context);
            IStatus queryItem = QueryItem.Get(context);
            queryItem.ChangePreview(val);
        }
    }
}
