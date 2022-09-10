using ConnorWyatt.EventStoreExample.Shared.Events;
using EventStore.Client;

namespace ConnorWyatt.EventStoreExample.Shared.EventStore;

public class EventStoreWrapper
{
  private readonly EventStoreClient _client;
  private readonly EventSerializer _eventSerializer;

  public EventStoreWrapper(EventStoreClient client, EventSerializer eventSerializer)
  {
    _client = client;
    _eventSerializer = eventSerializer;
  }

  public async Task<IAsyncEnumerable<EventEnvelope<IEvent>>?> ReadStreamAsync(
    Direction direction,
    string streamName,
    StreamPosition revision,
    bool resolveLinkTos = false,
    TimeSpan? deadline = null,
    UserCredentials? userCredentials = null,
    CancellationToken cancellationToken = default)
  {
    var result = _client.ReadStreamAsync(
      direction,
      streamName,
      revision,
      resolveLinkTos: resolveLinkTos,
      cancellationToken: cancellationToken);

    if (await result.ReadState == ReadState.StreamNotFound)
    {
      return null;
    }

    return result.Select(
      resolvedEvent => new EventEnvelope<IEvent>(
        _eventSerializer.Deserialize(resolvedEvent.Event),
        EventMetadata.FromResolvedEvent(resolvedEvent)));
  }

  public async Task AppendToStreamAsync(
    string streamName,
    StreamRevision expectedRevision,
    IEnumerable<IEvent> events,
    CancellationToken cancellationToken = default)
  {
    await _client.AppendToStreamAsync(
      streamName,
      expectedRevision,
      events.Select(
        e => new EventData(Uuid.NewUuid(), EventUtilities.GetType(e.GetType()), _eventSerializer.Serialize(e))),
      cancellationToken: cancellationToken);
  }

  public async Task<StreamSubscription> SubscribeToStreamAsync(
    string streamName,
    FromStream start,
    Func<StreamSubscription, EventEnvelope<IEvent>, CancellationToken, Task> eventAppeared,
    bool resolveLinkTos = false,
    CancellationToken cancellationToken = default)
  {
    return await _client.SubscribeToStreamAsync(
      streamName,
      start,
      (ss, re, ct) =>
      {
        var @event = new EventEnvelope<IEvent>(
          _eventSerializer.Deserialize(re.Event),
          EventMetadata.FromResolvedEvent(re));

        return eventAppeared.Invoke(ss, @event, ct);
      },
      resolveLinkTos,
      cancellationToken: cancellationToken);
  }
}
