namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Shared
{
    /// <summary>
    /// Error payload returned by Weatherstack. The provider reports application-level
    /// failures with HTTP 200 and this object in the body instead of an error status code.
    /// </summary>
    public class WeatherstackError
    {
        public int code { get; set; }
        public string? type { get; set; }
        public string? info { get; set; }
    }
}
