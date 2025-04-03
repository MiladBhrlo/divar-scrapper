using System.Collections.Generic;
using System.Threading.Tasks;
using DivarScraper.Core.Models;

namespace DivarScraper.Core.Data
{
    public interface ICarAdRepository
    {
        Task SaveAdsAsync(List<CarAd> ads);
        Task<List<CarAd>> GetAllAdsAsync();
        Task<City> GetOrCreateCityAsync(City city);
        Task<District> GetOrCreateDistrictAsync(District district);
        Task<CarModel> GetOrCreateCarModelAsync(CarModel model);
    }
} 