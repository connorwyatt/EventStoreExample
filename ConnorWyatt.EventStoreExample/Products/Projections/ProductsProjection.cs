using ConnorWyatt.EventStoreExample.Mongo;
using ConnorWyatt.EventStoreExample.Products.Events;
using ConnorWyatt.EventStoreExample.Shared.Events;
using ConnorWyatt.EventStoreExample.Shared.Subscriptions;

namespace ConnorWyatt.EventStoreExample.Products.Projections;

[SubscriberName("ProductsProjection")]
[Subscription("$ce-products")]
public class ProductsProjection : SubscriberBase
{
  private readonly MongoProductsRepository _productsRepository;

  public ProductsProjection(MongoProductsRepository productsRepository)
  {
    _productsRepository = productsRepository;

    When<ProductAdded>(AddProduct);
    When<ProductUpdated>(UpdateProduct);
  }

  private async Task AddProduct(ProductAdded @event, EventMetadata metadata)
  {
    var product = await _productsRepository.GetProduct(@event.ProductId);

    if (product != null)
    {
      return;
    }

    await _productsRepository.InsertProduct(
      new Product(
        @event.ProductId,
        @event.Name,
        @event.Description,
        metadata.Timestamp,
        metadata.Timestamp,
        0));
  }

  private async Task UpdateProduct(ProductUpdated @event, EventMetadata metadata)
  {
    var product = await _productsRepository.GetProduct(@event.ProductId);

    if (product == null || !TryUpdateVersion(product, metadata.StreamPosition, out product))
    {
      return;
    }

    product = product with
    {
      Name = @event.Name,
      Description = @event.Description,
      UpdatedAt = metadata.Timestamp,
    };

    await _productsRepository.UpdateProduct(product);
  }

  private static bool TryUpdateVersion(Product product, ulong streamPosition, out Product newProduct)
  {
    var expectedVersion = streamPosition - 1;
    if (product.Version >= streamPosition)
    {
      newProduct = product;
      return false;
    }

    if (product.Version != expectedVersion)
    {
      throw new InvalidOperationException($"Version mismatch, expected {expectedVersion}, saw {product.Version}");
    }

    newProduct = product with
    {
      Version = streamPosition,
    };
    return true;
  }
}
