using System.Reflection;
using ConnorWyatt.EventStoreExample.Shared.Events;
using Microsoft.Extensions.DependencyInjection;

namespace ConnorWyatt.EventStoreExample.Shared.Domain;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddAggregateRepository(this IServiceCollection services, Assembly assembly) =>
    services
      .AddTransient<AggregateRepository>()
      .AddSingleton(_ => new EventSerializer(assembly));
}
