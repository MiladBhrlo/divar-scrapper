using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DivarScraper.Core.Models;
using DivarScraper.Core.Settings;

namespace DivarScraper.Core.Data
{
    public class SqliteCarAdRepository : ICarAdRepository
    {
        private readonly AppSettings _settings;
        private readonly DivarDbContext _context;

        public SqliteCarAdRepository(AppSettings settings)
        {
            _settings = settings;
            _context = new DivarDbContext(settings);
            _context.Database.EnsureCreated();
        }

        public async Task SaveAdsAsync(List<CarAd> ads)
        {
            foreach (var ad in ads)
            {
                if (ad.City != null)
                {
                    ad.City = await GetOrCreateCityAsync(ad.City);
                    ad.CityId = ad.City.Id;
                }

                if (ad.District != null)
                {
                    ad.District = await GetOrCreateDistrictAsync(ad.District);
                    ad.DistrictId = ad.District.Id;
                }

                if (ad.CarModel != null)
                {
                    ad.CarModel = await GetOrCreateCarModelAsync(ad.CarModel);
                    ad.CarModelId = ad.CarModel.Id;
                }

                var existingAd = await _context.CarAds.FirstOrDefaultAsync(a => a.Token == ad.Token);
                if (existingAd == null)
                {
                    await _context.CarAds.AddAsync(ad);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<CarAd>> GetAllAdsAsync()
        {
            return await _context.CarAds
                .Include(a => a.City)
                .Include(a => a.District)
                .Include(a => a.CarModel)
                .ThenInclude(m => m.Brand)
                .ToListAsync();
        }

        public async Task<City> GetOrCreateCityAsync(City city)
        {
            var existingCity = await _context.Cities.FirstOrDefaultAsync(c => c.DivarId == city.DivarId);
            if (existingCity != null)
            {
                return existingCity;
            }

            await _context.Cities.AddAsync(city);
            await _context.SaveChangesAsync();
            return city;
        }

        public async Task<District> GetOrCreateDistrictAsync(District district)
        {
            var existingDistrict = await _context.Districts
                .FirstOrDefaultAsync(d => d.CityId == district.CityId && d.DivarId == district.DivarId);

            if (existingDistrict != null)
            {
                return existingDistrict;
            }

            await _context.Districts.AddAsync(district);
            await _context.SaveChangesAsync();
            return district;
        }

        public async Task<CarModel> GetOrCreateCarModelAsync(CarModel model)
        {
            var existingModel = await _context.CarModels
                .FirstOrDefaultAsync(m => m.BrandId == model.BrandId && m.DivarId == model.DivarId);

            if (existingModel != null)
            {
                return existingModel;
            }

            await _context.CarModels.AddAsync(model);
            await _context.SaveChangesAsync();
            return model;
        }
    }
} 