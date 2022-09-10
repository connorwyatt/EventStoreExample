using NodaTime;

namespace ConnorWyatt.EventStoreExample.Products.Models;

public record Product(string ProductId, string Name, string Description, Instant AddedAt, Instant UpdatedAt);
