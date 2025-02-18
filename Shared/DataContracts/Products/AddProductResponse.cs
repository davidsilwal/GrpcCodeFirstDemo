using ProtoBuf;

namespace Shared.DataContracts.Products;

[ProtoContract]
public class AddProductResponse
{
    [ProtoMember(1)] public int Id { get; set; }
}