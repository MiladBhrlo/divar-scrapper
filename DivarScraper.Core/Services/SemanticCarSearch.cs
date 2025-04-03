using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivarScraper.Core.Data;
using DivarScraper.Core.Models;
using DivarScraper.Core.Settings;

namespace DivarScraper.Core.Services
{
    public class SemanticCarSearch
    {
        private readonly ICarAdRepository _repository;
        private readonly DeepSeekClient _deepSeekClient;
        private readonly AppSettings _settings;

        public SemanticCarSearch(ICarAdRepository repository, AppSettings settings)
        {
            _repository = repository;
            _settings = settings;
            _deepSeekClient = new DeepSeekClient(
                settings.DeepSeekKey,
                settings.DeepSeekModel,
                settings.DeepSeekEndpoint
            );
        }

        public class SearchResult
        {
            public CarAd Ad { get; set; }
            public float Relevance { get; set; }
            public string MatchReason { get; set; }
        }

        public async Task<List<SearchResult>> SearchAsync(string query)
        {
            try
            {
                var allAds = await _repository.GetAllAsync();
                var results = new List<SearchResult>();

                foreach (var ad in allAds)
                {
                    var prompt = $@"
                        تحلیل تطابق آگهی خودرو با جستجوی کاربر:
                        
                        جستجوی کاربر: {query}
                        
                        مشخصات آگهی:
                        - عنوان: {ad.Title}
                        - قیمت: {ad.Price:N0}
                        - سال ساخت: {ad.Year}
                        - کیلومتر: {ad.Kilometer:N0}
                        - شهر: {ad.City?.Name}
                        - منطقه: {ad.District?.Name}
                        
                        لطفاً تحلیل زیر را ارائه دهید:
                        1. میزان تطابق آگهی با جستجو (عدد بین 0 تا 1)
                        2. دلایل تطابق یا عدم تطابق
                    ";

                    var analysis = await _deepSeekClient.GetCompletionAsync(prompt);
                    var lines = analysis.Split('\n');
                    var relevanceLine = lines.FirstOrDefault(l => l.Contains("میزان تطابق"));
                    var reasonLine = lines.FirstOrDefault(l => l.Contains("دلایل"));

                    if (float.TryParse(relevanceLine?.Split(':').Last().Trim(), out var relevance))
                    {
                        results.Add(new SearchResult
                        {
                            Ad = ad,
                            Relevance = relevance,
                            MatchReason = reasonLine?.Split(':').Last().Trim() ?? "بدون توضیح"
                        });
                    }
                }

                return results
                    .Where(r => r.Relevance > 0.5)
                    .OrderByDescending(r => r.Relevance)
                    .ToList();
            }
            catch (Exception ex)
            {
                return new List<SearchResult>();
            }
        }

        public async Task<string> GetInsightsAsync(string query)
        {
            try
            {
                var prompt = $@"
                    تحلیل بازار خودرو بر اساس جستجوی کاربر:
                    
                    جستجوی کاربر: {query}
                    
                    لطفاً تحلیل زیر را ارائه دهید:
                    1. وضعیت کلی بازار خودرو
                    2. نکات مهم برای خریدار
                    3. روند قیمت‌ها
                    4. پیشنهادات و توصیه‌ها
                ";

                return await _deepSeekClient.GetCompletionAsync(prompt);
            }
            catch (Exception ex)
            {
                return "خطا در دریافت تحلیل بازار. لطفاً دوباره تلاش کنید.";
            }
        }
    }
} 