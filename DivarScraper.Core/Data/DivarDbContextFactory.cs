using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using DivarScraper.Core.Settings;

namespace DivarScraper.Core.Data;

public class DivarDbContextFactory : IDesignTimeDbContextFactory<DivarDbContext>
{
    public DivarDbContext CreateDbContext(string[] args)
    {
        var settings = new AppSettings
        {
            ConnectionStrings = new ConnectionStrings
            {
                DefaultConnection = "Host=localhost;Database=divarscraper;Username=postgres;Password=your_password"
            }
        };

        var optionsBuilder = new DbContextOptionsBuilder<DivarDbContext>();
        optionsBuilder.UseNpgsql(settings.ConnectionStrings.DefaultConnection);

        return new DivarDbContext(settings);
    }
} 