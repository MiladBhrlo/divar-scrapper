using System;
using System.Threading.Tasks;
using DivarScraper.Core.Settings;
using DivarScraper.Core.Data;
using DivarScraper.Core.Services;

namespace DivarScraper.Trainer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine("Starting Model Trainer...");

                var settings = new AppSettings();
                var repository = new SqliteCarAdRepository(settings);
                var predictor = new CarPricePredictor();
                var trainer = new ModelTrainer(settings, repository, predictor);

                await trainer.TrainAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
