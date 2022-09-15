using ConnorWyatt.EventStoreExample.Mongo;
using ConnorWyatt.EventStoreExample.Products.Commands;
using ConnorWyatt.EventStoreExample.Products.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ConnorWyatt.EventStoreExample.Products;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
  private readonly IMediator _mediator;
  private readonly MongoProductsRepository _productsRepository;

  public ProductsController(IMediator mediator, MongoProductsRepository productsRepository)
  {
    _mediator = mediator;
    _productsRepository = productsRepository;
  }

  [HttpGet]
  [Route("")]
  public async Task<IActionResult> GetProducts()
  {
    var products = await _productsRepository.GetProducts();

    return Ok(products.Select(p => p.ToApiModel()));
  }

  [HttpGet]
  [Route("{productId}")]
  public async Task<IActionResult> GetProduct([FromRoute] string productId)
  {
    var product = await _productsRepository.GetProduct(productId);

    if (product == null)
    {
      return NotFound();
    }

    return Ok(product.ToApiModel());
  }

  [HttpPost]
  [Route("")]
  public async Task<IActionResult> AddProduct([FromBody] ProductDefinition productDefinition)
  {
    var id = Guid.NewGuid().ToString();

    await _mediator.Send(new AddProduct(id, productDefinition.Name, productDefinition.Description));

    return Accepted(new ProductReference(id));
  }

  [HttpPatch]
  [Route("{productId}")]
  public async Task<IActionResult> UpdateProduct(
    [FromRoute] string productId,
    [FromBody]
    ProductPatchDefinition productPatchDefinition)
  {
    var product = await _productsRepository.GetProduct(productId);

    if (product == null)
    {
      return NotFound();
    }

    await _mediator.Send(new UpdateProduct(productId, productPatchDefinition.Name, productPatchDefinition.Description));

    return Accepted();
  }
}
