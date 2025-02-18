using ProtoBuf.Grpc.Configuration;
using ProtoBuf.Grpc.Reflection;
using Shared.ServiceContracts;

// Create a SchemaGenerator instance
var generator = new SchemaGenerator();

// Use reflection to find all types in the current assembly
var assembly = typeof(IProductGrpcService).Assembly;

// Filter types that are attributed with [Service]
foreach (var type in assembly.GetTypes())
{
    if (type.IsInterface && type.GetCustomAttributes(typeof(ServiceAttribute), false).Length > 0)
    {
        Console.WriteLine($"Found gRPC service: {type.Name}");

        // Generate the schema for the service and its messages
        var schema = generator.GetSchema(type);

        // Write the schema to a .proto file
        var protoFileName = $"{type.Name}.proto";
        File.WriteAllText(protoFileName, schema);

        Console.WriteLine($"Generated .proto file: {protoFileName}");
    }
}