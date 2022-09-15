using System.Reflection;
using ConnorWyatt.EventStoreExample.Shared.Domain;
using ConnorWyatt.EventStoreExample.Shared.EventStore;
using ConnorWyatt.EventStoreExample.Shared.MongoDB;
using ConnorWyatt.EventStoreExample.Shared.Subscriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace ConnorWyatt.EventStoreExample.Shared;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSharedServices(
    this IServiceCollection services,
    IConfiguration configuration,
    Assembly assembly) =>
    services
      .AddEventStore(configuration)
      .AddMongoDB(
        new MongoDBOptions(
          configuration.GetConnectionString("MongoDB"),
          configuration.GetRequiredSection("MongoDB")["DatabaseName"]))
      .AddAggregateRepository(assembly)
      .AddSubscriptions()
      .AddTransient<IClock>(_ => SystemClock.Instance);
}
