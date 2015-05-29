using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Clustering
{
    internal class TopDeals
    {
        public DataTable CalculateTopDeals(DataTable pivot, DataTable distancesTable, int cluster)
        {
            var topDealsList = new List<DataTable>();
            var topDeals = new DataTable();
            topDeals.Columns.Add("Offer");
            topDeals.Columns.Add("Cluster " + cluster);
            topDeals.Columns[1].DataType = Type.GetType("System.Int32");


            foreach (var offer in pivot.AsEnumerable())
            {
                var offerDeals = topDeals.NewRow();
                offerDeals[0] = offer[0];
                var assignedUserDistances =
                    distancesTable.AsEnumerable()
                        .Where(s => s.Field<string>(distancesTable.Columns.Count - 1) == cluster.ToString())
                        .ToList();
                //Get the names
                var names = assignedUserDistances.AsEnumerable().Select(s => s.Field<string>("Customer"));
                var totalBought = 0;
                foreach (var name in names)
                {
                    if (offer.Field<string>(name).Equals("1"))
                    {
                        totalBought++;
                    }
                }

                if (totalBought < 3) continue;

                offerDeals[1] = totalBought;
                topDeals.Rows.Add(offerDeals);
            }

            var orderedTable = topDeals.AsEnumerable().OrderBy(x => x);
            var dv = topDeals.DefaultView;
            dv.Sort = "Cluster " + cluster + " desc";
            return dv.ToTable();
        }
    }
}