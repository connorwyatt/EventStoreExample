using System.Reflection;
using ConnorWyatt.EventStoreExample.Mongo;
using ConnorWyatt.EventStoreExample.Products.Projections;
using ConnorWyatt.EventStoreExample.Shared;
using ConnorWyatt.EventStoreExample.Shared.Subscriptions;
using MediatR;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

var executingAssembly = Assembly.GetExecutingAssembly();

var builder = WebApplication.CreateBuilder(args);

var configuration = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json")
  .Build();

builder.Services.AddControllers().AddJsonOptions(
  options =>
  {
    options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
  });
builder.Services.AddMediatR(executingAssembly);

builder.Services.AddSharedServices(configuration, executingAssembly);

builder.Services.AddSubscriber<ProductsProjection>();
builder.Services.AddSingleton<MongoProductsRepository>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
