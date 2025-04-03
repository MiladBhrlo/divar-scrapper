using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using DivarScraper.Core.Models;

namespace DivarScraper.Core.Services
{
    public class CarPricePredictor : ICarPricePredictor
    {
        private readonly MLContext _mlContext;
        private ITransformer _model;
        private PredictionEngine<CarTrainingData, CarPricePrediction> _predictionEngine;

        public CarPricePredictor()
        {
            _mlContext = new MLContext();
        }

        public async Task TrainAsync(List<CarTrainingData> trainingData)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Price")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CityEncoded", inputColumnName: "City"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DistrictEncoded", inputColumnName: "District"))
                .Append(_mlContext.Transforms.Concatenate("Features", "Year", "Kilometer", "CityEncoded", "DistrictEncoded"))
                .Append(_mlContext.Regression.Trainers.Sdca());

            _model = pipeline.Fit(dataView);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<CarTrainingData, CarPricePrediction>(_model);
        }

        public async Task<float> PredictAsync(CarTrainingData input)
        {
            if (_predictionEngine == null)
            {
                throw new System.InvalidOperationException("Model has not been trained yet.");
            }

            var prediction = _predictionEngine.Predict(input);
            return prediction.PredictedPrice;
        }

        public async Task SaveModelAsync(string path)
        {
            if (_model == null)
            {
                throw new System.InvalidOperationException("Model has not been trained yet.");
            }

            _mlContext.Model.Save(_model, dataView.Schema, path);
        }

        public async Task LoadModelAsync(string path)
        {
            _model = _mlContext.Model.Load(path, out var schema);
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<CarTrainingData, CarPricePrediction>(_model);
        }
    }

    public class CarPricePrediction
    {
        [ColumnName("Score")]
        public float PredictedPrice { get; set; }
    }
} 