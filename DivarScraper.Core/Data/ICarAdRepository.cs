using System.Collections.Generic;
using System.Threading.Tasks;
using DivarScraper.Core.Models;

namespace DivarScraper.Core.Data
{
    public interface ICarAdRepository
    {
        Task<List<CarAd>> GetAllAsync();
        Task<List<CarAd>> GetAllAdsAsync();
        Task<CarAd> GetByIdAsync(int id);
        Task<CarAd> GetByTokenAsync(string token);
        Task AddAsync(CarAd carAd);
        Task UpdateAsync(CarAd carAd);
        Task DeleteAsync(int id);
        Task SaveAdsAsync(IEnumerable<CarAd> carAds);
    }
}