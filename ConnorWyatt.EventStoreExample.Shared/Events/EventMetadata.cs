using EventStore.Client;
using NodaTime;

namespace ConnorWyatt.EventStoreExample.Shared.Events;

public record EventMetadata(Instant Timestamp, ulong StreamPosition, ulong AggregatedStreamPosition)
{
  public static EventMetadata FromResolvedEvent(ResolvedEvent resolvedEvent)
  {
    return new EventMetadata(resolvedEvent.Event.CreatedInstant(), resolvedEvent.Event.EventNumber, resolvedEvent.OriginalEventNumber);
  }
}
