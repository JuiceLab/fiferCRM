using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using UserContext;
using EntityRepository;
using Settings;
using Microsoft.AspNet.SignalR.Hubs;

namespace SignalrService.Hub
{
    [HubName("adminHub")]
    public class AdminHub : Microsoft.AspNet.SignalR.Hub
    {
        public static Dictionary<Guid, string> connectionUserIds;
        public static List<User> ConnectedUsers { get; set; }

        public AdminHub()
        {
            if (ConnectedUsers == null)
                ConnectedUsers = new List<User>();
            if (connectionUserIds == null)
                connectionUserIds = new Dictionary<Guid, string>();
        }
        public void Connect(Guid userId)
        {
            using (UserEntities context = new UserEntities(AccessSettings.LoadSettings().UserEntites))
            {
                var user = context.GetUnitById<User>(userId);
                if (!ConnectedUsers.Any(x => x.UserId == userId))
                {
                    ConnectedUsers.Add(new  User() { UserId = userId, Login = user.Login });
                    connectionUserIds.Add(userId, Context.ConnectionId);
                }
                else
                    connectionUserIds[userId] = Context.ConnectionId;
                Clients.Client(connectionUserIds[userId]).SendMessageNotify(user.Login, "connect", "", "success");
            }
        }

        public void TicketUpdated(IHubContext hub, Guid userId, string msg)
        {
            hub.Clients.All.ticketTableRefresh(userId);
            try
            {
                hub.Clients.Client(connectionUserIds[userId]).ticketCreated(msg);
            }
            catch
            { }


        }

        public void TaskUpdated(IHubContext hub, Guid userId, string msg)
        {
            hub.Clients.All.taskTableRefresh(userId);
            try
            {
                hub.Clients.Client(connectionUserIds[userId]).taskCreated(msg);
            }
            catch
            { }
        }


        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

    }
}
