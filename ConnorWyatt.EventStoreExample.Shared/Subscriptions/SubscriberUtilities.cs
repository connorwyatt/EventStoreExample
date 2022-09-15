using System.Reflection;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public static class SubscriberUtilities
{
  public static string GetStreamName<T>() where T : class => GetStreamName(typeof(T));

  public static string GetStreamName(Type type) => GetSubscriberAttribute(type).StreamName;

  public static string GetSubscriberName<T>() where T : class => GetSubscriberName(typeof(T));

  public static string GetSubscriberName(Type type) => GetSubscriberAttribute(type).SubscriberName;

  private static SubscriptionAttribute GetSubscriberAttribute(Type type) =>
    type.GetCustomAttribute<SubscriptionAttribute>() ??
    throw new InvalidOperationException($"Missing \"SubscriptionAttribute\" on {type.Name}.");
}
