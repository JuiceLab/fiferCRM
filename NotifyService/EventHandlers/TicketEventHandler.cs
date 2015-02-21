using LogContext;
using NotifyModel;
using PatternTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyService.EventHandlers
{
    public class TicketEventsObserver
    {
        private TicketEventsObserver()
        {
        }

        public static TicketEventHandler Instance
        {
            get
            {
                return Singleton<TicketEventHandler>.Instance;
            }
        }
    }

    public class TicketEventHandler : IObservable<NotifyObserveItem>
    {
        public List<IObserver<NotifyObserveItem>> Observers { get; set; }

        private TicketEventHandler()
        {
            List<IObserver<NotifyObserveItem>> observers = new List<IObserver<NotifyObserveItem>>();
        }

        public IDisposable Subscribe(IObserver<NotifyObserveItem> observer)
        {
            if (Observers!=null && !Observers.Contains(observer))
            {
                Observers.Add(observer);
            }
            return new Unsubscriber<NotifyObserveItem>(Observers, observer);
        }

        public void EventFired(NotifyObserveItem item)
        {
            if (Observers != null)
            {
                foreach (var observer in Observers)
                    observer.OnNext(item);
            }
        }
    }
}
