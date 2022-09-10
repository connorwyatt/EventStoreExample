using ConnorWyatt.EventStoreExample.Products.Models;

namespace ConnorWyatt.EventStoreExample.Products;

public static class ProductExtensions
{
  public static Product ToApiModel(this Mongo.Product product) => new Product(product.ProductId, product.Name, product.Description, product.AddedAt, product.UpdatedAt);
}
