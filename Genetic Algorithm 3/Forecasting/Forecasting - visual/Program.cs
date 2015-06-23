using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Forecasting___visual;
using GeneticAlgorithm;

namespace Forecasting
{
    class Program
    {
        private const string FilePath = "../../../SwordForecasting.csv";
        private const int TotalPeriods = 36;
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FileReader fileReader = new FileReader();

            var dataSet =fileReader.ReadDataFromFile(FilePath);
            
            SES ses = new SES(dataSet);
            ses.Execute();

//            DES des = new DES(dataSet);
//            des.Execute();

            var form = new Form1();
            form.LoadData(dataSet, TotalPeriods);
            Application.Run(form);

        }
    }
}
