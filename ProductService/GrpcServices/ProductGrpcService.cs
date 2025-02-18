using Grpc.Core;
using Shared.DataContracts.Products;
using Shared.ServiceContracts;

namespace ProductService.GrpcServices;

public class ProductGrpcService(ILogger<ProductGrpcService> logger) : IProductGrpcService
{
    private readonly List<Product> _products = [];

    public ValueTask<AddProductResponse> AddProductAsync(
        AddProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = new Product
        {
            Id = _products.Count + 1,
            Name = request.Name,
            Price = request.Price
        };

        _products.Add(product);

        logger.LogInformation("Product added: {Product}", product.Name);

        var result = new AddProductResponse { Id = product.Id };
        return new ValueTask<AddProductResponse>(result);
    }

    public ValueTask<GetProductResponse> GetProductAsync(GetProductRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = _products.Find(p => p.Id == request.Id);

        if (product == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Product not found"));
        }

        var result = new GetProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = (double)product.Price
        };

        return new ValueTask<GetProductResponse>(result);
    }

    public ValueTask<GetAllProductsResponse> GetAllProductsAsync(GetAllProductsRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = new GetAllProductsResponse();
        response.Products.AddRange(_products.Select(p => new GetProductResponse
        {
            Id = p.Id,
            Name = p.Name,
            Price = (double)p.Price
        }));

        return new ValueTask<GetAllProductsResponse>(response);
    }
}