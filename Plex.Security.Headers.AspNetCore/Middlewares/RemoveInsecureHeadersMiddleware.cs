namespace Plex.Security.Headers.AspNetCore.Middlewares;

public class RemoveInsecureHeadersMiddleware
{
    private readonly RequestDelegate _next;

    const string _serverKey = "Server";
    const string _xAspNetVersionKey = "X-AspNet-Version";
    const string _xPoweredByKey = "X-Powered-By";
    const string _xHtmlMinificationPoweredByKey = "X-HTML-Minification-Powered-By";
    public RemoveInsecureHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {

        if (context.Response.Headers.ContainsKey(_xAspNetVersionKey))
        {
            context.Response.Headers.Remove(_xAspNetVersionKey);
        }
        if (context.Response.Headers.ContainsKey(_xPoweredByKey))
        {
            context.Response.Headers.Remove(_xPoweredByKey);
        }

        context.Response.OnStarting(state =>
        {
            var httpContext = (HttpContext)state;
            httpContext.Response.Headers.Remove(_serverKey);
            context.Response.Headers.Remove(_xHtmlMinificationPoweredByKey);
            return Task.CompletedTask;
        }, context);

        await _next(context);
    }
}
