using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.IO;

namespace AbaloneServer.Resources.Model
{
    public class RegressionModel
    {
        private readonly string _modelPath;
        private MLContext _mlContext;
        private ITransformer _trainedModel;

        public RegressionModel(string modelPath)
        {
            _modelPath = modelPath;
            _mlContext = new MLContext();
        }

        public bool Load()
        {
            if (File.Exists(_modelPath))
            {
                _trainedModel = _mlContext.Model.Load(_modelPath, out _);
                Console.WriteLine("Model loaded successfully.");
                return true;
            }
            Console.WriteLine("Model file not found.");
            return false;
        }

        public void Fit(IEnumerable<AbaloneData> trainingData, IEnumerable<AbaloneData> validationData)
        {
            Console.WriteLine("Training model...");

            IDataView trainDataView = _mlContext.Data.LoadFromEnumerable(trainingData);
            IDataView validationDataView = _mlContext.Data.LoadFromEnumerable(validationData);

            var pipeline = _mlContext.Transforms.Concatenate(
                    "Features",
                    new[]
                    {
                        "Length", "Diameter", "Height", "WholeWeight", "ShuckedWeight", "VisceraWeight", "ShellWeight"
                    })
                .Append(_mlContext.Regression.Trainers.Sdca(labelColumnName: "Rings", featureColumnName: "Features"));

            _trainedModel = pipeline.Fit(trainDataView);

            // Evaluate the model
            var predictions = _trainedModel.Transform(validationDataView);
            var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "Rings");

            Console.WriteLine($"R^2: {metrics.RSquared:0.00}");
            Console.WriteLine($"MAE: {metrics.MeanAbsoluteError:0.00}");

        }

        public void Save()
        {
            Console.WriteLine("Saving the trained model...");
            _mlContext.Model.Save(_trainedModel, null, _modelPath);
        }

        public float Predict(AbaloneData input)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<AbaloneData, AbalonePrediction>(_trainedModel);
            var prediction = predictionEngine.Predict(input);
            return prediction.Score;
        }

        public IEnumerable<float> Predict(IEnumerable<AbaloneData> inputs)
        {
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<AbaloneData, AbalonePrediction>(_trainedModel);
            return inputs.Select(input => predictionEngine.Predict(input).Score);
        }

    }
}
