using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDb.Bson.NodaTime;
using MongoDB.Driver;

namespace ConnorWyatt.EventStoreExample.Shared.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddMongoDB(this IServiceCollection services, IConfiguration configuration)
  {
    NodaTimeSerializers.Register();
    return services.AddSingleton<IMongoClient>(_ => new MongoClient(configuration.GetConnectionString("MongoDB")));
  }
}
