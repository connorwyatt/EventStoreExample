using ConnorWyatt.EventStoreExample.Shared.EventStore;
using Microsoft.Extensions.Hosting;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public class SubscriptionsManager : IHostedService
{
  private readonly EventStoreWrapper _eventStoreWrapper;
  private readonly IEnumerable<ISubscriber> _subscribers;

  private IList<SubscriptionManager> _subscriptionManagers = new List<SubscriptionManager>();

  public SubscriptionsManager(EventStoreWrapper eventStoreWrapper, IEnumerable<ISubscriber> subscribers)
  {
    _eventStoreWrapper = eventStoreWrapper;
    _subscribers = subscribers;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _subscriptionManagers = _subscribers
      .Select(subscriber => new SubscriptionManager(_eventStoreWrapper, subscriber))
      .ToList();

    await Task.WhenAll(
      _subscriptionManagers.Select(subscriptionManager => subscriptionManager.StartAsync(cancellationToken)));
  }

  public async Task StopAsync(CancellationToken cancellationToken)
  {
    await Task.WhenAll(
      _subscriptionManagers.Select(subscriptionManager => subscriptionManager.StopAsync(cancellationToken)));
  }
}
