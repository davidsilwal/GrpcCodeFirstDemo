using System.ServiceModel;
using ProtoBuf.Grpc;
using Shared.DataContracts.Products;

namespace Shared.ServiceContracts;

[ServiceContract]
public interface IProductService
{
   // [OperationContract]
    ValueTask<GetAllProductsResponse> GetAllProductsAsync(
        GetAllProductsRequest request,
        CallContext context = default,
        CancellationToken cancellationToken = default);

    [OperationContract]
    ValueTask<AddProductResponse> AddProductAsync(
        AddProductRequest request,
        CallContext context = default,
        CancellationToken cancellationToken = default);

    [OperationContract]
    ValueTask<GetProductResponse> GetProductAsync(
        GetProductRequest request,
        CallContext context = default,
        CancellationToken cancellationToken = default);
}