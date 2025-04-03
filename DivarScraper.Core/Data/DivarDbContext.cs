using Microsoft.EntityFrameworkCore;
using DivarScraper.Core.Models;
using DivarScraper.Core.Settings;

namespace DivarScraper.Core.Data
{
    public class DivarDbContext : DbContext
    {
        private readonly AppSettings _settings;

        public DivarDbContext(AppSettings settings)
        {
            _settings = settings;
        }

        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<CarBrand> CarBrands { get; set; }
        public DbSet<CarModel> CarModels { get; set; }
        public DbSet<CarAd> CarAds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = $"Host={_settings.DbHost};Database={_settings.DbName};Username={_settings.DbUser};Password={_settings.DbPassword}";
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasIndex(c => c.DivarId)
                .IsUnique();

            modelBuilder.Entity<District>()
                .HasIndex(d => new { d.CityId, d.DivarId })
                .IsUnique();

            modelBuilder.Entity<CarBrand>()
                .HasIndex(b => b.DivarId)
                .IsUnique();

            modelBuilder.Entity<CarModel>()
                .HasIndex(m => new { m.BrandId, m.DivarId })
                .IsUnique();

            modelBuilder.Entity<CarAd>()
                .HasIndex(a => a.Token)
                .IsUnique();
        }
    }
} 