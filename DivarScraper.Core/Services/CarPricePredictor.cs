using System;
using System.Collections.Generic;
using System.IO;
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

        public async Task TrainAsync(IEnumerable<CarTrainingData> trainingData)
        {
            var dataView = _mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = _mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "Price")
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CityEncoded", inputColumnName: "City"))
                .Append(_mlContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DistrictEncoded", inputColumnName: "District"))
                .Append(_mlContext.Transforms.Concatenate("Features", "Year", "Kilometer", "CityEncoded", "DistrictEncoded"))
                .Append(_mlContext.Regression.Trainers.Sdca());

            _model = await Task.Run(() => pipeline.Fit(dataView));
            _predictionEngine = _mlContext.Model.CreatePredictionEngine<CarTrainingData, CarPricePrediction>(_model);
        }

        public async Task<float> PredictPriceAsync(CarTrainingData carData)
        {
            try
            {
                if (_model == null)
                {
                    await LoadModelAsync("model.zip");
                }

                var prediction = _predictionEngine.Predict(carData);
                return prediction.PredictedPrice;
            }
            catch (Exception ex)
            {
                // در صورت خطا، یک قیمت پیش‌فرض برگردانید
                return 0;
            }
        }

        public async Task SaveModelAsync(string path)
        {
            if (_model != null)
            {
                await Task.Run(() => _mlContext.Model.Save(_model, null, path));
            }
            else
            {
                throw new InvalidOperationException("مدل هنوز آموزش ندیده است.");
            }
        }

        public async Task LoadModelAsync(string path)
        {
            if (File.Exists(path))
            {
                _model = await Task.Run(() => _mlContext.Model.Load(path, out _));
                _predictionEngine = _mlContext.Model.CreatePredictionEngine<CarTrainingData, CarPricePrediction>(_model);
            }
            else
            {
                throw new FileNotFoundException("مدل آموزش دیده یافت نشد.");
            }
        }
    }

    public class CarPricePrediction
    {
        [ColumnName("Score")]
        public float PredictedPrice { get; set; }
    }
} 