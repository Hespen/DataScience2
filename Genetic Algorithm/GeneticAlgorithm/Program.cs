using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GeneticAlgorithm
{
    class Program
    {
        static int dataSize = 20;
        static int populationSize = 30;
        static int numberOfIterations = 50;

        static void Main(string[] args)
        {
            /* FUNCTIONS TO DEFINE (for each problem):
            Func<Ind> createIndividual;                                 ==> input is nothing, output is a new individual
            Func<Ind,double> computeFitness;                            ==> input is one individual, output is its fitness
            Func<Ind[],double[],Func<Tuple<Ind,Ind>>> selectTwoParents; ==> input is an array of individuals (population) and an array of corresponding fitnesses, output is a function which (without any input) returns a tuple with two individuals (parents)
            Func<Tuple<Ind, Ind>, Tuple<Ind, Ind>> crossover;           ==> input is a tuple with two individuals (parents), output is a tuple with two individuals (offspring/children)
            Func<Ind, double, Ind> mutation;                            ==> input is one individual and mutation rate, output is the mutated individual
            */
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            GeneticAlgorithm<int[]> fakeProblemGA = new GeneticAlgorithm<int[]>(0.95, 0.01, true, populationSize, dataSize, numberOfIterations); 
            var solution = fakeProblemGA.Run(fakeProblemGA.CreateIndividual, fakeProblemGA.ComputeFitness, fakeProblemGA.SelectTwoParents, fakeProblemGA.Crossover, fakeProblemGA.Mutation);
            stopWatch.Stop();
            
            Console.WriteLine("Solution: ");

            Console.Write("[");
            int counter = 0;
            foreach (var number in solution)
            {
                counter++;
                Console.Write(number);
                if (counter < solution.Length)
                {
                    Console.Write(", ");
                }
            }
            Console.Write("]");

            Console.WriteLine("\n\nTotal fitness: ");
            Console.WriteLine(solution.Sum());

            Console.WriteLine("\n\nElapsed time: ");
            Console.WriteLine(stopWatch.ElapsedMilliseconds + " milliseconds");
            Console.ReadKey();

        }
    }
}
