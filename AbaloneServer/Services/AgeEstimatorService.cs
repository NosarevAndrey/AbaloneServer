using AbaloneServer.Models;
using AbaloneServer.Resources.Model;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace AbaloneServer.Services
{
    public class AgeEstimatorService
    {
        private readonly string _modelFilePath;
        private readonly string _datasetFilePath;
        private static RegressionModel _model;
        private static Scaler _scaler;
        public AgeEstimatorService()
        {
            _datasetFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Dataset", "abalone_prep.csv");
            _modelFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Model", "abalone_model.dat");

            Directory.CreateDirectory(Path.GetDirectoryName(_modelFilePath));

            InitModel();

        }
        void InitModel()
        {
            _model = new RegressionModel(_modelFilePath);
            if (_model.Load()) {
                LoadScaler();
                return;
            }

            CreateAndTrainModel();
            SaveModel();
        }
        private void LoadScaler() {
            _scaler = new Scaler();

            var lines = File.ReadLines(_datasetFilePath).Skip(1); // Skip header line
            var data = new List<double[]>();
            foreach (var line in lines)
            {
                var parts = line.Split(',');

                var features = new double[]
                {
                    parts[0] == "M" ? 1.0 : (parts[0] == "F" ? 0.0 : -1.0), // Encoding 'Sex'
                    double.Parse(parts[1], CultureInfo.InvariantCulture), // Length
                    double.Parse(parts[2], CultureInfo.InvariantCulture), // Diameter
                    double.Parse(parts[3], CultureInfo.InvariantCulture), // Height
                    double.Parse(parts[4], CultureInfo.InvariantCulture), // WholeWeight
                    double.Parse(parts[5], CultureInfo.InvariantCulture), // ShuckedWeight
                    double.Parse(parts[6], CultureInfo.InvariantCulture), // VisceraWeight
                    double.Parse(parts[7], CultureInfo.InvariantCulture)  // ShellWeight
                };

                data.Add(features);
            }
            _scaler.Fit(data);
        }
        public float EstimateAge(AbaloneSubmissionViewModel model)
        {
            double[] scaledInput = new double[]
            {
                model.Sex == AbaloneSex.Male ? 1f : (model.Sex == AbaloneSex.Female ? 0f : -1f),
                model.Length / 200.0,
                model.Diameter / 200.0,
                model.Height / 200.0,
                model.WholeWeight / 200.0,
                model.ShuckedWeight / 200.0,
                model.VisceraWeight / 200.0,
                model.ShellWeight / 200.0
            };
            double[] scaledData = _scaler.Transform(scaledInput);
            var predictionInput = new AbaloneData
            {
                Sex = (float)scaledData[0],      
                Length = (float)scaledData[1], 
                Diameter = (float)scaledData[2], 
                Height = (float)scaledData[3], 
                WholeWeight = (float)scaledData[4], 
                ShuckedWeight = (float)scaledData[5], 
                VisceraWeight = (float)scaledData[6], 
                ShellWeight = (float)scaledData[7]
            };
            var predictedRings = _model.Predict(predictionInput);
            
            return predictedRings;
        }
        private void CreateAndTrainModel()
        {
            Console.WriteLine("Training new model using dataset...");

            var (training, validation) = LoadDataset();

            _model.Fit(training, validation);
        }
        private (IEnumerable<AbaloneData> TrainingData, IEnumerable<AbaloneData> ValidationData) LoadDataset()
        {
            _scaler = new Scaler();

            var lines = File.ReadLines(_datasetFilePath).Skip(1); // Skip header line
            int totalLines = lines.Count();
            int[] rings = new int[totalLines];
            var data = new List<double[]>();
            foreach (var (line, index) in lines.Select((line, index) => (line, index)))
            {
                var parts = line.Split(',');

                var features = new double[]
                {
                    parts[0] == "M" ? 1.0 : (parts[0] == "F" ? 0.0 : -1.0), // Encoding 'Sex'
                    double.Parse(parts[1], CultureInfo.InvariantCulture), // Length
                    double.Parse(parts[2], CultureInfo.InvariantCulture), // Diameter
                    double.Parse(parts[3], CultureInfo.InvariantCulture), // Height
                    double.Parse(parts[4], CultureInfo.InvariantCulture), // WholeWeight
                    double.Parse(parts[5], CultureInfo.InvariantCulture), // ShuckedWeight
                    double.Parse(parts[6], CultureInfo.InvariantCulture), // VisceraWeight
                    double.Parse(parts[7], CultureInfo.InvariantCulture)  // ShellWeight
                };
                rings[index] = int.Parse(parts[8]);

                data.Add(features);
            }

            IEnumerable<double[]> scaledData = _scaler.FitTransform(data);

            var abaloneDataList = scaledData.Select((scaledFeatures, index) =>
                new AbaloneData
                {
                    Sex = (float)scaledFeatures[0],
                    Length = (float)scaledFeatures[1],
                    Diameter = (float)scaledFeatures[2],
                    Height = (float)scaledFeatures[3],
                    WholeWeight = (float)scaledFeatures[4],
                    ShuckedWeight = (float)scaledFeatures[5],
                    VisceraWeight = (float)scaledFeatures[6],
                    ShellWeight = (float)scaledFeatures[7],
                    Rings = rings[index] // Use original Rings column as target
                }).ToList();

            var trainingSize = (int)(abaloneDataList.Count * 0.8);

            var trainingData = abaloneDataList.Take(trainingSize);
            var validationData = abaloneDataList.Skip(trainingSize);

            return (trainingData, validationData);

        }
        private void SaveModel()
        {
            Console.WriteLine("Saving trained model...");
            _model.Save();
        }
    }
}
