using Microsoft.AspNetCore.Authentication;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Sinks.MSSqlServer;

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
                builder.Services.AddScoped<Services.Interfaces.IWeatherService, Services.WeatherService>();

                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("basic", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                        Scheme = "basic",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                        Description = "Basic Authorization header using the Bearer scheme."
                    });
                    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                    {
                        {
                            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                            {
                                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                                {
                                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                    Id = "basic"
                                }
                            },
                            new string[] {}
                        }
                    });
                });

                var app = builder.Build();

                // Add Serilog request logging middleware
                app.UseSerilogRequestLogging();

                // Configure the HTTP request pipeline.
                //if (app.Environment.IsDevelopment())
                //{
                app.UseSwagger();
                app.UseSwaggerUI();
                //}

                app.UseHttpsRedirection();

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

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
