using ConnorWyatt.EventStoreExample.Products.Commands;
using ConnorWyatt.EventStoreExample.Shared.Domain;
using MediatR;

namespace ConnorWyatt.EventStoreExample.Products.Domain;

public class ProductHandlers : IRequestHandler<AddProduct>, IRequestHandler<UpdateProduct>
{
  private readonly AggregateRepository _aggregateRepository;

  public ProductHandlers(AggregateRepository aggregateRepository) => _aggregateRepository = aggregateRepository;

  public async Task<Unit> Handle(AddProduct command, CancellationToken cancellationToken)
  {
    var aggregate = await _aggregateRepository.LoadAggregate<Product>(command.ProductId);

    aggregate.AddProduct(command.Name, command.Description);

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }

  public async Task<Unit> Handle(UpdateProduct command, CancellationToken cancellationToken)
  {
    var aggregate = await _aggregateRepository.LoadAggregate<Product>(command.ProductId);

    aggregate.UpdateProduct(command.Name, command.Description);

    await _aggregateRepository.SaveAggregate(aggregate);

    return Unit.Value;
  }
}
