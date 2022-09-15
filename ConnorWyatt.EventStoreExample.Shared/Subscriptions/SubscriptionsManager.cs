using ConnorWyatt.EventStoreExample.Shared.EventStore;
using Microsoft.Extensions.Hosting;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public class SubscriptionsManager : IHostedService
{
  private readonly EventStoreWrapper _eventStoreWrapper;
  private readonly MongoSubscriptionCursorsRepository _subscriptionCursorsRepository;
  private readonly IEnumerable<ISubscriber> _subscribers;

  private IList<SubscriptionManager> _subscriptionManagers = new List<SubscriptionManager>();

  public SubscriptionsManager(
    EventStoreWrapper eventStoreWrapper,
    MongoSubscriptionCursorsRepository subscriptionCursorsRepository,
    IEnumerable<ISubscriber> subscribers)
  {
    _eventStoreWrapper = eventStoreWrapper;
    _subscriptionCursorsRepository = subscriptionCursorsRepository;
    _subscribers = subscribers;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    _subscriptionManagers = _subscribers
      .Select(subscriber => new SubscriptionManager(_eventStoreWrapper, _subscriptionCursorsRepository, subscriber))
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
