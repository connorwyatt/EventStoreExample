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

  public async Task<SubscriptionCursor?> GetSubscriptionCursor(string subscriberName, string streamName)
  {
    return await _collection.Find(sc => sc.SubscriberName == subscriberName && sc.StreamName == streamName)
      .SingleOrDefaultAsync();
  }

  public async Task UpsertSubscriptionCursor(string subscriberName, string streamName, ulong position)
  {
    await _collection.FindOneAndReplaceAsync<SubscriptionCursor>(
      sc => sc.SubscriberName == subscriberName && sc.StreamName == streamName,
      new SubscriptionCursor(
        GetId(subscriberName, streamName),
        subscriberName,
        streamName,
        position,
        _clock.GetCurrentInstant()),
      new FindOneAndReplaceOptions<SubscriptionCursor>
      {
        IsUpsert = true,
      });
  }

  private string GetId(string subscriberName, string streamName) => $"{subscriberName}:{streamName}";
}
