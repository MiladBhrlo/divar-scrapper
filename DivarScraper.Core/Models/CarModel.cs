namespace DivarScraper.Core.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PersianName { get; set; }
        public int BrandId { get; set; }
        public CarBrand Brand { get; set; }
    }
} 