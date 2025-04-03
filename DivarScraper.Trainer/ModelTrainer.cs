using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivarScraper.Core.Models;
using DivarScraper.Core.Services;
using DivarScraper.Core.Data;
using DivarScraper.Core.Settings;

namespace DivarScraper.Trainer
{
    public class ModelTrainer
    {
        private readonly AppSettings _settings;
        private readonly ICarAdRepository _repository;
        private readonly ICarPricePredictor _predictor;

        public ModelTrainer(AppSettings settings, ICarAdRepository repository, ICarPricePredictor predictor)
        {
            _settings = settings;
            _repository = repository;
            _predictor = predictor;
        }

        public async Task TrainAsync()
        {
            try
            {
                Console.WriteLine("Loading training data...");
                var ads = await _repository.GetAllAdsAsync();
                
                // Filter out ads with invalid data
                var trainingData = ads
                    .Where(ad => ad.Price > 0 && ad.Year > 0 && ad.Kilometer > 0)
                    .Select(ad => new CarTrainingData
                    {
                        Title = ad.Title,
                        Price = ad.Price,
                        Year = ad.Year,
                        Kilometer = ad.Kilometer,
                        City = ad.City?.Name ?? "Unknown",
                        District = ad.District?.Name ?? "Unknown",
                        Token = ad.Token
                    })
                    .ToList();

                Console.WriteLine($"Found {trainingData.Count} valid ads for training");

                if (trainingData.Count == 0)
                {
                    Console.WriteLine("No valid training data found!");
                    return;
                }

                Console.WriteLine("Starting model training...");
                await _predictor.TrainAsync(trainingData);

                Console.WriteLine("Saving trained model...");
                await _predictor.SaveModelAsync(_settings.ModelPath);

                Console.WriteLine("Model training completed successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during training: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
} 