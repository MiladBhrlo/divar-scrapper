using System;
using System.Threading.Tasks;
using DivarScraper.Core.Models;
using DivarScraper.Core.Settings;

namespace DivarScraper.Core.Services
{
    public class SmartPriceAdvisor
    {
        private readonly ICarPricePredictor _predictor;
        private readonly DeepSeekClient _deepSeekClient;
        private readonly AppSettings _settings;

        public SmartPriceAdvisor(ICarPricePredictor predictor, AppSettings settings)
        {
            _predictor = predictor;
            _settings = settings;
            _deepSeekClient = new DeepSeekClient(
                settings.DeepSeekKey,
                settings.DeepSeekModel,
                settings.DeepSeekEndpoint
            );
        }

        public class PriceAdvice
        {
            public float PredictedPrice { get; set; }
            public string Explanation { get; set; } = string.Empty;
            public float Confidence { get; set; }
            public string[] Factors { get; set; } = Array.Empty<string>();
        }

        public async Task<PriceAdvice> GetPriceAdviceAsync(CarTrainingData carData)
        {
            try
            {
                // پیش‌بینی قیمت با ML.NET
                var predictedPrice = await _predictor.PredictPriceAsync(carData);

                // تحلیل هوشمند با DeepSeek
                var prompt = $@"
                    تحلیل قیمت خودرو با مشخصات زیر:
                    - سال ساخت: {carData.Year}
                    - کیلومتر: {carData.Kilometer}
                    - شهر: {carData.City}
                    - منطقه: {carData.District}
                    - قیمت پیش‌بینی شده: {predictedPrice}

                    لطفاً تحلیل زیر را ارائه دهید:
                    1. آیا قیمت پیش‌بینی شده منصفانه است؟
                    2. فاکتورهای مؤثر بر قیمت را ذکر کنید
                    3. پیشنهادات برای قیمت‌گذاری
                ";

                var analysis = await _deepSeekClient.GetCompletionAsync(prompt);

                return new PriceAdvice
                {
                    PredictedPrice = predictedPrice,
                    Explanation = analysis,
                    Confidence = 0.85f, // این مقدار می‌تواند بر اساس تحلیل DeepSeek محاسبه شود
                    Factors = new[] { "سال ساخت", "کیلومتر", "موقعیت جغرافیایی" }
                };
            }
            catch
            {
                // در صورت خطا، یک پاسخ پیش‌فرض برگردانید
                return new PriceAdvice
                {
                    PredictedPrice = 0,
                    Explanation = "خطا در تحلیل قیمت. لطفاً دوباره تلاش کنید.",
                    Confidence = 0,
                    Factors = Array.Empty<string>()
                };
            }
        }
    }
} 