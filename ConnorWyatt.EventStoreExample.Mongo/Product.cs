using MongoDB.Bson.Serialization.Attributes;
using NodaTime;

namespace ConnorWyatt.EventStoreExample.Mongo;

public record Product(
  [property: BsonId]
  string ProductId,
  string Name,
  string Description,
  Instant AddedAt,
  Instant UpdatedAt,
  ulong Version);
