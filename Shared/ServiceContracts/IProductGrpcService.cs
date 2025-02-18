using System.ServiceModel;
using Shared.DataContracts.Products;

namespace Shared.ServiceContracts;

[ServiceContract]
public interface IProductGrpcService
{
    [OperationContract]
    ValueTask<GetAllProductsResponse> GetAllProductsAsync(
        GetAllProductsRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    ValueTask<AddProductResponse> AddProductAsync(
        AddProductRequest request, CancellationToken cancellationToken = default);

    [OperationContract]
    ValueTask<GetProductResponse> GetProductAsync(
        GetProductRequest request, CancellationToken cancellationToken = default);
}