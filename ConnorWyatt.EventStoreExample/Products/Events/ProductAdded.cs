using ConnorWyatt.EventStoreExample.Shared.Events;

namespace ConnorWyatt.EventStoreExample.Products.Events;

[Event("eventStoreExample", "productAdded", 1)]
public class ProductAdded : IEvent
{
  public string ProductId { get; }

  public string Name { get; }

  public string Description { get; }

  public ProductAdded(string productId, string name, string description)
  {
    ProductId = productId;
    Name = name;
    Description = description;
  }
}
