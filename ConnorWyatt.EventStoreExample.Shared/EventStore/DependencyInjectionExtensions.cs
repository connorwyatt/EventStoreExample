using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConnorWyatt.EventStoreExample.Shared.EventStore;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddEventStore(this IServiceCollection services, IConfiguration configuration) =>
    services.AddEventStoreClient(configuration.GetConnectionString("EventStore")).AddTransient<EventStoreWrapper>();
}
