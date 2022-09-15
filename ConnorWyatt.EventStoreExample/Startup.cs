using System.Reflection;
using ConnorWyatt.EventStoreExample.Mongo;
using ConnorWyatt.EventStoreExample.Products.Projections;
using ConnorWyatt.EventStoreExample.Shared;
using ConnorWyatt.EventStoreExample.Shared.Subscriptions;
using MediatR;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace ConnorWyatt.EventStoreExample;

public class Startup
{
  private readonly IConfiguration _configuration;

  public Startup(IConfiguration configuration) => _configuration = configuration;

  public void ConfigureServices(IServiceCollection services)
  {
    var executingAssembly = Assembly.GetExecutingAssembly();

    services.AddControllers()
      .AddJsonOptions(options => { options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb); });
    services.AddMediatR(executingAssembly);

    services.AddSharedServices(_configuration, executingAssembly);

    services.AddSubscriber<ProductsProjection>();
    services.AddSingleton<MongoProductsRepository>();
  }

  public void Configure(WebApplication app)
  {
    app.UseHttpsRedirection();

    app.MapControllers();
  }
}
