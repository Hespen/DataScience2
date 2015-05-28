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

        public DataTable CreateClusters()
        {
            var rand = new Random();
            var clusterLocations = new DataTable();
            clusterLocations.Columns.Add("Offer");
            for (var i = 1; i <= _k; i++)
            {
                clusterLocations.Columns.Add("Cluster " + i);
            }
            var locations = new float[_k];
            
            for (var i = 1; i <= 32; i++)
            {
                var row = clusterLocations.NewRow();
                row[0] = i;
                for (var j = 1; j <= _k; j++)
                {
                    row[j] = rand.NextDouble();
                }
                clusterLocations.Rows.Add(row);
//                clusterLocations.Add(new Tuple<int, float[]>(i, locations));
            }
            return clusterLocations;
        }

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
                    //Get all Rows assigned to cluster k
                    var assignedUserDistances =
                        distancesTable.AsEnumerable()
                            .Where(s => s.Field<string>(distancesTable.Columns.Count - 1) == cluster.ToString())
                            .ToList();
                    //Get the names
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