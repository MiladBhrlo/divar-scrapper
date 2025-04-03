using System;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using DivarScraper.Core.Models;
using DivarScraper.Core.Settings;

namespace DivarScraper.Core.Services
{
    public class PriceAdvice
    {
        public long PredictedPrice { get; set; }
        public string Explanation { get; set; }
        public double Confidence { get; set; }
        public List<string> Factors { get; set; }
    }

    public class SmartPriceAdvisor
    {
        private readonly IKernel _kernel;
        private readonly ICarPricePredictor _mlPredictor;
        private readonly AppSettings _settings;

        public SmartPriceAdvisor(AppSettings settings, ICarPricePredictor mlPredictor)
        {
            _settings = settings;
            _mlPredictor = mlPredictor;

            var builder = new KernelBuilder();
            builder.WithDeepSeekChatCompletionService(
                _settings.DeepSeekModel,
                _settings.DeepSeekKey,
                _settings.DeepSeekEndpoint
            );
            _kernel = builder.Build();
        }

        public async Task<PriceAdvice> GetPriceAdviceAsync(CarTrainingData carData)
        {
            try
            {
                var mlPrice = await _mlPredictor.PredictAsync(carData);

                var prompt = @$"
                    با توجه به مشخصات خودرو و شرایط بازار، تحلیل کنید که آیا قیمت {mlPrice:N0} تومان منصفانه است:
                    
                    شهر: {carData.City}
                    منطقه: {carData.District}
                    سال ساخت: {carData.Year}
                    کارکرد: {carData.Kilometer:N0} کیلومتر

                    لطفا پاسخ را در قالب JSON با فرمت زیر برگردانید:
                    {{
                        ""explanation"": ""توضیح کامل درباره قیمت"",
                        ""confidence"": عدد بین 0 تا 1,
                        ""factors"": [""فاکتور 1"", ""فاکتور 2"", ...]
                    }}";

                var function = _kernel.CreateSemanticFunction(prompt);
                var result = await function.InvokeAsync();
                
                var analysis = System.Text.Json.JsonSerializer.Deserialize<dynamic>(result.ToString());

                return new PriceAdvice
                {
                    PredictedPrice = mlPrice,
                    Explanation = analysis.GetProperty("explanation").GetString(),
                    Confidence = analysis.GetProperty("confidence").GetDouble(),
                    Factors = analysis.GetProperty("factors").EnumerateArray()
                        .Select(f => f.GetString())
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                return new PriceAdvice
                {
                    PredictedPrice = await _mlPredictor.PredictAsync(carData),
                    Explanation = "متأسفانه در تحلیل هوشمند قیمت خطایی رخ داد.",
                    Confidence = 0.5,
                    Factors = new List<string>()
                };
            }
        }
    }
} 