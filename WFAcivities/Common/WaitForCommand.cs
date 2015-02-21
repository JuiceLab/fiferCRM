using EnumHelper;
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
    /// CodeActivity for wait any WFCommand enum and resume workflow
    /// </summary>
    public sealed class WaitForCommand : NativeActivity<object>
    {
        //Expected status for rewumeworkflow
        public InArgument<WFCommand> Command { get; set; }
        //TicketId(obsolete) 
        public InArgument<Guid> TicketId { get; set; }

        //Participiant for set Bookmarks
        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            metadata.RequireExtension<BookmarksParticipant>();
            base.CacheMetadata(metadata);
        }

        /// <summary>
        /// When implemented in a derived class, runs the activity’s execution logic.
        /// </summary>
        /// <param name="context">The execution context in which the activity executes.</param>
        protected override void Execute(NativeActivityContext context)
        {
            WFCommand command = Command.Get(context);
            var regParticipant = context.GetExtension<BookmarksParticipant>();
            regParticipant.Bookmarks.Add(command.ToString("d"));

            context.CreateBookmark(command.ToString("d"), new BookmarkCallback(HandleEvent));
        }

        // Resume workflow with value
        private void HandleEvent(NativeActivityContext context, Bookmark bookmark, object value)
        {
            context.GetExtension<BookmarksParticipant>().Bookmarks = new List<string>();
            context.SetValue(this.Result, value);

        }

        /// <summary>
        ///   Gets a value indicating whether CanInduceIdle.
        /// </summary>
        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

    }
}
