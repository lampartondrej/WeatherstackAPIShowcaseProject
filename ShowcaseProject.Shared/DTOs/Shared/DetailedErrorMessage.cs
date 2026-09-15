namespace ShowcaseProject.Shared.Model.DTOs.Shared
{
    /// <summary>
    /// Error body returned by every failing endpoint.
    /// </summary>
    public class DetailedErrorMessage
    {
        /// <summary>Short, human readable description of what went wrong.</summary>
        public required string Message { get; set; }

        /// <summary>Additional context, including the weather provider's own error text when available.</summary>
        public required string Details { get; set; }

        /// <summary>HTTP status code that accompanies this error.</summary>
        public required int HttpStatusCode { get; set; }
    }
}
