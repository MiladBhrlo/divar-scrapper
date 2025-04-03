using DivarScraper.Core.Data;
using DivarScraper.Core.Settings;
using Microsoft.EntityFrameworkCore;

var settings = new AppSettings
{
    ConnectionStrings = new ConnectionStrings
    {
        DefaultConnection = "Host=localhost;Database=divarscraper;Username=postgres;Password=your_password"
    }
};

using var context = new DivarDbContext(settings);
await context.Database.MigrateAsync(); 