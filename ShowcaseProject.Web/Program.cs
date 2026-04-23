using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace ShowcaseProject.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add HttpClient for API calls with retry and circuit-breaker resilience
            builder.Services.AddHttpClient("WeatherApi", client =>
            {
                client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "https://localhost:7001");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddResilienceHandler("weather-resilience", pipeline =>
            {
                // Retry up to 3 times with exponential back-off for transient errors
                pipeline.AddRetry(new HttpRetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = args => ValueTask.FromResult(
                        args.Outcome.Exception is HttpRequestException ||
                        (args.Outcome.Result?.StatusCode is System.Net.HttpStatusCode status &&
                         (status == System.Net.HttpStatusCode.RequestTimeout ||
                          status == System.Net.HttpStatusCode.ServiceUnavailable ||
                          status == System.Net.HttpStatusCode.GatewayTimeout ||
                          status == System.Net.HttpStatusCode.BadGateway)))
                });

                // Per-attempt timeout of 10 s so a slow API doesn't block the retry loop
                pipeline.AddTimeout(TimeSpan.FromSeconds(10));

                // Open the circuit after 5 consecutive failures; half-open after 30 s
                pipeline.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
                {
                    SamplingDuration = TimeSpan.FromSeconds(60),
                    MinimumThroughput = 5,
                    FailureRatio = 0.5,
                    BreakDuration = TimeSpan.FromSeconds(30)
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
