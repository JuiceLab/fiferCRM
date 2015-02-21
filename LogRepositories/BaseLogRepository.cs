using LogContext;
using Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskModel;

namespace LogRepositories
{
    public class BaseLogRepository : IDisposable
    {
        public LogEntities Context { get; set; }

        public BaseLogRepository()
        {
            Context = new LogEntities(AccessSettings.LoadSettings().LogEntites);
        }

        public IEnumerable<MessageViewModel> GetUserLogActions(IEnumerable<Guid> users, DateTime date)
        {

            return Context.CRMActions.Where(m => users.Contains(m.UserId) && m.TimeExecute >= date)
                .Select(m => new MessageViewModel()
                {
                    Created = m.TimeExecute,
                    Msg = m.ActionDescription,
                    UserId = m.UserId
                }).ToList();
        }

        public void Dispose()
        {
            Context.Database.Connection.Close();
            Context.Dispose();
        }
    }
}
