using EventStore.Client;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public class SubscriptionManager
{
  private readonly EventStore.EventStoreWrapper _eventStoreWrapper;
  private readonly ISubscriber _subscriber;
  private StreamSubscription? _subscription;

  public SubscriptionManager(EventStore.EventStoreWrapper eventStoreWrapper, ISubscriber subscriber)
  {
    _eventStoreWrapper = eventStoreWrapper;
    _subscriber = subscriber;
  }

  public async Task StartAsync(CancellationToken cancellationToken)
  {
    var currentStreamPosition = await _subscriber.GetCursor();

    _subscription = await _eventStoreWrapper.SubscribeToStreamAsync(
      SubscriberUtilities.GetStreamName(_subscriber.GetType()),
      currentStreamPosition.HasValue ? FromStream.After(currentStreamPosition.Value) : FromStream.Start,
      async (_, @event, _) =>
      {
        await _subscriber.HandleEvent(@event);
        await _subscriber.UpdateCursor(@event);
      },
      resolveLinkTos: true,
      cancellationToken: cancellationToken);
  }

  public Task StopAsync(CancellationToken cancellationToken)
  {
    _subscription?.Dispose();

    return Task.CompletedTask;
  }
}
