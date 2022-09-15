namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

[AttributeUsage(AttributeTargets.Class)]
public class SubscriptionAttribute : Attribute
{
  public string StreamName { get; }

  public string SubscriberName { get; }

  public SubscriptionAttribute(string streamName, string subscriberName)
  {
    StreamName = streamName;
    SubscriberName = subscriberName;
  }
}
