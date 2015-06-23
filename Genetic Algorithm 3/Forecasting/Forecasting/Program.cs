using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneticAlgorithm;

namespace Forecasting
{
    class Program
    {
        private const string FilePath = "../../../SwordForecasting.csv";
        static void Main(string[] args)
        {
            FileReader fileReader = new FileReader();
            var dataSet =fileReader.ReadDataFromFile(FilePath);
            
//            SES ses = new SES(dataSet);
//            ses.Execute();

            DES des = new DES(dataSet);
            des.Execute();

        }
    }
}
