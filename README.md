# WeatherWebApi
This project is a RESTful API developed with ASP.NET Core Web API, designed to serve an Angular SPA for displaying weather information. 
It features user authentication and authorization with JWT bearer tokens, data persistence in MySQL, and integrates with the OpenWeatherMap API.

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [MySQL/MariaDB](https://www.mysql.com/downloads/)
- IIS for hosting (Optional for development)

### Installation
You'll just need to clone the repo, update appsettings with your settings and connection string. Build and run.
One tricky part is that you should register on [openweatherapi](https://openweathermap.org/api) to get your apikey.

### Endpoints
POST /api/auth/register: Registers a new user.
POST /api/auth/login: Authenticates a user and returns a JWT.
GET /api/weather/week-summary: Retrieves weather information. Requires authentication.

### Security
This project uses ASP.NET Core Identity for user management and role-based authorization. Passwords are hashed and stored securely. JWT tokens are used for authentication.

### Deployment
Instructions for deploying to IIS.
It is pretty simple I've followed this youtube [video]([https://www.youtube.com/watch?v=Lt3wve_nb0g&t=186s&ab_channel=IntCoder](https://www.youtube.com/watch?v=kMmZ9SbPBQA&t=121s&ab_channel=MacroCode)).

Angular SPA [link](https://github.com/stefankrstevski/WeatherApp). 
