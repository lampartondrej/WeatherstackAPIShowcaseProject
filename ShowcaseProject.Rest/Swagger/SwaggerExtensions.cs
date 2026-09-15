using Microsoft.OpenApi.Models;

namespace ShowcaseProject.RestApi.Swagger
{
    /// <summary>
    /// Swagger/OpenAPI registration, including the API "About" page shown at the top of Swagger UI.
    /// </summary>
    public static class SwaggerExtensions
    {
        private const string BasicSecuritySchemeId = "basic";

        private static readonly string[] XmlDocumentationFiles =
        [
            "ShowcaseProject.RestApi.xml",
            "ShowcaseProject.Shared.Model.xml"
        ];

        public static IServiceCollection AddShowcaseSwagger(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Weatherstack Showcase API",
                    Version = "v1",
                    Description = ApiDescription
                });

                options.AddSecurityDefinition(BasicSecuritySchemeId, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "basic",
                    In = ParameterLocation.Header,
                    Description = "Basic authentication. Provide credentials in the format: Basic <base64(username:password)>."
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = BasicSecuritySchemeId
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                foreach (var xmlFile in XmlDocumentationFiles)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    if (File.Exists(xmlPath))
                    {
                        options.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                    }
                }
            });

            return services;
        }

        /// <summary>
        /// Markdown rendered by Swagger UI underneath the API title.
        /// </summary>
        private const string ApiDescription = """
            A showcase REST API that wraps the public [Weatherstack](https://weatherstack.com) provider
            and demonstrates production-oriented backend practices in a small, complete .NET solution.

            ## Authentication

            Every endpoint below requires **HTTP Basic authentication**.

            * Credentials are supplied to the server through environment variables — never from source control.
              The variable names themselves are configured in `appsettings.json` (`AuthSettings:UsernameEnvVar` / `AuthSettings:PasswordEnvVar`).
            * The submitted username and password are compared in **constant time**, without short-circuiting,
              so neither timing nor early exit reveals how much of a guessed credential was correct.
            * A successful check produces a `ClaimsPrincipal`; a failed or missing header returns
              **401 Unauthorized** together with a `WWW-Authenticate: Basic` challenge.
            * To try the endpoints here, click **Authorize** above and enter the username and password.

            ## Rate limiting

            Requests are throttled by a **fixed-window limiter partitioned per client IP address**.

            * Defaults to **60 requests per 60 seconds**, configurable via `RateLimiting:PermitLimit`
              and `RateLimiting:WindowSeconds`.
            * The limiter runs **before authentication**, so repeated failed sign-in attempts are throttled
              as well — the endpoints are not a free brute-force target.
            * Exceeding the window returns **429 Too Many Requests** with a `Retry-After` header
              and a JSON error body.
            * The health endpoint is deliberately excluded so infrastructure probes are never throttled.

            ## Resilience (Polly)

            Outbound calls to Weatherstack go through a named `HttpClient` wrapped in Polly policies:

            * **Retry** — up to 3 attempts with exponential back-off (2s, 4s, 8s) for transient HTTP
              failures and timeouts, with each retry logged.
            * **Timeout** — 5 seconds per attempt, so one slow upstream call cannot block the request.

            The MVC frontend that consumes this API adds its own resilience pipeline on top
            (retry with jitter, per-attempt timeout and a circuit breaker).

            ## Caching

            Successful responses are cached in-process using `IMemoryCache`.

            * **10 minute** lifetime, which keeps the free Weatherstack quota usable.
            * The cache key contains the endpoint **and every request parameter**
              (location, units, language, forecast days, interval…), so different queries never collide.
            * **Only successful responses are cached** — provider errors, incomplete payloads and
              network failures are never stored, so a transient problem cannot be served for 10 minutes.

            ## Error handling

            Weatherstack answers with **HTTP 200 even when a request failed** (invalid access key,
            unknown location, exhausted quota), reporting the problem with `success: false` and an
            `error` object in the body. This API inspects that payload instead of trusting the status
            code alone, and translates the outcome:

            | Situation | Status |
            |---|---|
            | Request rejected by the provider (unknown location, invalid parameter) | `400 Bad Request` |
            | Provider error or unusable payload (invalid key, quota exhausted) | `502 Bad Gateway` |
            | Provider unreachable, timed out or retries exhausted | `503 Service Unavailable` |
            | Unexpected failure inside this API | `500 Internal Server Error` |

            Every error response uses the same `DetailedErrorMessage` shape.

            ## Endpoints

            | Method | Route | Description |
            |---|---|---|
            | `POST` | `/Weather/current` | Current weather conditions for a location. |
            | `POST` | `/Weather/forecast` | Forecast (and current conditions) for a location. |
            | `GET` | `/health` | Liveness probe — no authentication, not rate limited. |

            > Forecast support depends on the Weatherstack plan in use; the endpoint and its flow are
            > implemented regardless of provider-tier constraints.
            """;
    }
}
