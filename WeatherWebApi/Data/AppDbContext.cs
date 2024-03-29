using WeatherWebApi.Data.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WeatherWebApi.Data
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<User>(options)
    {
    }
}
