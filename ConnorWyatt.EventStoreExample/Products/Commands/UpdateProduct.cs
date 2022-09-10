using MediatR;

namespace ConnorWyatt.EventStoreExample.Products.Commands;

public record UpdateProduct(string ProductId, string Name, string Description) : IRequest;
