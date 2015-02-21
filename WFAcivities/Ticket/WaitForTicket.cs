using EnumHelper;
using Interfaces.CRM;
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
    /// CodeActivity for wait any WFCommand enum and resume workflow with change ticket's state
    /// </summary>
    public sealed class WaitForTicket : NativeActivity<object>
    {
        private IStatus queryItem;
        private WFStatus callbackStatus;
        // Define an activity input argument of type string
        public InArgument<IStatus> QueryItem { get; set; }
        public InArgument<WFCommand> Command { get; set; }
        public InArgument<WFStatus> CallbackStatus { get; set; }

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
            regParticipant.Bookmarks.Add(command.ToString());

            queryItem = QueryItem.Get(context);
            callbackStatus = CallbackStatus.Get(context);
            context.CreateBookmark(this.Command.Get(context).ToString(), new BookmarkCallback(HandleEvent));
        }

        // after resuming set callbackStatus for value
        private void HandleEvent(NativeActivityContext context, Bookmark bookmark, object value)
        {
            context.SetValue(this.Result, value);
            queryItem.ChangeStatus((int)callbackStatus);
            context.GetExtension<BookmarksParticipant>().Bookmarks = new List<string>();

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
