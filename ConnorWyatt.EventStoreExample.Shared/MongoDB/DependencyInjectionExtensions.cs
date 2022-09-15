using Microsoft.Extensions.DependencyInjection;
using MongoDb.Bson.NodaTime;
using MongoDB.Driver;

namespace ConnorWyatt.EventStoreExample.Shared.MongoDB;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddMongoDB(this IServiceCollection services, MongoDBOptions mongoDBOptions)
  {
    NodaTimeSerializers.Register();
    return services
      .AddSingleton(mongoDBOptions)
      .AddSingleton<IMongoClient>(
        serviceProvider => new MongoClient(serviceProvider.GetRequiredService<MongoDBOptions>().ConnectionString));
  }
}
