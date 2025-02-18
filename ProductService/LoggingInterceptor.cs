using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ProductService;

public class LoggingInterceptor(ILogger<LoggingInterceptor> logger) : Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        // Log the incoming request
        logger.LogInformation("Incoming request: {RequestMethod} at {RequestTimestamp}",
            context.Method, context.RequestHeaders);

        try
        {
            // Call the next handler in the pipeline (i.e., the actual service method)
            var response = await continuation(request, context);

            // Log the outgoing response
            logger.LogInformation("Outgoing response: {Response}", response);

            return response;
        }
        catch (Exception ex)
        {
            // Log any exceptions that occur during processing
            logger.LogError(ex, "Error occurred while processing request: {RequestMethod}", context.Method);
            throw;
        }
    }

    public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        ServerCallContext context,
        ClientStreamingServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation("Incoming client streaming request: {RequestMethod} at {RequestTimestamp}",
            context.Method, context.RequestHeaders);

        try
        {
            var response = await continuation(requestStream, context);
            logger.LogInformation("Outgoing response: {Response}", response);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing client streaming request: {RequestMethod}",
                context.Method);
            throw;
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation("Incoming server streaming request: {RequestMethod} at {RequestTimestamp}",
            context.Method, context.RequestHeaders);

        try
        {
            await continuation(request, responseStream, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing server streaming request: {RequestMethod}",
                context.Method);
            throw;
        }
    }

    public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
        IAsyncStreamReader<TRequest> requestStream,
        IServerStreamWriter<TResponse> responseStream,
        ServerCallContext context,
        DuplexStreamingServerMethod<TRequest, TResponse> continuation)
    {
        logger.LogInformation("Incoming duplex streaming request: {RequestMethod} at {RequestTimestamp}",
            context.Method, context.RequestHeaders);

        try
        {
            await continuation(requestStream, responseStream, context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while processing duplex streaming request: {RequestMethod}",
                context.Method);
            throw;
        }
    }
}