using ProtoBuf;

namespace Shared.DataContracts.Products;

[ProtoContract]
public class GetProductResponse
{
    [ProtoMember(1)] public int Id { get; set; }
    [ProtoMember(2)] public string Name { get; set; }
    [ProtoMember(3)] public double Price { get; set; }
}