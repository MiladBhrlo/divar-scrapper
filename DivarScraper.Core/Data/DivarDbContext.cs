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

        public DbSet<CarAd> CarAds { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<CarBrand> CarBrands { get; set; }
        public DbSet<CarModel> CarModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_settings.ConnectionStrings.DefaultConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // تنظیمات جدول City
            modelBuilder.Entity<City>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.DivarId).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.PersianName).IsRequired();
            });

            // تنظیمات جدول District
            modelBuilder.Entity<District>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.CityId, e.DivarId }).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.PersianName).IsRequired();
                entity.HasOne(d => d.City)
                    .WithMany()
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // تنظیمات جدول CarBrand
            modelBuilder.Entity<CarBrand>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.DivarId).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.PersianName).IsRequired();
            });

            // تنظیمات جدول CarModel
            modelBuilder.Entity<CarModel>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.BrandId, e.DivarId }).IsUnique();
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.PersianName).IsRequired();
                entity.HasOne(m => m.Brand)
                    .WithMany()
                    .HasForeignKey(m => m.BrandId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // تنظیمات جدول CarAd
            modelBuilder.Entity<CarAd>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Token).IsUnique();
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.Year).IsRequired();
                entity.Property(e => e.Kilometer).IsRequired();
                entity.Property(e => e.Token).IsRequired();
                entity.Property(e => e.Link).IsRequired();

                entity.HasOne(a => a.City)
                    .WithMany()
                    .HasForeignKey(a => a.CityId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.District)
                    .WithMany()
                    .HasForeignKey(a => a.DistrictId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(a => a.CarModel)
                    .WithMany()
                    .HasForeignKey(a => a.CarModelId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
} 