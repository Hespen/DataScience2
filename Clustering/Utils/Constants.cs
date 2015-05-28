using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clustering.Utils
{
    class Constants
    {
        public static String TransactionLocation = "../Assets/Transaction.csv";
        public static String OfferLocation = "../Assets/OfferInformation.csv";
        public static String Pivot = "../../Assets/Pivot.csv";

        public const int Transaction = 1;
        public const int Offer = 2;
    }
}
