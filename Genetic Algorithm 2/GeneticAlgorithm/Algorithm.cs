using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithm<Ind>
    {
        private double _crossoverRate;
        private readonly double _mutationRate;
        private readonly bool _elitism;
        private readonly int _populationSize;
        private readonly int _numIterations;
        private readonly int _dataSize;
        private Random _r;

        private readonly DataTable _purchaseData;

        public GeneticAlgorithm(double crossoverRate, double mutationRate, bool elitism, int populationSize, int dataSize, int numIterations, DataTable purchaseData)
        {
            _crossoverRate = crossoverRate;
            _mutationRate = mutationRate;
            _elitism = elitism;
            _populationSize = populationSize;
            _dataSize = dataSize;
            _numIterations = numIterations;
            _purchaseData = purchaseData;
        }

        public Tuple<Ind, double> Run(Func<Ind> createIndividual, Func<Ind,double> computeFitness, Func<Ind[],double[],Func<Tuple<Ind,Ind>>> selectTwoParents, 
            Func<Tuple<Ind,Ind>,Tuple<Ind,Ind>> crossover, Func<Ind, double, Ind> mutation)
        {
            // initialize the first population
            var initialPopulation = Enumerable.Range(0, _populationSize).Select(i => createIndividual()).ToArray();


            var currentPopulation = initialPopulation;
            
            for (int generation = 0; generation < _numIterations; generation++)
            {
                // compute fitness of each individual in the population
                var fitnesses = Enumerable.Range(0, _populationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();

                var nextPopulation = new Ind[_populationSize]; 

                // apply elitism
                int startIndex; 
                if(_elitism)
                {
                    startIndex = 1;
                    var populationWithFitness = currentPopulation.Select((individual, index) => new Tuple<Ind,double>(individual,fitnesses[index]));
                    var populationSorted = populationWithFitness.OrderBy(tuple => tuple.Item2); // item2 is the fitness
                    var bestIndividual = populationSorted.First();
                    nextPopulation[0] = bestIndividual.Item1;
                } else
                {
                    startIndex = 0;
                }

                // initialize the selection function given the current individuals and their fitnesses
                var getTwoParents = selectTwoParents(currentPopulation, fitnesses);
                
                // create the individuals of the next generation
                for (int newInd = startIndex; newInd < _populationSize; newInd++)
                {
                    // select two parents
                    var parents = getTwoParents();

                    // do a crossover between the selected parents to generate two children (with a certain probability, crossover does not happen and the two parents are kept unchanged)
                    Tuple<Ind,Ind> offspring;
                    if (_r.NextDouble() < _crossoverRate)
                        offspring = crossover(parents);
                    else
                        offspring = parents;

                    // save the two children in the next population (after mutation)
                    nextPopulation[newInd++] = mutation(offspring.Item1, _mutationRate);
                    if (newInd < _populationSize) //there is still space for the second children inside the population
                        nextPopulation[newInd] = mutation(offspring.Item2, _mutationRate);
                }

                // the new population becomes the current one
                currentPopulation = nextPopulation;

                // in case it's needed, check here some convergence condition to terminate the generations loop earlier
            }

            // recompute the fitnesses on the final population and return the best individual
            var finalFitnesses = Enumerable.Range(0, _populationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();
            return currentPopulation.Select((individual, index) => new Tuple<Ind, double>(individual, finalFitnesses[index])).OrderBy(tuple => tuple.Item2).First();
        }

        public double[] CreateIndividual()
        {
            if (_r == null)
            {
                _r = new Random();
            }

            double[] individual = new double[_dataSize];
            

            // Generate a random binary number for each index in the data size to create an individual.
            for (int i = 0; i < _dataSize; i++)
            {
                individual[i] = _r.NextDouble() * (1 - -1) - 1;
            }

            return individual;

        }

        public double ComputeFitness(double[] individual)
        {
            // Fitness is measured by the sse; the lower the sse, the better the fitness.
            double sse = 0;

            // Loop through all purchase data rows
            foreach (DataRow purchase in _purchaseData.Rows)
            {
                double prediction = 0;

                // Loop through all columns except for PREGNANT collumn, so choose the individual length, which doesn't contain PREGNANT column.
                for (int i = 0; i < individual.Length; i++)
                {
                    prediction += (individual[i] * Convert.ToDouble(purchase[i]));
                }
                sse += Math.Pow(Convert.ToDouble(purchase[_purchaseData.Columns.Count - 1]) - prediction, 2);
            }

            return sse;
        }

        public Func<Tuple<double[], double[]>> SelectTwoParents(double[][] currentPopulation, double[] fitness)
        {
            const int tournamentSize = 18;

            return () => GetTwoParents(tournamentSize, currentPopulation, fitness);
        }


        public Tuple<double[], double[]> GetTwoParents(int tournamentSize, double[][] currentPopulation, double[] fitness)
        {
            int[] parents = new int[2];

            // Get first parent
            parents[0] = RunCompetition(tournamentSize, currentPopulation, fitness);
            // Get 2nd parent, now with an extra parameter: The first parent. This makes sure the first parent doesn't participate in this tournament.
            parents[1] = RunCompetition(tournamentSize, currentPopulation, fitness, parents[0]);



            return new Tuple<double[], double[]>(currentPopulation[parents[0]], currentPopulation[parents[1]]);
        }

        /// <summary>
        /// This method randomly chooses participators for the tournament and 'runs' the actual tournament by 
        /// selecting the individual with the best (lowest) fitness.
        /// </summary>
        /// <param name="tournamentSize">Number of people in a single tournament</param>
        /// <param name="currentPopulation">All individuals within the population</param>
        /// <param name="fitness">All fitnesses of the individuals</param>
        /// <param name="firstParentIndex">If the method is run for the second time, this parameter contains the first winner</param>
        /// <returns></returns>
        private int RunCompetition(int tournamentSize, double[][] currentPopulation, double[] fitness, int firstParentIndex = -1)
        {
            List<int> participatorIndexes = new List<int>();

            // Generate random numbers, which represents indexes of the tournament participators
            while (true)
            {
                int randomParticipatorIndex = _r.Next(0, currentPopulation.Length);
                if (!participatorIndexes.Contains(randomParticipatorIndex) && randomParticipatorIndex != firstParentIndex)
                {
                    participatorIndexes.Add(randomParticipatorIndex);

                    if (participatorIndexes.Count == tournamentSize)
                    {
                        break;
                    }
                }
            }

            // Store index of the best individual
            int bestIndex = -1;

            // Loop trough all participators and choose the person with the lowest fitness as the winner.
            foreach (var participatorIndex in participatorIndexes)
            {

                if (bestIndex == -1 || fitness[participatorIndex] < fitness[bestIndex])
                {
                    bestIndex = participatorIndex;
                }
            }

            return bestIndex;
        }

        public Tuple<double[], double[]> Crossover(Tuple<double[], double[]> parents)
        {
            double[][] offspring = new double[2][];
            offspring[0] = new double[_dataSize];
            offspring[1] = new double[_dataSize];

            for (int i = 0; i < _dataSize; i++)
            {
                // Uniform crossover, so randomly crossover data points. Generate number between 0 and 1 to determine which parent to select from.
                if (_r.Next(0, 2) == 0)
                {
                    offspring[0][i] = parents.Item1[i];
                    offspring[1][i] = parents.Item2[i];
                }
                else
                {
                    offspring[0][i] = parents.Item2[i];
                    offspring[1][i] = parents.Item1[i];
                }
            }

            return new Tuple<double[], double[]>(offspring[0], offspring[1]);
        }

        public double[] Mutation(double[] individual, double mutationRate)
        {
            for (int i = 0; i < individual.Length; i++)
            {
                if (_r.NextDouble() <= mutationRate)
                {
                    // Mirror value; Positive becomes negative and negative becomes positive.
                    individual[i] = individual[i] * -1;
                }
            }
            return individual;
        }

    }

}