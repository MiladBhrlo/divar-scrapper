using System.Collections.Generic;
using System.Threading.Tasks;
using DivarScraper.Core.Models;

namespace DivarScraper.Core.Services
{
    public interface ICarPricePredictor
    {
        Task TrainAsync(List<CarTrainingData> trainingData);
        Task<float> PredictAsync(CarTrainingData input);
        Task SaveModelAsync(string path);
        Task LoadModelAsync(string path);
    }
} 