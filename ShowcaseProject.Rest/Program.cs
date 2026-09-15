using Microsoft.AspNetCore.Authentication;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using ShowcaseProject.RestApi.CustomHelpers;
using ShowcaseProject.RestApi.RateLimiting;
using ShowcaseProject.RestApi.Swagger;
using ShowcaseProject.Services;
using ShowcaseProject.Services.Interfaces;

namespace ShowcaseProject
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configure Serilog using the builder's configuration
                try
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(builder.Configuration)
                        .CreateLogger();

                    Log.Information("Starting web application");
                }
                catch (Exception ex)
                {
                    // Fallback to console logging if Serilog configuration fails
                    Log.Logger = new LoggerConfiguration()
                        .WriteTo.Console()
                        .CreateLogger();

                    Log.Warning(ex, "Failed to configure Serilog from configuration. Using console logging as fallback.");
                }

                // Add Serilog to the application
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllers();

                // Configure Basic Authentication
                builder.Services.AddAuthentication("BasicAuthentication")
                    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

                builder.Services.AddAuthorization();

                builder.Services.AddShowcaseRateLimiting(builder.Configuration);

                //register services
                builder.Services.AddHttpClient("WeatherServiceClient")
                    .AddPolicyHandler((serviceProvider, request) =>
                    {
                        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
                        return HttpPolicyExtensions
                            .HandleTransientHttpError()
                            .Or<TimeoutException>()
                            .WaitAndRetryAsync(
                                retryCount: 3,
                                sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                onRetry: (outcome, timespan, retryAttempt, context) =>
                                {
                                    logger.LogWarning(
                                        "Retry attempt {RetryAttempt} for {PolicyKey} due to {Exception} after waiting {Delay}s",
                                        retryAttempt,
                                        context.PolicyKey,
                                        outcome.Exception?.GetType().Name ?? outcome.Result?.StatusCode.ToString(),
                                        timespan.TotalSeconds);
                                });
                    })
                    .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(5)));

                builder.Services.AddMemoryCache();
                builder.Services.AddHealthChecks();
                builder.Services.AddScoped<IWeatherstackRequestBuilder, BuildUriStringForWeatherstack>();
                builder.Services.AddScoped<IWeatherService, WeatherService>();

                // Swagger/OpenAPI, including the API "About" page and XML based endpoint documentation.
                builder.Services.AddShowcaseSwagger();

                var app = builder.Build();

                // Add Serilog request logging middleware
                app.UseSerilogRequestLogging();

                // Configure the HTTP request pipeline.
                // Swagger is intentionally enabled in all environments to allow
                // API exploration by internal consumers and the integration test suite.
                // Restrict access via network/auth policy if needed in production.
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.DocumentTitle = "Weatherstack Showcase API";
                    options.DisplayRequestDuration();
                });

                if (!app.Environment.IsDevelopment())
                {
                    app.UseHsts();
                }

                app.UseHttpsRedirection();

                app.UseRouting();

                // Rate limiting runs before authentication so repeated failed sign-in attempts are throttled.
                app.UseRateLimiter();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers().RequireRateLimiting(RateLimitingExtensions.PerClientPolicy);

                app.MapHealthChecks("/health");

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
