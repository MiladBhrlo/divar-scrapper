using System.Collections.Generic;
using System.Threading.Tasks;
using DivarScraper.Core.Models;

namespace DivarScraper.Core.Services
{
    public interface ICarPricePredictor
    {
        Task<float> PredictPriceAsync(CarTrainingData carData);
        Task TrainAsync(IEnumerable<CarTrainingData> trainingData);
        Task SaveModelAsync(string path);
        Task LoadModelAsync(string path);
    }
} 