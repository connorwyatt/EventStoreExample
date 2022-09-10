using EventStore.Client;
using NodaTime;
using NodaTime.Extensions;

namespace ConnorWyatt.EventStoreExample.Shared.Events;

public static class EventRecordExtensions
{
  public static Instant CreatedInstant(this EventRecord eventRecord) => eventRecord.Created.ToInstant();
}
