using ConnorWyatt.EventStoreExample.Products.Events;
using ConnorWyatt.EventStoreExample.Shared.Domain;

namespace ConnorWyatt.EventStoreExample.Products.Domain;

[Category("products")]
public class Product : Aggregate
{
  private bool _added;

  public Product()
  {
    When<ProductAdded>(Apply);
    When<ProductUpdated>(Apply);
  }

  public void AddProduct(string name, string description)
  {
    if (_added)
    {
      return;
    }

    RaiseEvent(new ProductAdded(Id, name, description));
  }

  public void UpdateProduct(string name, string description)
  {
    if (!_added)
    {
      return;
    }

    RaiseEvent(new ProductUpdated(Id, name, description));
  }

  private void Apply(ProductAdded @event)
  {
    _added = true;
  }

  private void Apply(ProductUpdated @event)
  {
  }
}
