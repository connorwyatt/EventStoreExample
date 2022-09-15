using ConnorWyatt.EventStoreExample.Shared.Events;
using ConnorWyatt.EventStoreExample.Shared.EventStore;
using EventStore.Client;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public class SubscriptionManager
{
  private readonly EventStoreWrapper _eventStoreWrapper;
  private readonly MongoSubscriptionCursorsRepository _subscriptionCursorsRepository;
  private readonly ISubscriber _subscriber;
  private StreamSubscription? _subscription;

  public SubscriptionManager(
    EventStoreWrapper eventStoreWrapper,
    MongoSubscriptionCursorsRepository subscriptionCursorsRepository,
    ISubscriber subscriber)
  {
    _eventStoreWrapper = eventStoreWrapper;
    _subscriptionCursorsRepository = subscriptionCursorsRepository;
    _subscriber = subscriber;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    var currentStreamPosition = await GetCursor();

    _subscription = await _eventStoreWrapper.SubscribeToStreamAsync(
      SubscriberUtilities.GetStreamName(_subscriber.GetType()),
      currentStreamPosition.HasValue ? FromStream.After(currentStreamPosition.Value) : FromStream.Start,
      async (_, @event, _) =>
      {
        await _subscriber.HandleEvent(@event);
        await UpdateCursor(@event);
      },
      true,
      cancellationToken);
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _subscription?.Dispose();

    return Task.CompletedTask;
  }

  private async Task<ulong?> GetCursor()
  {
    var subscriberType = _subscriber.GetType();

    var subscriptionCursor = await _subscriptionCursorsRepository.GetSubscriptionCursor(
      SubscriberUtilities.GetStreamName(subscriberType),
      SubscriberUtilities.GetSubscriberName(subscriberType));

    return subscriptionCursor?.Position;
  }

  private async Task UpdateCursor(EventEnvelope<IEvent> @event)
  {
    var subscriberType = _subscriber.GetType();

    await _subscriptionCursorsRepository.UpsertSubscriptionCursor(
      SubscriberUtilities.GetStreamName(subscriberType),
      SubscriberUtilities.GetSubscriberName(subscriberType),
      @event.Metadata.AggregatedStreamPosition);
  }
}
