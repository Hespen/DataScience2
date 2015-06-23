using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class FileReader
    {

        public FileReader()
        {

        }

        public DataTable ReadDataFromFile(String filePath)
        {
            var table = new DataTable();
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    table = ProcessColumnNames(table);
                    table = CreateInitialRow(table);
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        table.Rows.Add(ProcessLine(line, table));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                Console.ReadKey();
            }
            return table;
        }

        private DataTable CreateInitialRow(DataTable table)
        {
            DataRow row = table.NewRow();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                row[i] = 0;
            }
            table.Rows.Add(row);
            return table;
        }

        private static DataTable ProcessColumnNames(DataTable table)
        {
            var columnNames = new []{"t","Demand","Level Estimate","Forecast Error","Squared Error","Trend","Forecast"};
            foreach (var s in columnNames)
            {
                table.Columns.Add(s, typeof(double));
            }
            return table;
        }

        private DataRow ProcessLine(String line, DataTable table)
        {
            String[] data = line.Split(';');
            DataRow row = table.NewRow();
            for (int i = 0; i < data.Length; i++)
            {
                row[i] = data[i];
            }
            for (int i = data.Length; i < table.Columns.Count; i++)
            {
                row[i] = 0;
            }
            return row;
        }
    }
}