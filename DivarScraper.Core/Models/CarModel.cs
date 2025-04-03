using System;
using System.ComponentModel.DataAnnotations;

namespace DivarScraper.Core.Models
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PersianName { get; set; } = string.Empty;
        public string DivarId { get; set; } = string.Empty;
        public int BrandId { get; set; }
        public CarBrand? Brand { get; set; }
    }
} 