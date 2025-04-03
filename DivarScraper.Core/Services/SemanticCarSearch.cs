using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using DivarScraper.Core.Models;
using DivarScraper.Core.Settings;
using DivarScraper.Core.Data;

namespace DivarScraper.Core.Services
{
    public class SearchResult
    {
        public CarAd Ad { get; set; }
        public double Relevance { get; set; }
        public string MatchReason { get; set; }
    }

    public class SemanticCarSearch
    {
        private readonly IKernel _kernel;
        private readonly ICarAdRepository _repository;
        private readonly AppSettings _settings;

        public SemanticCarSearch(AppSettings settings, ICarAdRepository repository)
        {
            _settings = settings;
            _repository = repository;

            var builder = new KernelBuilder();
            builder.WithDeepSeekChatCompletionService(
                _settings.DeepSeekModel,
                _settings.DeepSeekKey,
                _settings.DeepSeekEndpoint
            );
            _kernel = builder.Build();
        }

        public async Task<List<SearchResult>> SearchAsync(string userQuery)
        {
            var ads = await _repository.GetAllAdsAsync();
            var results = new List<SearchResult>();

            var prompt = @$"
                با توجه به جستجوی کاربر: ""{userQuery}""
                و مشخصات خودرو زیر، میزان تطابق را مشخص کنید:

                عنوان: {{{{$title}}}}
                قیمت: {{{{$price}}}} تومان
                سال: {{{{$year}}}}
                کارکرد: {{{{$kilometer}}}} کیلومتر
                شهر: {{{{$city}}}}
                منطقه: {{{{$district}}}}

                لطفا پاسخ را در قالب JSON با فرمت زیر برگردانید:
                {{
                    ""relevance"": عدد بین 0 تا 1,
                    ""reason"": ""دلیل تطابق یا عدم تطابق""
                }}";

            var function = _kernel.CreateSemanticFunction(prompt);

            foreach (var ad in ads)
            {
                try
                {
                    var context = new Dictionary<string, string>
                    {
                        { "title", ad.Title },
                        { "price", ad.Price.ToString("N0") },
                        { "year", ad.Year.ToString() },
                        { "kilometer", ad.Kilometer.ToString("N0") },
                        { "city", ad.City?.Name ?? "" },
                        { "district", ad.District?.Name ?? "" }
                    };

                    var result = await function.InvokeAsync(context);
                    var analysis = System.Text.Json.JsonSerializer.Deserialize<dynamic>(result.ToString());

                    var relevance = analysis.GetProperty("relevance").GetDouble();
                    if (relevance > 0.5) // فقط نتایج مرتبط را برمی‌گرداند
                    {
                        results.Add(new SearchResult
                        {
                            Ad = ad,
                            Relevance = relevance,
                            MatchReason = analysis.GetProperty("reason").GetString()
                        });
                    }
                }
                catch (Exception)
                {
                    // در صورت خطا، این آگهی را نادیده می‌گیریم
                    continue;
                }
            }

            return results.OrderByDescending(r => r.Relevance).ToList();
        }

        public async Task<string> GetInsightsAsync(string userQuery)
        {
            var prompt = @$"
                با توجه به جستجوی کاربر: ""{userQuery}""
                لطفا یک تحلیل کلی از بازار خودرو ارائه دهید.
                نکات مهم برای خرید، روندهای قیمت و پیشنهادات را ذکر کنید.";

            var function = _kernel.CreateSemanticFunction(prompt);
            var result = await function.InvokeAsync();
            return result.ToString();
        }
    }
} 