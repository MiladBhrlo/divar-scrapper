using System.ComponentModel.DataAnnotations;

namespace DivarScraper.Core.Settings
{
    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }

    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new ConnectionStrings();
        public string DbHost { get; set; } = "localhost";
        public string DbName { get; set; } = "divarscraper";
        public string DbUser { get; set; } = "postgres";
        public string DbPassword { get; set; } = "postgres";
        public string JsonDataPath { get; set; } = "car_ads.json";
        public int MaxScrolls { get; set; } = 10;
        public string ModelPath { get; set; } = "car_price_model.zip";
        public string DivarBaseUrl { get; set; } = "https://divar.ir";
        public List<string> CityIds { get; set; } = new List<string>();
        
        // Semantic Kernel Settings
        public string DeepSeekKey { get; set; } = string.Empty;
        public string DeepSeekModel { get; set; } = "deepseek-chat";
        public string DeepSeekEndpoint { get; set; } = "https://api.deepseek.com/v1/";
    }
} 