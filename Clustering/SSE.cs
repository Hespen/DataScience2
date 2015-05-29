using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;

namespace Clustering
{
    internal class SSE
    {
        public float CalculateDistances(DataTable pivot, DataTable clusterLocations, DataTable distancesTable, int k)
        {
            float sse = 0;
            for (var cluster = 1; cluster <= k; cluster++)
            {
                var assignedUserDistances =
                    distancesTable.AsEnumerable()
                        .Where(s => s.Field<string>(distancesTable.Columns.Count - 1) == cluster.ToString())
                        .ToList();
                var names = assignedUserDistances.AsEnumerable().Select(s => s.Field<string>("Customer"));
                for (var i = 0; i < pivot.Rows.Count; i++)
                {
                    var offer = pivot.Rows[i];
                    var clusterLocation = float.Parse(clusterLocations.Rows[i][cluster].ToString());
                    foreach (var name in names)
                    {
                        float customerPosition = offer.Field<string>(name).Equals("1") ? 1 : 0;
                            
                        sse += (float) Math.Pow(clusterLocation - customerPosition, 2);
                    }
                }
            }
            return sse;
        }
    }
}