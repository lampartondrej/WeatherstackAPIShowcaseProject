namespace ShowcaseProject.RestApi.RateLimiting
{
    /// <summary>
    /// Fixed-window rate limiting settings, bound from the "RateLimiting" configuration section.
    /// </summary>
    public sealed class RateLimitingOptions
    {
        public const string SectionName = "RateLimiting";

        public int PermitLimit { get; set; } = 60;
        public int WindowSeconds { get; set; } = 60;
    }
}
