using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Clustering
{
    internal class ClusterHandler
    {
        private readonly int _k;

        public ClusterHandler(int k)
        {
            _k = k;
        }

        /// <summary>
        /// This method creates all initial clusters/centroids at random.
        /// </summary>
        /// <returns>Centroid locations for 4 cluster in 32 dimensions</returns>
        public DataTable CreateClusters()
        {
            var rand = new Random();
            var clusterLocations = new DataTable();

            clusterLocations.Columns.Add("Offer");
            for (var i = 1; i <= _k; i++)
            {
                clusterLocations.Columns.Add("Cluster " + i);
            }
            
            // We need to create a centroid position for each dimension, in this case there are 32 dimensions.
            for (var i = 1; i <= 32; i++)
            {
                var row = clusterLocations.NewRow();
                row[0] = i;

                // Generate random centroid position for each cluster
                for (var j = 1; j <= _k; j++)
                {
                    row[j] = rand.NextDouble();
                }
                clusterLocations.Rows.Add(row);
            }
            return clusterLocations;
        }

        /// <summary>
        /// This method updates centroids for k clusters in all dimensions.
        /// </summary>
        /// <param name="pivot">Binary data of purchases per offer</param>
        /// <param name="distancesTable">Distances from each customer to each cluster and assigned cluster</param>
        /// <param name="k">Number of clusters used</param>
        /// <returns>A DataTable with the new updated centroids</returns>
        public DataTable UpdateCentroids(DataTable pivot, DataTable distancesTable, int k)
        {
            var clusterLocations = new DataTable();
            clusterLocations.Columns.Add("Offer");
            for (var i = 1; i <= _k; i++)
            {
                clusterLocations.Columns.Add("Cluster " + i);
            }

            var offerCounter = 0;
            foreach (var offer in pivot.AsEnumerable())
            {
                var row = clusterLocations.NewRow();
                var locations = new float[k];
                for (var cluster = 1; cluster <= k; cluster++)
                {
                    //Get all customers assigned to cluster k
                    var assignedUserDistances =
                        distancesTable.AsEnumerable()
                            .Where(s => s.Field<string>(distancesTable.Columns.Count - 1) == cluster.ToString())
                            .ToList();
                    //Get only the names of customers that are assigned to cluster k
                    var names = assignedUserDistances.AsEnumerable().Select(s => s.Field<string>("Customer"));
                    float totalBought = 0;
                    foreach (var name in names)
                    {
                        if (offer.Field<string>(name).Equals("1"))
                        {
                            totalBought++;
                        }
                    }
                    if (names.Count() == 0) continue;

                    // New centroid position is the number of purchases for the offer for cluster k, 
                    // divided by the total amount of customers assigned to the cluster.
                    row[cluster] = totalBought/names.Count();
                }
                offerCounter++;
                row[0] = offerCounter;
                clusterLocations.Rows.Add(row);
            }
            return clusterLocations;
        }

        public void CalculateSSE(DataTable distancesTable, DataTable clusterLocations)
        {
            for (var i = 1; i <= _k; i++)
            {
                foreach (var clusterLocation in clusterLocations.AsEnumerable().Where(s=>s.Field<String>(clusterLocations.Columns.Count-1)==i.ToString()).ToList())
                {
                    
                }
            }
        }
    }
}