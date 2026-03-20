# PrezentacniProjekt

A ASP.Net MVC project built with .NET 8 and C# 12.0 that provides weather information by integrating with the Weatherstack API.
The project includes a service-oriented architecture for retrieving current and forecast weather data, with robust error handling and configuration via environment variables and `appsettings.json`.

## Features

- Retrieve current weather data for a specified location.
- Retrieve weather forecasts for a specified location and number of days.
- Clean separation of concerns using service interfaces and dependency injection.
- Configuration-driven API key and endpoint management.
- Detailed error reporting.

## Technologies

- ASP.NET (.NET 8)
- C# 12.0
- Weatherstack API
- Dependency Injection
- Logging (ILogger)
- IHttpClientFactory

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 or later

### Configuration

1. **Set up environment variables:**

   The Weatherstack API key is read from an environment variable. Set the variable name in `appsettings.json` under `APIOptions:WeatherstackApiKeyEnvVar`, and set the actual API key in your environment.
   

## Usage

The main weather functionality is provided by the `WeatherService` class, which implements `IWeatherService`. It exposes two main methods:

- `GetCurrentWeather(GetCurrentWeatherRequest request)`
- `GetForecastWeather(GetForecastWeatherRequest request)`

These methods return a tuple containing the response DTO and a detailed error message (if any).
