using System;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Clustering
{
    internal class DistanceCalculator
    {
        /// <summary>
        ///     Calculate the distance between every customer and the cluster locations.
        /// </summary>
        /// <param name="pivot">DataTable containing the purchases for every customer</param>
        /// <param name="clusterLocations">The locations of the cluster</param>
        public DataTable CalculateDistanceBetween(DataTable pivot, DataTable clusterLocations, int clusters)
        {
            var distancesTable = CreateDataTable(clusters);

            //Loop through all customers
            for (var i = 1; i < pivot.Columns.Count; i++)
            {
                //Retrieve the Column with the purchases made by this customer
                var purchases = pivot.AsEnumerable().Select(s => s.Field<String>(pivot.Columns[i])).ToList();

                var newDistancerow = distancesTable.NewRow();
                newDistancerow[0] = pivot.Columns[i];

                double smallestClusterDistance = 100;
                var assignedCluster = 0;

                // Loop through all clusters, j is the current cluster
                for (var j = 0; j < clusters; j++)
                {
                    double clusterDistance = 0;

                    var counter = 0;
                    foreach (var purchase in purchases)
                    {
                        var clusterLocationForOffer = clusterLocations.Rows[counter];

                        //Euclidian Sum
                        var purchaseValue = purchase.Equals("1") ? 1 : 0;
                        var clusterPosition = clusterLocationForOffer[j + 1];

                        var clusterOffer = clusterLocationForOffer[j + 1] == DBNull.Value ? 0 : clusterLocationForOffer[j + 1];
                        double k = purchaseValue - float.Parse(clusterOffer.ToString());
                        clusterDistance += Math.Pow(k, 2);
                        counter++;
                    }
                    //Euclidian Squareroot
                    clusterDistance = Math.Sqrt(clusterDistance);


                    newDistancerow[j + 1] = clusterDistance;

                    //Assign to nearest cluster
                    if (clusterDistance < smallestClusterDistance)
                    {
                        smallestClusterDistance = clusterDistance;
                        assignedCluster = j + 1;
                    }
                }
                newDistancerow[distancesTable.Columns.Count - 1] = assignedCluster;
                distancesTable.Rows.Add(newDistancerow);
            }
            return distancesTable;
        }


        private DataTable CreateDataTable(int length)
        {
            var table = new DataTable();
            table.Columns.Add("Customer");
            for (var i = 0; i < length; i++)
            {
                table.Columns.Add("Cluster " + (i + 1));
            }
            table.Columns.Add("Assigned Cluster");
            return table;
        }

        public float CalculateTotalDistance(DataTable distancesTable, int k)
        {
            float totalDistance = 0;
            for (var i = 1; i <= k; i++)
            {
                //Select all Customers assigned to cluster k
                var distances =
                    distancesTable.AsEnumerable()
                        .Where(s => s.Field<String>(distancesTable.Columns.Count - 1) == i.ToString())
                        .ToList();
                foreach (var distance in distances)
                {
                    totalDistance += float.Parse(distance[i].ToString());
                }
            }
            return totalDistance;
        }

        
    }
}