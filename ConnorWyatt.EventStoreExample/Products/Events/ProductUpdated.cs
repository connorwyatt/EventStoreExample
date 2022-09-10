using ConnorWyatt.EventStoreExample.Shared.Events;

namespace ConnorWyatt.EventStoreExample.Products.Events;

[Event("eventStoreExample", "productUpdated", 1)]
public class ProductUpdated : IEvent
{
  public string ProductId { get; }

  public string Name { get; }

  public string Description { get; }

  public ProductUpdated(string productId, string name, string description)
  {
    ProductId = productId;
    Name = name;
    Description = description;
  }
}
