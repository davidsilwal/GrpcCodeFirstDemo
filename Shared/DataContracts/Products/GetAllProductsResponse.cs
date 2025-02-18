using ProtoBuf;

namespace Shared.DataContracts.Products;

[ProtoContract]
public class GetAllProductsResponse
{
    [ProtoMember(1)] public List<GetProductResponse> Products { get; set; }
}