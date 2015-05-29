using System;
using System.Collections.Generic;
using System.Data;
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


            var distancesTable = calculator.CalculateDistanceBetween(pivot, clusterLocations, _k);

            var silhouette = new Silhouette();

            

            var totalDistance = 0.0f;
            while(true){
                
                clusterLocations = _clusterHandler.UpdateCentroids(pivot, distancesTable, _k);

                distancesTable = calculator.CalculateDistanceBetween(pivot, clusterLocations,_k);

                if (totalDistance == calculator.CalculateTotalDistance(distancesTable, _k))
                {
                    break;
                }
                else
                {
                    totalDistance = calculator.CalculateTotalDistance(distancesTable, _k);
                }
            }

            var SSE = new SSE();
            SSE.CalculateDistances(pivot,clusterLocations,distancesTable,_k);

            var customerDistances = silhouette.CalculateCustomerDistances(pivot);
            silhouette.CalculateAverageClusterDistance(customerDistances, distancesTable, _k);
            silhouette.CalculateSilhouette();

            var topDeals = new TopDeals();
            var topDealsList = new List<DataTable>();
            for (var i = 1; i <= _k; i++)
            {
                topDealsList.Add( topDeals.CalculateTopDeals(pivot, distancesTable, i));
            }
            Console.ReadKey();
        }
    }
}
