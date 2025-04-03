using System;
using System.ComponentModel.DataAnnotations;

namespace DivarScraper.Core.Models
{
    public class District
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PersianName { get; set; } = string.Empty;
        public string DivarId { get; set; } = string.Empty;
        public int CityId { get; set; }
        public City? City { get; set; }
    }
} 