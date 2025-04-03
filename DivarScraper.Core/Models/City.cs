using System.ComponentModel.DataAnnotations;

namespace DivarScraper.Core.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PersianName { get; set; } = string.Empty;
        public string DivarId { get; set; } = string.Empty;
        public List<District> Districts { get; set; } = new List<District>();
    }
} 