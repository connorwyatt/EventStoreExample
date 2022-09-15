namespace ConnorWyatt.EventStoreExample.Shared.Events;

public class EventHandler
{
  private readonly IDictionary<string, Func<EventEnvelope<IEvent>, Task>> _handlers =
    new Dictionary<string, Func<EventEnvelope<IEvent>, Task>>();

  protected void When<T>(Func<EventEnvelope<T>, Task> handler) where T : class, IEvent
  {
    _handlers.Add(
      EventUtilities.GetType<T>(),
      async e => await handler(new EventEnvelope<T>((T)e.Event, e.Metadata)));
  }

  protected Task HandleEvent<T>(EventEnvelope<T> eventEnvelope) where T : class, IEvent
  {
    if (!_handlers.TryGetValue(EventUtilities.GetType(eventEnvelope.Event.GetType()), out var handler))
    {
      throw new InvalidOperationException();
    }

    return ((Func<EventEnvelope<T>, Task>)handler).Invoke(eventEnvelope);
  }

  protected bool CanHandleEvent<T>(EventEnvelope<T> eventEnvelope) where T : class, IEvent =>
    _handlers.ContainsKey(EventUtilities.GetType(eventEnvelope.Event.GetType()));
}
