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

        /// <summary>
        /// This method calculates the distances between customers
        /// </summary>
        /// <param name="pivot">Binary purchase data</param>
        /// <returns>A DataTable which contains distances between all customers</returns>
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
                //Get column of customer A to use for Euclidian
                var columnCustomerA = pivot.AsEnumerable().Select(s => s.Field<String>(i)).ToList();

                var dataRow = distances.NewRow();
                //Give row's first value the customers name
                dataRow[0] = pivot.Columns[i];

                for (var j = 1; j < pivot.Columns.Count; j++)
                {
                    //Get column of customer B to use for Euclidian
                    var columnCustomerB = pivot.AsEnumerable().Select(s => s.Field<String>(j)).ToList();
                    float distance = 0;
                    //Loop through the rows of the A and B columns, which is all binary purchase data for both customers
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
        
        /// <summary>
        /// Calculates the average silhouette value.
        /// </summary>
        /// <param name="customerDistances">Distances between all customers</param>
        /// <param name="distancesTable">Distance from customers to clusters</param>
        /// <param name="k">Amount of clusters</param>
        /// <returns>Silhoutte value</returns>
        public double CalculateSilhoutte(DataTable customerDistances, DataTable distancesTable,int k)
        {

            var view = new DataView(distancesTable);
            //Create a new DataTable with the columns Customer & Assigned Cluster
            var assignments = view.ToTable("SELECTED", false,"Customer", "Assigned Cluster");

            // Contains silhouette values for each customer.
            var silhouetteList = new List<double>();

            foreach (var customerA in customerDistances.AsEnumerable())
            {
                var averageDistances = new float[k];
                var customerACluster = 0;
                for (var i = 1; i <= k; i++)
                {
                    float totalDistance = 0;
                    //Find all names assigned to Cluster i
                    var names = assignments.AsEnumerable().Where(s => s.Field<String>(1) == i.ToString()).ToList();
                    //For every customerB in names, get the distance value between customerA 
                    foreach(var customerB in names)
                    {
                        var t = customerB.Field<String>(0);
                        if (t.Equals(customerA[0])) customerACluster = i;
                        //Add cell value to totalDistance
                        totalDistance += float.Parse(customerA.Field<string>(t));
                    }

                   averageDistances[i-1] = totalDistance/names.Count;
                }

                var ownCluster = averageDistances[customerACluster - 1];

                //Remove own cluster distance from array, so we can pick the second closest by using the .Min() method.
                averageDistances = averageDistances.Where(val => val != ownCluster).ToArray();
                var nearestCluster = averageDistances.Min();

                silhouetteList.Add((nearestCluster - ownCluster)/Math.Max(nearestCluster, ownCluster));
            }
            return silhouetteList.Average();
        }
    }
}
