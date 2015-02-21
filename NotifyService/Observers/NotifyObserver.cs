using LogContext;
using NotifyEventModel;
using NotifyModel;
using NotifyService.Notifiers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotifyService.Observers
{
    public class NotifyObserver : IObserver<NotifyObserveItem>
    {
        private IDisposable cancellation;

        public NotifyObserver()
        {
        }

        public virtual void Subscribe(IObservable<NotifyObserveItem> provider)
        {
            cancellation = provider.Subscribe(this);
        }

        public virtual void OnNext(NotifyObserveItem value)
        {
            DoNotify(value);
        }

        public virtual void DoNotify(NotifyObserveItem value)
        {
            try
            {
                List<int> notifyItems = new List<int>();
                if (value.FromUserId.HasValue && NotifySingletonWrapper.Instance.Users.ContainsKey(value.FromUserId.Value))
                {
                    var listenerId = NotifySingletonWrapper.Instance.Users[value.FromUserId.Value];
                    var item = NotifySingletonWrapper.Instance.UserSettings
                        .FirstOrDefault(m => m.ListenerId == listenerId)
                        .NotifyProfile.FirstOrDefault(m => m.IsActive && m.NotifyId == value.NotifyId && m.FromMe);
                    if (item != null)
                        notifyItems.Add(listenerId);
                }

                if (value.ToUserId.HasValue && NotifySingletonWrapper.Instance.Users.ContainsKey(value.ToUserId.Value))
                {
                    var listenerId = NotifySingletonWrapper.Instance.Users[value.ToUserId.Value];
                    var item = NotifySingletonWrapper.Instance.UserSettings
                        .FirstOrDefault(m => m.ListenerId == listenerId)
                        .NotifyProfile.FirstOrDefault(m => m.IsActive && m.NotifyId == value.NotifyId && m.ToMe);
                    if (item != null)
                        notifyItems.Add(listenerId);
                }

                if (value.OwnerId.HasValue && NotifySingletonWrapper.Instance.Users.ContainsKey(value.OwnerId.Value))
                {
                    var listenerId = NotifySingletonWrapper.Instance.Users[value.OwnerId.Value];
                    var item = NotifySingletonWrapper.Instance.UserSettings
                        .FirstOrDefault(m => m.ListenerId == listenerId)
                        .NotifyProfile.FirstOrDefault(m => m.IsActive && m.NotifyId == value.NotifyId && m.UnderMe);
                    if (item != null)
                        notifyItems.Add(listenerId);
                }
                var notifier = NotifyFabric();

                notifier.DoNotify(value, notifyItems);
            }
            catch (Exception ex)
            {
            }

        }

        protected IEventNotifier NotifyFabric()
        {
            IEventNotifier notifier = new SignalrNotifier();
            return notifier;
        }

        public virtual void OnCompleted()
        {
        }

        public virtual void OnError(Exception error)
        {
        }
    }
}
