using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using GrpcCodeFirstDemo;
using ProtoBuf.Grpc.Client;
using Shared.DataContracts.Products;
using Shared.ServiceContracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddSingleton(sp =>
    {
        var logger = sp.GetRequiredService<ILogger<ClientLoggingInterceptor>>();
        var channel = GrpcChannel.ForAddress("https://localhost:7241");
        channel.Intercept(new ClientLoggingInterceptor(logger));
        return channel;
    });

builder.Services.AddScoped(sp =>
{
    var channel = sp.GetRequiredService<GrpcChannel>();
    return channel.CreateGrpcService<IProductGrpcService>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/products", async (IProductGrpcService productService) =>
{
    var response = await productService.GetAllProductsAsync(new GetAllProductsRequest());
    return Results.Ok(response.Products);
});

app.MapPost("/products", async (IProductGrpcService productService, AddProductRequest request) =>
{
    var response = await productService.AddProductAsync(request);
    return Results.Ok(response);
});

app.MapGet("/products/{id}", async (IProductGrpcService productService, int id) =>
{
    var response = await productService.GetProductAsync(new GetProductRequest { Id = id });
    return Results.Ok(response);
});

app.Run();