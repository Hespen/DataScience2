using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecasting
{
    class DES
    {
        private readonly DataTable _dataSet;
        private const int PredictionPeriod = 12;
        private double _optimalAlpha,_optimalGamma;
        private double _standardError = -1;
        private double _level = 155.88;
        private double _trend = 0.8369;

        public DES(DataTable dataSet)
        {
            _dataSet = dataSet;
            var firstRow = _dataSet.Rows[0];
            firstRow["Level Estimate"] = _level;
            firstRow["Trend"] = _trend;
        }

        public void Execute()
        {
           for (float gamma = 0.01f; gamma < 1; gamma += 0.01f)
            {
                for (float alpha = 0.01f; alpha < 1; alpha += 0.01f)
                {
                    CalculateLevelEstimate(alpha, gamma);
                    CalculateSSE(alpha, gamma);
                }
            }

            CalculateLevelEstimate(_optimalAlpha,_optimalGamma);
            Forecast();
        }

        private void CalculateSSE(double alpha, double gamma)
        {
            var sse = _dataSet.AsEnumerable().Sum(x => x.Field<double>("Squared Error"));
            var standardError = Math.Sqrt(sse / (_dataSet.Rows.Count - 3));
            if ((_standardError == -1 || standardError < _standardError) && standardError>0)
            {
                _standardError = standardError;
                _optimalAlpha = alpha;
                _optimalGamma = gamma;
            }
        }

        private void Forecast()
        {
            var lastRow = _dataSet.Rows[_dataSet.Rows.Count - 1];
            var lastT = (double)lastRow["t"];
            var lastLevelEstimate = (double) lastRow["Level Estimate"];
            var lastTrend = (double) lastRow["Trend"];
            for (int i = 1; i <= PredictionPeriod; i++)
            {
                var newRow = _dataSet.NewRow();
                newRow["t"] = lastT + i;
                newRow["Forecast"] = lastLevelEstimate + i*lastTrend;
                _dataSet.Rows.Add(newRow);
            }
        }

        private void CalculateLevelEstimate(double alpha, double gamma)
        {
            for (int i = 1; i < _dataSet.Rows.Count; i++)
            {
                var previousRow = _dataSet.Rows[i - 1];
                var row = _dataSet.Rows[i];

                double previousLevelEstimate = (double) previousRow["Level Estimate"];
                double previousTrend = (double) previousRow["Trend"];
                double oneStepForecast = previousLevelEstimate + previousTrend;

                double foreCastError =  (double) row["Demand"]-oneStepForecast;
                double levelEstimate = previousLevelEstimate + previousTrend +
                                       alpha*foreCastError;

                double trend = previousTrend + gamma*alpha*foreCastError;
                
                row["Level Estimate"] = levelEstimate;
                row["Trend"] = trend;
                row["Forecast"] = oneStepForecast;
                row["Forecast Error"] = foreCastError;
                row["Squared Error"] = Math.Pow(foreCastError,2);
            }
        }
    }
}
