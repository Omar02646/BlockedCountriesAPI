# 🌍 Country Blocking API

## 📌 About the Project

This is a .NET 8 Web API that allows blocking countries based on IP addresses.
The system supports permanent and temporary blocking, IP geolocation lookup using a third-party API, and logging of blocked access attempts.

---

## 🚀 Features

* Block a country permanently
* Remove a blocked country
* Get all blocked countries (with pagination & filtering)
* Lookup country information using IP address
* Check if the current user's IP is blocked
* Log blocked access attempts (IP, time, country, user agent)
* Temporarily block countries with expiration
* Background service to automatically remove expired blocks

---

## 🛠 Technologies Used

* .NET 8 Web API
* HttpClient for external API calls
* ip-api.com (IP Geolocation)
* In-Memory Storage using ConcurrentDictionary (Thread-safe)
* Background Hosted Service
* Swagger (OpenAPI)

---

## ⚙️ How to Run

```bash
git clone https://github.com/Omar02646/BlockedCountriesAPI.git
cd BlockedCountriesAPI
dotnet restore
dotnet run
```

Then open Swagger in your browser:
[https://localhost:{port}/swagger](https://localhost:{port}/swagger)

---

## 📡 API Endpoints

### Countries

* POST /api/countries/block
* DELETE /api/countries/block/{countryCode}
* GET /api/countries/blocked
* POST /api/countries/temporal-block

### IP

* GET /api/ip/lookup?ipAddress={ip}
* GET /api/ip/check-block

### Logs

* GET /api/logs/blocked-attempts

---

## 🧪 Notes

* The application uses in-memory storage (no database).
* Temporary blocks expire automatically using a background service.
* All external API calls are handled using HttpClient with async/await.
* Input validation and error handling are implemented.

---

## 📄 Author

Omar Mohamed
