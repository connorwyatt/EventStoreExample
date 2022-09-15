using ConnorWyatt.EventStoreExample.Shared.EventStore;
using EventStore.Client;

namespace ConnorWyatt.EventStoreExample.Shared.Domain;

public class AggregateRepository
{
  private readonly EventStoreWrapper _eventStoreWrapper;

  public AggregateRepository(EventStoreWrapper eventStoreWrapper) => _eventStoreWrapper = eventStoreWrapper;

  public async Task<T> LoadAggregate<T>(string id) where T : Aggregate, new()
  {
    var streamName = AggregateUtilities.GetStreamName<T>(id);

    var stream = await _eventStoreWrapper.ReadStreamAsync(
      Direction.Forwards,
      streamName,
      StreamPosition.Start);

    var aggregate = new T
    {
      Id = id,
    };

    if (stream == null)
    {
      return aggregate;
    }

    await foreach (var @event in stream)
    {
      aggregate.ReplayEvent(@event);
    }

    return aggregate;
  }

  public async Task SaveAggregate<T>(T aggregate) where T : Aggregate
  {
    var unsavedEvents = aggregate.GetUnsavedEvents();
    var hasUnsavedEvents = unsavedEvents.Any();

    if (!hasUnsavedEvents)
    {
      return;
    }

    await _eventStoreWrapper.AppendToStreamAsync(
      AggregateUtilities.GetStreamName<T>(aggregate.Id),
      aggregate.LatestSavedEventVersion() ?? StreamRevision.None,
      unsavedEvents);
  }
}
