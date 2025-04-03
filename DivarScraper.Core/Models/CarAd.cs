using System.Text.Json.Serialization;

namespace DivarScraper.Core.Models
{
    public class CarAd
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Year { get; set; }
        public string Kilometer { get; set; }
        public string Token { get; set; }
        public string Link { get; set; }
        public string ImageUrl { get; set; }

        public int? CityId { get; set; }
        public City City { get; set; }

        public int? DistrictId { get; set; }
        public District District { get; set; }

        public int? CarModelId { get; set; }
        public CarModel CarModel { get; set; }
    }
} 