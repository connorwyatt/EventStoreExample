using ConnorWyatt.EventStoreExample.Shared.MongoDB;
using MongoDB.Driver;
using NodaTime;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public class MongoSubscriptionCursorsRepository
{
  private const string CollectionName = "__subscription-cursors__";

  private readonly IMongoCollection<SubscriptionCursor> _collection;
  private readonly IClock _clock;

  public MongoSubscriptionCursorsRepository(IMongoClient mongoClient, IClock clock, MongoDBOptions mongoDBOptions)
  {
    var database = mongoClient.GetDatabase(mongoDBOptions.DatabaseName);
    _collection = database.GetCollection<SubscriptionCursor>(CollectionName);
    _clock = clock;
  }

  public async Task<SubscriptionCursor?> GetSubscriptionCursor(string streamName, string subscriberName)
  {
    return await _collection.Find(sc => sc.StreamName == streamName && sc.SubscriberName == subscriberName)
      .SingleOrDefaultAsync();
  }

  public async Task UpsertSubscriptionCursor(string streamName, string subscriberName, ulong position)
  {
    await _collection.FindOneAndReplaceAsync<SubscriptionCursor>(
      sc => sc.StreamName == streamName && sc.SubscriberName == subscriberName,
      new SubscriptionCursor(
        GetId(streamName, subscriberName),
        streamName,
        subscriberName,
        position,
        _clock.GetCurrentInstant()),
      new FindOneAndReplaceOptions<SubscriptionCursor>
      {
        IsUpsert = true,
      });
  }

  private string GetId(string streamName, string subscriberName) => $"{streamName}:{subscriberName}";
}
