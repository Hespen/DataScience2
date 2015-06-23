using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forecasting
{
    class SES
    {
        private DataTable _dataSet;
        private double _standardError = -1;
        private const int PredictionPeriod = 12;
        private double _alpha = 0.5;
        private double _optimalAlpha;

        public SES(DataTable dataSet)
        {
            _dataSet = dataSet;
        }

        public void Execute()
        {
            CalculateAverage();
            for (_alpha = 0; _alpha < 1; _alpha += 0.02f)
            {
                CalculateLevelEstimate(_alpha);
                CalculateSSE(_alpha);
            }
            CalculateLevelEstimate(_optimalAlpha);

            Forecast();
        }

        private void CalculateSSE(double alpha)
        {
            var sse = _dataSet.AsEnumerable().Sum(x => x.Field<double>("Squared Error"));
            var standardError = Math.Sqrt(sse/(_dataSet.Rows.Count - 2));
            if (_standardError == -1 || standardError < _standardError)
            {
                _standardError = standardError;
                _optimalAlpha = alpha;
            }
        }

        private void Forecast()
        {
            var lastLevelEstimate = _dataSet.Rows[_dataSet.Rows.Count - 1]["Level Estimate"];
            var lastT = Convert.ToInt32(_dataSet.Rows[_dataSet.Rows.Count - 1]["t"]);
            for (int i = 0; i < PredictionPeriod; i++)
            {
                var row = _dataSet.NewRow();
                row["t"] = lastT + i;
                row["Demand"] = lastLevelEstimate;
                _dataSet.Rows.Add(row);
            }
        }

        private void CalculateLevelEstimate(double alpha)
        {
            for (int i = 1; i < _dataSet.Rows.Count; i++)
            {

                var row = _dataSet.Rows[i];
                var oneStepForecast = Convert.ToDouble(_dataSet.Rows[i - 1]["Level Estimate"]);
                var foreCastError = Convert.ToDouble(_dataSet.Rows[i]["Demand"]) - oneStepForecast;
                var levelEstimate = oneStepForecast + alpha*foreCastError;

                row["One-step Forecast"] = oneStepForecast;
                row["Forecast Error"] = foreCastError;
                row["Level Estimate"] = levelEstimate;
                row["Squared Error"] = Math.Pow(foreCastError, 2);
            }
        }

        private void CalculateAverage()
        {
            int total = 0;
            for (int i = 1; i <= PredictionPeriod; i++)
            {
                total += Convert.ToInt32(_dataSet.Rows[i]["Demand"]);
            }
            _dataSet.Rows[0]["Level Estimate"]=total/PredictionPeriod;
        }
    }
}
