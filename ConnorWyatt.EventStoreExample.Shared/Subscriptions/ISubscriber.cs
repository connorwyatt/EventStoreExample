using ConnorWyatt.EventStoreExample.Shared.Events;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public interface ISubscriber
{
  public Task HandleEvent(EventEnvelope<IEvent> eventEnvelope);

  public Task<ulong?> GetCursor();

  public Task UpdateCursor(EventEnvelope<IEvent> eventEnvelope);
}
