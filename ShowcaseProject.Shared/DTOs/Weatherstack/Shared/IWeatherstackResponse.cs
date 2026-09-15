namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Shared
{
    /// <summary>
    /// Shared shape of every Weatherstack response, allowing provider-level errors
    /// and incomplete payloads to be validated in one place regardless of endpoint.
    /// </summary>
    public interface IWeatherstackResponse
    {
        bool? success { get; set; }
        WeatherstackError? error { get; set; }

        /// <summary>True when the response actually carries the data the endpoint is expected to return.</summary>
        bool HasUsablePayload { get; }
    }
}
