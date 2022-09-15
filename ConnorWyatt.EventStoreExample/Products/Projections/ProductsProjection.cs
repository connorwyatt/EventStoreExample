using ConnorWyatt.EventStoreExample.Mongo;
using ConnorWyatt.EventStoreExample.Products.Events;
using ConnorWyatt.EventStoreExample.Shared.Events;
using ConnorWyatt.EventStoreExample.Shared.Subscriptions;

namespace ConnorWyatt.EventStoreExample.Products.Projections;

[Subscription("$ce-products", "ProductsProjection")]
public class ProductsProjection : SubscriberBase
{
  private readonly MongoProductsRepository _productsRepository;

  public ProductsProjection(MongoProductsRepository productsRepository)
  {
    _productsRepository = productsRepository;

    When<ProductAdded>(AddProduct);
    When<ProductUpdated>(UpdateProduct);
  }

  private async Task AddProduct(EventEnvelope<ProductAdded> eventEnvelope)
  {
    var product = await _productsRepository.GetProduct(eventEnvelope.Event.ProductId);

    if (product != null)
    {
      return;
    }

    await _productsRepository.InsertProduct(
      new Product(
        eventEnvelope.Event.ProductId,
        eventEnvelope.Event.Name,
        eventEnvelope.Event.Description,
        eventEnvelope.Metadata.Timestamp,
        eventEnvelope.Metadata.Timestamp,
        0));
  }

  private async Task UpdateProduct(EventEnvelope<ProductUpdated> eventEnvelope)
  {
    var product = await _productsRepository.GetProduct(eventEnvelope.Event.ProductId);

    if (product == null || !TryUpdateVersion(product, eventEnvelope.Metadata.StreamPosition, out product))
    {
      return;
    }

    product = product with
    {
      Name = eventEnvelope.Event.Name,
      Description = eventEnvelope.Event.Description,
      UpdatedAt = eventEnvelope.Metadata.Timestamp,
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
