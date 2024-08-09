namespace Plex.Security.Headers.AspNetCore.Middlewares;
public class ListEndpointsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly EndpointDataSource _endpointDataSource;

    public ListEndpointsMiddleware(RequestDelegate next,
                                   EndpointDataSource endpointDataSource)
    {
        _next = next;
        _endpointDataSource = endpointDataSource;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/listendpoints"))
        {
            var endpoints = _endpointDataSource.Endpoints;

            context.Response.ContentType = "text/plain";
            foreach (var endpoint in endpoints)
            {
                if (endpoint is RouteEndpoint routeEndpoint)
                {
                    var routePattern = routeEndpoint.RoutePattern.RawText;
                    await context.Response.WriteAsync($"{routePattern}\n");
                }
            }
            return;
        }

        await _next(context);
    }
}