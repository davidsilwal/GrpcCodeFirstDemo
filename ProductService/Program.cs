using ProtoBuf.Grpc.Server;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCodeFirstGrpc();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapGrpcService<ProductService.GrpcServices.ProductGrpcService>();

app.Run();