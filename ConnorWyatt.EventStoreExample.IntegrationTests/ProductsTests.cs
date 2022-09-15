using System.Net.Http.Json;
using ConnorWyatt.EventStoreExample.Products.Models;
using ConnorWyatt.EventStoreExample.Shared.Serialization;
using FluentAssertions;
using FluentAssertions.Execution;
using NodaTime;
using Polly;
using Polly.Retry;

namespace ConnorWyatt.EventStoreExample.IntegrationTests;

public class ProductsTests : IClassFixture<EventStoreExampleWebApplicationFactory>
{
  private static readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy =
    Policy.HandleResult<HttpResponseMessage>(httpResponseMessage => !httpResponseMessage.IsSuccessStatusCode)
      .WaitAndRetryAsync(50, _ => TimeSpan.FromMilliseconds(100));

  private readonly HttpClient _client;

  public ProductsTests(EventStoreExampleWebApplicationFactory factory) => _client = factory.CreateClient();

  [Fact]
  public async Task When_Adding_A_Product__Then_It_Can_Be_Retrieved()
  {
    var testStartTime = SystemClock.Instance.GetCurrentInstant();

    var productDefinition = new ProductDefinition("Pencil", "A graphite writing implement.");

    var postResponseMessage = await _client.PostAsJsonAsync(
      "products",
      productDefinition,
      DefaultJsonSerializerOptions.Options);

    postResponseMessage.IsSuccessStatusCode.Should().BeTrue();

    var reference =
      await postResponseMessage.Content.ReadFromJsonAsync<ProductReference>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to Reference.");

    var getResponseMessage =
      await RetryPolicy.ExecuteAsync(() => _client.GetAsync($"products/{reference.ProductId}"));

    var product =
      await getResponseMessage.EnsureSuccessStatusCode()
        .Content.ReadFromJsonAsync<Product>(DefaultJsonSerializerOptions.Options) ??
      throw new InvalidOperationException("Could not deserialize to Product.");

    var testEndTime = SystemClock.Instance.GetCurrentInstant();

    using (new AssertionScope())
    {
      product.ProductId.Should().Be(reference.ProductId);
      product.Name.Should().Be("Pencil");
      product.Description.Should().Be("A graphite writing implement.");
      product.AddedAt.Should().BeInRange(testStartTime, testEndTime);
      product.UpdatedAt.Should().BeInRange(testStartTime, testEndTime);
      product.AddedAt.Should().Be(product.UpdatedAt);
    }
  }
}
