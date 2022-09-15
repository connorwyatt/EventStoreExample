using System.Reflection;
using ConnorWyatt.EventStoreExample.Shared.Domain;
using ConnorWyatt.EventStoreExample.Shared.EventStore;
using ConnorWyatt.EventStoreExample.Shared.MongoDB;
using ConnorWyatt.EventStoreExample.Shared.Subscriptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConnorWyatt.EventStoreExample.Shared;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSharedServices(
    this IServiceCollection services,
    IConfiguration configuration,
    Assembly assembly) =>
    services
      .AddEventStore(configuration)
      .AddMongoDB(configuration)
      .AddAggregateRepository(assembly)
      .AddSubscriptions();
}
