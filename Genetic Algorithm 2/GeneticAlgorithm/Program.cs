using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;

namespace GeneticAlgorithm
{
    class Program
    {
        private const int DataSize = 20;
        private const int PopulationSize = 30;
        private const int NumberOfIterations = 100;

        private const string FilePath = "../../RetailMart.csv";

        static void Main(string[] args)
        {
            FileReader reader = new FileReader();
            DataTable purchaseData = reader.ReadDataFromFile(FilePath);

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            GeneticAlgorithm<double[]> fakeProblemGA = new GeneticAlgorithm<double[]>(0.95, 0.01, true, PopulationSize, DataSize, NumberOfIterations, purchaseData);
            Tuple<double[], double> solution = fakeProblemGA.Run(fakeProblemGA.CreateIndividual, fakeProblemGA.ComputeFitness, fakeProblemGA.SelectTwoParents, fakeProblemGA.Crossover, fakeProblemGA.Mutation);

            stopWatch.Stop();
            
            Console.WriteLine("Solution: ");

            Console.Write("[");
            int counter = 0;
            foreach (var number in solution.Item1)
            {
                counter++;
                Console.Write(number);
                if (counter < solution.Item1.Length)
                {
                    Console.Write(", ");
                }
            }
            Console.Write("]");

            Console.WriteLine("\n\nTotal fitness: ");
            Console.WriteLine(solution.Item2);

            Console.WriteLine("\n\nElapsed time: ");
            Console.WriteLine(stopWatch.ElapsedMilliseconds + " milliseconds");
            Console.ReadKey();

        }
    }
}
