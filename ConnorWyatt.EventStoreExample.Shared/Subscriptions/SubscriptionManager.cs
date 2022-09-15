using ConnorWyatt.EventStoreExample.Shared.Events;
using ConnorWyatt.EventStoreExample.Shared.EventStore;
using EventStore.Client;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public class SubscriptionManager
{
  private readonly EventStoreWrapper _eventStoreWrapper;
  private readonly MongoSubscriptionCursorsRepository _subscriptionCursorsRepository;
  private readonly ISubscriber _subscriber;
  private readonly IList<StreamSubscription> _streamSubscriptions = new List<StreamSubscription>();

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
    var subscriptionAttributes = SubscriberUtilities.GetSubscriptionAttributes(_subscriber.GetType());

    var streamNames = subscriptionAttributes
      .Select(subscriptionAttribute => subscriptionAttribute.StreamName)
      .Distinct();

    foreach (var streamName in streamNames)
    {
      var currentStreamPosition = await GetCursor(streamName);

      var streamSubscription = await _eventStoreWrapper.SubscribeToStreamAsync(
        streamName,
        currentStreamPosition.HasValue ? FromStream.After(currentStreamPosition.Value) : FromStream.Start,
        async (_, @event, _) =>
        {
          await _subscriber.HandleEvent(@event);
          await UpdateCursor(streamName, @event);
        },
        true,
        cancellationToken);

      _streamSubscriptions.Add(streamSubscription);
    }
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    foreach (var streamSubscription in _streamSubscriptions)
    {
      streamSubscription.Dispose();
    }

    return Task.CompletedTask;
  }

  private async Task<ulong?> GetCursor(string streamName)
  {
    var subscriberType = _subscriber.GetType();

    var subscriptionCursor = await _subscriptionCursorsRepository.GetSubscriptionCursor(
      SubscriberUtilities.GetSubscriberName(subscriberType),
      streamName);

    return subscriptionCursor?.Position;
  }

  private async Task UpdateCursor(string streamName, EventEnvelope<IEvent> @event)
  {
    var subscriberType = _subscriber.GetType();

    await _subscriptionCursorsRepository.UpsertSubscriptionCursor(
      SubscriberUtilities.GetSubscriberName(subscriberType),
      streamName,
      @event.Metadata.AggregatedStreamPosition);
  }
}
