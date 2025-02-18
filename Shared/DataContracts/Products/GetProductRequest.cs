using ProtoBuf;

namespace Shared.DataContracts.Products;

[ProtoContract]
public class GetProductRequest
{
    [ProtoMember(1)] public int Id { get; set; }
}