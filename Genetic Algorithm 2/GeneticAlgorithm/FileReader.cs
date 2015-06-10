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
                    var line = sr.ReadLine();
                    table = ProcessColumnNames(line, table);
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

        private static DataTable ProcessColumnNames(string line, DataTable table)
        {
            var data = line.Split(';');
            foreach (var s in data)
            {
                table.Columns.Add(s);
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
            return row;
        }
    }
}