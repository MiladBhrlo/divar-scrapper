using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DivarScraper.Core.Models;
using DivarScraper.Core.Settings;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace DivarScraper.Core.Data
{
    public class PostgresCarAdRepository : ICarAdRepository
    {
        private readonly AppSettings _settings;
        private readonly DivarDbContext _context;

        public PostgresCarAdRepository(AppSettings settings)
        {
            _settings = settings;
            _context = new DivarDbContext(settings);
            _context.Database.EnsureCreated();
        }

        public async Task<List<CarAd>> GetAllAsync()
        {
            return await _context.CarAds
                .Include(c => c.City)
                .Include(c => c.District)
                .Include(c => c.CarModel)
                .ThenInclude(m => m.Brand)
                .ToListAsync();
        }

        public async Task<List<CarAd>> GetAllAdsAsync()
        {
            return await _context.CarAds
                .Include(c => c.City)
                .Include(c => c.District)
                .Include(c => c.CarModel)
                .ThenInclude(m => m.Brand)
                .ToListAsync();
        }

        public async Task<CarAd> GetByIdAsync(int id)
        {
            return await _context.CarAds
                .Include(c => c.City)
                .Include(c => c.District)
                .Include(c => c.CarModel)
                .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CarAd> GetByTokenAsync(string token)
        {
            return await _context.CarAds
                .Include(c => c.City)
                .Include(c => c.District)
                .Include(c => c.CarModel)
                .ThenInclude(m => m.Brand)
                .FirstOrDefaultAsync(c => c.Token == token);
        }

        public async Task AddAsync(CarAd carAd)
        {
            if (carAd.City != null)
            {
                var existingCity = await _context.Cities
                    .FirstOrDefaultAsync(c => c.DivarId == carAd.City.DivarId);
                if (existingCity != null)
                {
                    carAd.CityId = existingCity.Id;
                    carAd.City = existingCity;
                }
            }

            if (carAd.District != null)
            {
                var existingDistrict = await _context.Districts
                    .FirstOrDefaultAsync(d => d.DivarId == carAd.District.DivarId);
                if (existingDistrict != null)
                {
                    carAd.DistrictId = existingDistrict.Id;
                    carAd.District = existingDistrict;
                }
            }

            if (carAd.CarModel != null)
            {
                if (carAd.CarModel.Brand != null)
                {
                    var existingBrand = await _context.CarBrands
                        .FirstOrDefaultAsync(b => b.DivarId == carAd.CarModel.Brand.DivarId);
                    if (existingBrand != null)
                    {
                        carAd.CarModel.BrandId = existingBrand.Id;
                        carAd.CarModel.Brand = existingBrand;
                    }
                }

                var existingModel = await _context.CarModels
                    .FirstOrDefaultAsync(m => m.DivarId == carAd.CarModel.DivarId);
                if (existingModel != null)
                {
                    carAd.CarModelId = existingModel.Id;
                    carAd.CarModel = existingModel;
                }
            }

            await _context.CarAds.AddAsync(carAd);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CarAd carAd)
        {
            _context.Entry(carAd).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var carAd = await _context.CarAds.FindAsync(id);
            if (carAd != null)
            {
                _context.CarAds.Remove(carAd);
                await _context.SaveChangesAsync();
            }
        }

        public async Task SaveAdsAsync(IEnumerable<CarAd> carAds)
        {
            foreach (var carAd in carAds)
            {
                var existingAd = await _context.CarAds
                    .FirstOrDefaultAsync(c => c.Token == carAd.Token);

                if (existingAd == null)
                {
                    await AddAsync(carAd);
                }
                else
                {
                    existingAd.Price = carAd.Price;
                    existingAd.Kilometer = carAd.Kilometer;
                    existingAd.Year = carAd.Year;
                    existingAd.UpdatedAt = DateTime.UtcNow;
                    await UpdateAsync(existingAd);
                }
            }
        }
    }
} 