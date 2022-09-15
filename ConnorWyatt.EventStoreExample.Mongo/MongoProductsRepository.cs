using ConnorWyatt.EventStoreExample.Shared.MongoDB;
using MongoDB.Driver;

namespace ConnorWyatt.EventStoreExample.Mongo;

public class MongoProductsRepository
{
  private const string CollectionName = "products";

  private readonly IMongoCollection<Product> _collection;

  public MongoProductsRepository(IMongoClient mongoClient, MongoDBOptions mongoDBOptions)
  {
    var database = mongoClient.GetDatabase(mongoDBOptions.DatabaseName);
    _collection = database.GetCollection<Product>(CollectionName);
  }

  public async Task<Product?> GetProduct(string productId)
  {
    return await _collection.Find(p => p.ProductId == productId).SingleOrDefaultAsync();
  }

  public async Task<IList<Product>> GetProducts()
  {
    return await _collection.Find(FilterDefinition<Product>.Empty).SortByDescending(p => p.AddedAt).ToListAsync();
  }

  public async Task InsertProduct(Product product)
  {
    await _collection.InsertOneAsync(product);
  }

  public async Task UpdateProduct(Product product)
  {
    await _collection.ReplaceOneAsync(p => p.ProductId == product.ProductId, product);
  }
}
