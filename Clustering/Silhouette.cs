using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering
{
    class Silhouette
    {

        public DataTable CalculateCustomerDistances(DataTable pivot)
        {
            var distances = new DataTable();
            distances.Columns.Add("Customer");

            //Create a column for every customer
            for (var i = 1; i < pivot.Columns.Count; i++)
            {
                distances.Columns.Add(pivot.Columns[i].ColumnName);
            }
            for (var i = 1; i < pivot.Columns.Count; i++)
            {
                //Get column A to use for Euclidian
                var columnCustomerA = pivot.AsEnumerable().Select(s => s.Field<String>(i)).ToList();

                var dataRow = distances.NewRow();
                //Give row's first value the customers name
                dataRow[0] = pivot.Columns[i];

                for (var j = 1; j < pivot.Columns.Count; j++)
                {
                    //Get column B to use for Euclidian
                    var columnCustomerB = pivot.AsEnumerable().Select(s => s.Field<String>(j)).ToList();
                    float distance = 0;
                    //Loop through the rows of the A and B columns
                    for (var k = 0; k < columnCustomerA.Count; k++)
                    {
                        //Euclidian
                        float valA = columnCustomerA[k].Equals("1") ? 1 : 0;
                        float valB = columnCustomerB[k].Equals("1") ? 1 : 0;
                        distance += (float)Math.Pow(valA - valB, 2);
                    }
                    distance = (float)Math.Sqrt(distance);
                    dataRow[j] = distance;
                }
                distances.Rows.Add(dataRow);
            }
            return distances;
        }
        
        public void CalculateSilhouette()
        {
            
        }

        public double CalculateAverageClusterDistance(DataTable customerDistances, DataTable distancesTable,int k)
        {
            var view = new DataView(distancesTable);
            var assignments = view.ToTable("SELECTED", false,"Customer", "Assigned Cluster");
            var silhouetteList = new List<double>();
            foreach (var customerA in customerDistances.AsEnumerable())
            {
                var averageDistances = new float[k];
                var customerACluster = 0;
                for (int i = 1; i <= k; i++)
                {
                    float totalDistance = 0;
                    var names = assignments.AsEnumerable().Where(s => s.Field<String>(1) == i.ToString()).ToList();
                    foreach(var customerB in names)
                    {
                        var t = customerB.Field<String>(0);
                        if (t.Equals(customerA[0])) customerACluster = i;
                        totalDistance += float.Parse(customerA.Field<string>(t));
                    }

                   averageDistances[i-1] = totalDistance/names.Count;
                }
                var ownCluster = averageDistances[customerACluster - 1];
                averageDistances = averageDistances.Where(val => val != ownCluster).ToArray();

                var nearestCluster = averageDistances.Min();

                silhouetteList.Add((nearestCluster - ownCluster)/Math.Max(nearestCluster, ownCluster));
            }
            var averageSilhouette = silhouetteList.Average();
            return averageSilhouette;
        }
    }
}
