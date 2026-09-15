using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using ShowcaseProject.Shared.Model.DTOs.Shared;
using System.Globalization;
using System.Threading.RateLimiting;

namespace ShowcaseProject.RestApi.RateLimiting
{
    /// <summary>
    /// Fixed-window rate limiting partitioned by client IP address.
    /// Registered ahead of authentication so repeated failed sign-in attempts are throttled too.
    /// </summary>
    public static class RateLimitingExtensions
    {
        public const string PerClientPolicy = "per-client";

        public static IServiceCollection AddShowcaseRateLimiting(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy(PerClientPolicy, httpContext =>
                {
                    var limits = httpContext.RequestServices.GetRequiredService<IOptions<RateLimitingOptions>>().Value;

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ResolveClientKey(httpContext),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = limits.PermitLimit,
                            Window = TimeSpan.FromSeconds(limits.WindowSeconds),
                            QueueLimit = 0
                        });
                });

                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo);
                    }

                    context.HttpContext.RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger(typeof(RateLimitingExtensions).FullName!)
                        .LogWarning("Rate limit exceeded for client {Client} on {Path}",
                            ResolveClientKey(context.HttpContext),
                            context.HttpContext.Request.Path);

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsJsonAsync(new DetailedErrorMessage
                    {
                        Message = "Too many requests.",
                        Details = "The request rate limit has been exceeded. Please retry later.",
                        HttpStatusCode = StatusCodes.Status429TooManyRequests
                    }, cancellationToken);
                };
            });

            return services;
        }

        private static string ResolveClientKey(HttpContext httpContext)
            => httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
