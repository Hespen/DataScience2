using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clustering.Reader;
using Clustering.Utils;

namespace Clustering
{
    class Program
    {
        private static ClusterHandler _clusterHandler;
        private static int _k = 4;

        static void Main(string[] args)
        {
            var reader = new FileReader();
            var pivot = reader.ReadDataFromFile(Constants.Pivot);



            _clusterHandler = new ClusterHandler(_k);
            var clusterLocations = _clusterHandler.CreateClusters();

            var calculator = new DistanceCalculator();


            var distancesTable = calculator.CalculateDistanceBetween(pivot, clusterLocations);

            var silhouette = new Silhouette();

            

            var totalDistance = 0.0f;
            while(true){
                
                clusterLocations = _clusterHandler.UpdateCentroids(pivot, distancesTable, _k);

                distancesTable = calculator.CalculateDistanceBetween(pivot, clusterLocations);

                if (totalDistance == calculator.CalculateTotalDistance(distancesTable, _k))
                {
                    break;
                }
                else
                {
                    totalDistance = calculator.CalculateTotalDistance(distancesTable, _k);
                }
            }

            var customerDistances = silhouette.CalculateCustomerDistances(pivot);
            silhouette.CalculateAverageClusterDistance(customerDistances, distancesTable, _k);
            silhouette.CalculateSilhouette();
            Console.ReadKey();
        }
    }
}
