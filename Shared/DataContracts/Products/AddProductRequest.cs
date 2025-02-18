using ProtoBuf;

namespace Shared.DataContracts.Products;

[ProtoContract]
public class AddProductRequest
{
    [ProtoMember(1)] public string Name { get; set; }
    [ProtoMember(2)] public decimal Price { get; set; }
}