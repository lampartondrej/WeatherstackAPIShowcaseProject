namespace ShowcaseProject.Shared.Model.Wrapper
{
    /// <summary>
    /// Classifies why a service call failed so callers can translate the failure
    /// into an appropriate HTTP status code.
    /// </summary>
    public enum ServiceErrorKind
    {
        None = 0,

        /// <summary>The caller supplied something the upstream provider rejected (bad location, unsupported unit).</summary>
        InvalidRequest,

        /// <summary>The upstream provider responded, but with an error or an unusable payload.</summary>
        UpstreamError,

        /// <summary>The upstream provider could not be reached at all (network failure, timeout).</summary>
        UpstreamUnavailable,

        /// <summary>An unexpected failure on our side.</summary>
        Unexpected
    }
}
