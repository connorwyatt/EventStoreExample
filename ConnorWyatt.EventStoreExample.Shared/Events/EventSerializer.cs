using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;
using ConnorWyatt.EventStoreExample.Shared.Serialization;
using EventStore.Client;

namespace ConnorWyatt.EventStoreExample.Shared.Events;

public class EventSerializer
{
  private readonly ImmutableDictionary<string, Type> _eventTypeLookup;

  public EventSerializer(Assembly assembly)
  {
    _eventTypeLookup = GetEventTypeLookup(assembly);
  }

  public byte[] Serialize(IEvent @event)
  {
    var eventType = EventUtilities.GetType(@event.GetType());

    if (!_eventTypeLookup.ContainsKey(eventType))
    {
      throw new InvalidOperationException($"Could not find Event class for {eventType}.");
    }

    return JsonSerializer.SerializeToUtf8Bytes(@event, @event.GetType(), DefaultJsonSerializerOptions.Options);
  }

  public IEvent Deserialize(EventRecord eventRecord)
  {
    if (!_eventTypeLookup.TryGetValue(eventRecord.EventType, out var eventType))
    {
      throw new InvalidOperationException($"Could not find Event class for {eventRecord.EventType}.");
    }

    var deserializedEvent = JsonSerializer.Deserialize(
      eventRecord.Data.Span,
      eventType,
      DefaultJsonSerializerOptions.Options);

    if (deserializedEvent == null)
    {
      throw new InvalidOperationException($"Could not deserialize event data to {eventType.Name}.");
    }

    return (IEvent)deserializedEvent;
  }

  private static ImmutableDictionary<string, Type> GetEventTypeLookup(Assembly assembly)
  {
    var eventSubclasses = assembly.DefinedTypes.Where(type => type.ImplementedInterfaces.Contains(typeof(IEvent)));

    return eventSubclasses.ToImmutableDictionary(EventUtilities.GetType, type => type.AsType());
  }
}
