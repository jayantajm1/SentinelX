using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace SentinelX.AuthService.Middleware;

public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-ID";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.ContainsKey(CorrelationIdHeader)
            ? context.Request.Headers[CorrelationIdHeader].ToString()
            : Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers.Add(CorrelationIdHeader, correlationId);

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}

public class AesDecryptionMiddleware
{
    private readonly RequestDelegate _next;
    private const string ContentEncryptionHeader = "X-Content-Encryption";

    public AesDecryptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Method == "POST" || context.Request.Method == "PUT")
        {
            if (context.Request.Headers.TryGetValue(ContentEncryptionHeader, out var encryptionValue) &&
                encryptionValue == "AES-256-CBC")
            {
                // Decrypt the request body
                context.Request.EnableBuffering();
                var body = context.Request.Body;
                var buffer = new byte[context.Request.ContentLength ?? 0];
                await body.ReadAsync(buffer, 0, buffer.Length);
                context.Request.Body.Position = 0;

                var decryptedBody = DecryptBody(buffer);
                context.Request.Body = new MemoryStream(decryptedBody);
            }
        }

        await _next(context);
    }

    private byte[] DecryptBody(byte[] encryptedData)
    {
        // AES decryption implementation - use a secure key management system in production
        // This is a placeholder implementation
        return encryptedData;
    }
}

public static class LogContext
{
    public static IDisposable PushProperty(string name, object value)
    {
        // Placeholder for structured logging context
        return new DisposableAction(() => { });
    }
}

public class DisposableAction : IDisposable
{
    private readonly Action _action;

    public DisposableAction(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }
}
