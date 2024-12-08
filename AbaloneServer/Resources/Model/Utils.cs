using System;
using System.Collections.Generic;
using System.Linq;

namespace AbaloneServer.Resources.Model
{
    public class Scaler
    {
        private double[] _means;
        private double[] _stdDevs;

        public void Fit(IEnumerable<double[]> data)
        {
            if (data == null || !data.Any())
                throw new ArgumentException("Data cannot be null or empty.");

            int featureCount = data.First().Length;
            _means = new double[featureCount];
            _stdDevs = new double[featureCount];

            for (int i = 0; i < featureCount; i++)
            {
                var featureValues = data.Select(row => row[i]).ToArray();
                _means[i] = featureValues.Average();
                _stdDevs[i] = Math.Sqrt(featureValues.Average(v => Math.Pow(v - _means[i], 2)));
            }
        }

        public double[] Transform(double[] data)
        {
            if (_means == null || _stdDevs == null)
                throw new InvalidOperationException("The scaler must be fitted before transformation.");

            if (data.Length != _means.Length)
                throw new ArgumentException("Input data length must match the number of features.");

            double[] scaledData = new double[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                scaledData[i] = (data[i] - _means[i]) / (_stdDevs[i] == 0 ? 1 : _stdDevs[i]); 
            }
            return scaledData;
        }

        public IEnumerable<double[]> FitTransform(IEnumerable<double[]> data)
        {
            Fit(data);
            return data.Select(Transform);
        }
     
        public double[] Means => _means;
        public double[] StandardDeviations => _stdDevs;
    }

    public class AbaloneData
    {
        public float Sex { get; set; }
        public float Length { get; set; }
        public float Diameter { get; set; }
        public float Height { get; set; }
        public float WholeWeight { get; set; }
        public float ShuckedWeight { get; set; }
        public float VisceraWeight { get; set; }
        public float ShellWeight { get; set; }
        public float Rings { get; set; } // This is the label (target).
    }

    public class AbalonePrediction
    {
        public float Score { get; set; }
    }
}
