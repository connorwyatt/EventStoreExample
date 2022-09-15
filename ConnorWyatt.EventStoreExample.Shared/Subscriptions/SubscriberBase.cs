using ConnorWyatt.EventStoreExample.Shared.Events;
using EventHandler = ConnorWyatt.EventStoreExample.Shared.Events.EventHandler;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public abstract class SubscriberBase : EventHandler, ISubscriber
{
  private ulong? _cursor;

  public async Task HandleEvent(EventEnvelope<IEvent> eventEnvelope)
  {
    if (!CanHandleEvent(eventEnvelope))
    {
      return;
    }

    await base.HandleEvent(eventEnvelope);
  }

  public Task<ulong?> GetCursor() => Task.FromResult(_cursor);

  public Task UpdateCursor(EventEnvelope<IEvent> eventEnvelope)
  {
    _cursor = eventEnvelope.Metadata.AggregatedStreamPosition;
    return Task.CompletedTask;
  }
}
