//TODO: Add screenshots/screenshare b4 master PR
# 🌦️ Weather Forecast App (.NET)

A modern **ASP.NET Core application** for retrieving current weather and forecasts using an external API.
This project demonstrates practical usage of **REST APIs, MVC architecture, configuration management, authentication, and clean service design**.

---

## 🚀 Overview

The application consists of two main parts:

* **REST API** – handles communication with the external Weather API
* **MVC Web App** – provides a user interface for displaying weather data

The system is designed to separate concerns between presentation, business logic, and external integrations.

---

## 🧱 Architecture

```
[ MVC Web App ]
        ↓
[ ASP.NET Core API ]
        ↓
[ External Weather API ]
```

* The web app communicates with the internal API
* The API processes requests, handles authentication, and calls the external service
* Data is mapped into DTO models for structured handling

---

## 🛠️ Technologies

* .NET 8 / ASP.NET Core
* ASP.NET MVC
* REST API
* C#
* HttpClientFactory
* Serilog (logging)
* Basic Authentication
* Swagger / OpenAPI

---

## ✨ Features

* 🔍 Search weather by city
* 🌡️ Current weather (temperature, humidity, wind, etc.)
* 📅 Multi-day forecast
* 🔐 Secured API (Basic Authentication)
* 📜 Structured logging
* ⚙️ Configuration via environment variables

---

## 🖼️ Screenshots

### Current view

![Current](./docs/screenshots/current.png)

### Forecast view

![Forecast](./docs/screenshots/forecast.png)

Note: Weatherstack API does not provide weather forcast on their open api model and it is paid function.
For this showcase project weather forecast is disabled and only simulated.

### Swagger API

![Swagger](./docs/screenshots/swagger.png)

---

## ⚙️ Getting Started

### 1. Clone repository

```bash
git clone https://github.com/lampartondrej/PrezentacniProjekt.git
cd PrezentacniProjekt
```

---

### 2. Configure environment variables

Create a `.env` file or set variables manually:

```bash
WeatherApi__ApiKey=YOUR_API_KEY
BasicAuth__Username=YOUR_GUID_USERNAME
BasicAuth__Password=YOUR_GUID_PASSWORD
```

---

### 3. Run API

```bash
cd PrezentacniProjekt
dotnet run
```

API will be available at:

```
https://localhost:5001
```

Swagger UI:

```
https://localhost:5001/swagger
```

---

### 4. Run Web App

```bash
cd PrezentacniProjekt.Web
dotnet run
```

Web app will be available at:

```
https://localhost:5002
```

---

## 🔌 API Endpoints (examples)

### Get current weather

```
GET /api/weather/current?city=Prague
```

### Get forecast

```
GET /api/weather/forecast?city=Prague&days=3
```

---

## 🔐 Authentication

The API uses **Basic Authentication**.

Example header:

```
Authorization: Basic base64(username:password)
```

---

## 🧪 Testing (planned improvements)

* [ ] Unit tests for service layer
* [ ] HttpClient mocking
* [ ] Integration tests

---

## 🚧 Possible Improvements

* Caching (MemoryCache / Redis)
* Retry policies (Polly)
* Rate limiting
* Docker support
* CI/CD pipeline
* Improved error handling
* Frontend upgrade (React / Blazor)

---

## 📌 What This Project Demonstrates

* Integration with external APIs
* Multi-layer application design
* Dependency injection usage
* Configuration & environment handling
* Logging and error handling
* Basic API security concepts

---

## 👨‍💻 Author

Ondřej Lampart
GitHub: https://github.com/lampartondrej

---

## 📄 License

This project is intended for demonstration and portfolio purposes.
