using LogContext;
using NotifyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyService.Notifiers
{
    public interface IEventNotifier
    {
        Guid UserId { get; set; }

        void DoNotify(NotifyObserveItem notify, IEnumerable<int> user);

        void LogingNotify(NotifyObserveItem notify, int status);
    }
}
