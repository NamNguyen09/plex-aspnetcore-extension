using System.Text;
using Microsoft.Net.Http.Headers;

namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class ContentSecurityPolicyMetaMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly string _cspHeader;
    private readonly string _nonceValue;

    public ContentSecurityPolicyMetaMiddleware(ILogger<ContentSecurityPolicyMetaMiddleware> logger,
                                               RequestDelegate next,
                                               string cspHeader,
                                               string nonceValue)
    {
        _logger = logger;
        _next = next;
        _cspHeader = cspHeader;
        _nonceValue = nonceValue;
    }

    public async Task Invoke(HttpContext ctx)
    {
        if (!ctx.Response.Headers.ContainsKey(HeaderNames.ContentSecurityPolicy)
                  && !string.IsNullOrWhiteSpace(_cspHeader))
        {
            if (HttpMethods.IsGet(ctx.Request.Method)
                && !string.IsNullOrWhiteSpace(ctx.Request.Headers.Accept)
                && ctx.Request.Headers.Accept.Contains("html"))
            {
               await AddCspMetaToResponseBody(ctx);
            }
        }

        await _next(ctx);
    }

    async Task AddCspMetaToResponseBody(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;
        try
        {
            using (var newBodyStream = new MemoryStream())
            {
                context.Response.Body = newBodyStream;
                await _next(context);

                if (context.Response.ContentType != null && context.Response.ContentType.Contains("text/html"))
                {
                    newBodyStream.Position = 0;
                    using (var reader = new StreamReader(newBodyStream))
                    {
                        string newContent = await reader.ReadToEndAsync();
                        string updatedHtml = newContent.Replace("</head>", $"<meta property=\"csp-nonce\" nonce=\"{_nonceValue}\"></head>")
                                                       .Replace("></script>", $" nonce=\"{_nonceValue}\"></script>")
                                                       .Replace("<link ", $"<link nonce=\"{_nonceValue}\" ")
                                                       .Replace("<style", $"<style nonce=\"{_nonceValue}\"")
                                                       .Replace("style=", $" nonce=\"{_nonceValue}\" style=")
                                                       .Replace("{nonce}", _nonceValue);

                        context.Response.ContentLength = Encoding.UTF8.GetByteCount(updatedHtml);

                        using (var updatedStream = new MemoryStream(Encoding.UTF8.GetBytes(updatedHtml)))
                        {
                            await updatedStream.CopyToAsync(originalBodyStream);
                        }
                    }
                }
                else
                {
                    newBodyStream.Position = 0;
                    await newBodyStream.CopyToAsync(originalBodyStream);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error when add CSP meta tag to response");
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }
}
