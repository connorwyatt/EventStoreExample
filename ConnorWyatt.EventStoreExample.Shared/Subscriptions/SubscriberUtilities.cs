using System.Reflection;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public static class SubscriberUtilities
{
  public static string GetStreamName<T>() where T : class => GetStreamName(typeof(T));

  public static string GetStreamName(Type type)
  {
    var subscriptionAttribute = type.GetCustomAttribute<SubscriptionAttribute>() ??
      throw new InvalidOperationException($"Missing \"SubscriptionAttribute\" on {type.Name}.");

    return subscriptionAttribute.StreamName;
  }
}
