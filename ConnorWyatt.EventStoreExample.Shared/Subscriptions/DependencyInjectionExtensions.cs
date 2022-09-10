using Microsoft.Extensions.DependencyInjection;

namespace ConnorWyatt.EventStoreExample.Shared.Subscriptions;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddSubscriber<T>(this IServiceCollection services) where T : class, ISubscriber
  {
    return services.AddTransient<ISubscriber, T>();
  }

  public static IServiceCollection AddSubscriptions(this IServiceCollection services)
  {
    return services.AddHostedService<SubscriptionsManager>();
  }
}
