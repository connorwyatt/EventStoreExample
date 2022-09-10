namespace ConnorWyatt.EventStoreExample.Shared.Events;

[AttributeUsage(AttributeTargets.Class)]
public class EventAttribute : Attribute
{
  public string Namespace { get; }

  public string EventType { get; }

  public int Version { get; }

  public EventAttribute(string @namespace, string eventType, int version)
  {
    Namespace = @namespace;
    EventType = eventType;
    Version = version;
  }
}
