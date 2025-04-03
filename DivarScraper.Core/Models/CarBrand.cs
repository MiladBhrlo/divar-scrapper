using System.ComponentModel.DataAnnotations;

namespace DivarScraper.Core.Models
{
    public class CarBrand
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PersianName { get; set; } = string.Empty;
        public string DivarId { get; set; } = string.Empty;
        public List<CarModel> Models { get; set; } = new List<CarModel>();
    }
}