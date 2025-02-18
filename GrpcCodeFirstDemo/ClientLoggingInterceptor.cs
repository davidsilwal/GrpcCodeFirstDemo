using Grpc.Core;
using Grpc.Core.Interceptors;

namespace GrpcCodeFirstDemo;

public class ClientLoggingInterceptor(ILogger<ClientLoggingInterceptor> logger) : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        LogRequest(context.Method.Name, request);

        var call = continuation(request, context);

        return new AsyncUnaryCall<TResponse>(
            HandleResponse(call.ResponseAsync, context.Method.Name),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    private void LogRequest(string methodName, object request)
    {
        logger.LogInformation("Outgoing request: {MethodName}, Payload: {Request}", methodName, request);
    }

    private async Task<TResponse> HandleResponse<TResponse>(Task<TResponse> responseTask, string methodName)
    {
        try
        {
            var response = await responseTask;
            logger.LogInformation("Incoming response: {MethodName}, Payload: {Response}", methodName, response);
            return response;
        }
        catch (RpcException ex)
        {
            logger.LogError(ex, "Error occurred while processing response for method: {MethodName}", methodName);
            throw;
        }
    }

    public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncClientStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        LogRequest(context.Method.Name, null); // No request payload for client streaming initially

        var call = continuation(context);

        return new AsyncClientStreamingCall<TRequest, TResponse>(
            call.RequestStream,
            HandleResponse(call.ResponseAsync, context.Method.Name),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(
        TRequest request,
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncServerStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        LogRequest(context.Method.Name, request);

        var call = continuation(request, context);

        return new AsyncServerStreamingCall<TResponse>(
            new LoggingResponseStream<TResponse>(call.ResponseStream, context.Method.Name, logger),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }

    public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(
        ClientInterceptorContext<TRequest, TResponse> context,
        AsyncDuplexStreamingCallContinuation<TRequest, TResponse> continuation)
    {
        LogRequest(context.Method.Name, null); // No request payload for duplex streaming initially

        var call = continuation(context);

        return new AsyncDuplexStreamingCall<TRequest, TResponse>(
            new LoggingRequestStream<TRequest>(call.RequestStream, context.Method.Name, logger),
            new LoggingResponseStream<TResponse>(call.ResponseStream, context.Method.Name, logger),
            call.ResponseHeadersAsync,
            call.GetStatus,
            call.GetTrailers,
            call.Dispose);
    }
}

public class LoggingResponseStream<T> : IAsyncStreamReader<T>
{
    private readonly IAsyncStreamReader<T> _inner;
    private readonly string _methodName;
    private readonly ILogger _logger;

    public LoggingResponseStream(IAsyncStreamReader<T> inner, string methodName, ILogger logger)
    {
        _inner = inner;
        _methodName = methodName;
        _logger = logger;
    }

    public T Current => _inner.Current;

    public async Task<bool> MoveNext(CancellationToken cancellationToken)
    {
        var result = await _inner.MoveNext(cancellationToken);
        if (result)
        {
            _logger.LogInformation("Incoming streaming response: {MethodName}, Payload: {Response}", _methodName,
                _inner.Current);
        }

        return result;
    }
}

public class LoggingRequestStream<T> : IClientStreamWriter<T>
{
    private readonly IClientStreamWriter<T> _inner;
    private readonly string _methodName;
    private readonly ILogger _logger;

    public LoggingRequestStream(IClientStreamWriter<T> inner, string methodName, ILogger logger)
    {
        _inner = inner;
        _methodName = methodName;
        _logger = logger;
    }

    public WriteOptions WriteOptions
    {
        get => _inner.WriteOptions;
        set => _inner.WriteOptions = value;
    }

    public async Task WriteAsync(T message)
    {
        _logger.LogInformation("Outgoing streaming request: {MethodName}, Payload: {Request}", _methodName, message);
        await _inner.WriteAsync(message);
    }

    public Task CompleteAsync() => _inner.CompleteAsync();
}