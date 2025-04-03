using System.ComponentModel.DataAnnotations;

namespace DivarScraper.Core.Models
{
    public class CarAd
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public long Price { get; set; }
        public int Year { get; set; }
        public int Kilometer { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int CityId { get; set; }
        public City? City { get; set; }
        public int DistrictId { get; set; }
        public District? District { get; set; }
        public int CarModelId { get; set; }
        public CarModel? CarModel { get; set; }
    }
}