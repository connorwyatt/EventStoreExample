using System.Reflection;

namespace ConnorWyatt.EventStoreExample.Shared.Events;

public static class EventUtilities
{
  public static string GetType<T>() where T : class => GetType(typeof(T));

  public static string GetType(Type type)
  {
    var eventAttribute = type.GetCustomAttribute<EventAttribute>() ??
      throw new InvalidOperationException($"Missing \"EventAttribute\" on {type.Name}.");

    return ConstructType(eventAttribute.Namespace, eventAttribute.EventType, eventAttribute.Version);
  }

  public static string ConstructType(string @namespace, string eventType, int version) =>
    string.Join(".", @namespace, eventType, $"v{version}");
}
