using ConnorWyatt.EventStoreExample.Shared.Events;
using EventHandler = ConnorWyatt.EventStoreExample.Shared.Events.EventHandler;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public abstract class SubscriberBase : EventHandler, ISubscriber
{
  public async Task HandleEvent(EventEnvelope<IEvent> eventEnvelope)
  {
    if (!CanHandleEvent(eventEnvelope))
    {
      return;
    }

    await base.HandleEvent(eventEnvelope);
  }
}
