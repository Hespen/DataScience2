using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    class Program
    {
        static int dataSize = 10;
        static int populationSize = 30;

        static void Main(string[] args)
        {
            /* FUNCTIONS TO DEFINE (for each problem):
            Func<Ind> createIndividual;                                 ==> input is nothing, output is a new individual
            Func<Ind,double> computeFitness;                            ==> input is one individual, output is its fitness
            Func<Ind[],double[],Func<Tuple<Ind,Ind>>> selectTwoParents; ==> input is an array of individuals (population) and an array of corresponding fitnesses, output is a function which (without any input) returns a tuple with two individuals (parents)
            Func<Tuple<Ind, Ind>, Tuple<Ind, Ind>> crossover;           ==> input is a tuple with two individuals (parents), output is a tuple with two individuals (offspring/children)
            Func<Ind, double, Ind> mutation;                            ==> input is one individual and mutation rate, output is the mutated individual
            */
            
            GeneticAlgorithm<int[]> fakeProblemGA = new GeneticAlgorithm<int[]>(1.0, 0.1, false, populationSize, dataSize, 1); // CHANGE THE GENERIC TYPE (NOW IT'S INT AS AN EXAMPLE) AND THE PARAMETERS VALUES
            var solution = fakeProblemGA.Run(fakeProblemGA.CreateIndividual, fakeProblemGA.ComputeFitness, fakeProblemGA.SelectTwoParents, fakeProblemGA.Crossover, fakeProblemGA.Mutation); 
            Console.WriteLine("Solution: ");
            Console.WriteLine(solution);

        }
    }
}
