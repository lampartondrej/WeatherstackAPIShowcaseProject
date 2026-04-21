namespace ShowcaseProject.Shared.Model.DTOs.Weatherstack.Forecast.Response
{
    /// <summary>
    /// Represents the forecast data container with dynamic date keys.
    /// </summary>
    public class ForecastWeatherResponseForecast : Dictionary<string, ForecastWeatherResponseDay>
    {
        /// <summary>
        /// Gets the forecast for a specific date.
        /// </summary>
        /// <param name="date">The date in format yyyy-MM-dd or yyyyMMdd</param>
        /// <returns>The forecast data if found, null otherwise</returns>
        public ForecastWeatherResponseDay? GetForecastByDate(string date)
        {
            var key = date.Replace("-", "");
            return this.TryGetValue(key, out var forecast) ? forecast : null;
        }

        /// <summary>
        /// Gets the forecast for a specific date.
        /// </summary>
        /// <param name="date">The date</param>
        /// <returns>The forecast data if found, null otherwise</returns>
        public ForecastWeatherResponseDay? GetForecastByDate(DateTime date)
        {
            var key = date.ToString("yyyyMMdd");
            return this.TryGetValue(key, out var forecast) ? forecast : null;
        }
    }
}