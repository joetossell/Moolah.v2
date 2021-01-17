using Microsoft.AspNetCore.Builder;

namespace Moolah.Monzo.Middleware
{
    public static class RequestResponseLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestResponseLogging(this IApplicationBuilder builder) => builder.UseMiddleware<RequestResponseLoggingMiddleware>();
    }
}
