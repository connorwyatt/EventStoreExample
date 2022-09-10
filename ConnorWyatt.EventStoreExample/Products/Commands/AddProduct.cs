using MediatR;

namespace ConnorWyatt.EventStoreExample.Products.Commands;

public record AddProduct(string ProductId, string Name, string Description) : IRequest;
