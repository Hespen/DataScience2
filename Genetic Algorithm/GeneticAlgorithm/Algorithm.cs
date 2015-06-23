using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm
{
    public class GeneticAlgorithm<Ind>
    {
        double crossoverRate;
        double mutationRate;
        bool elitism;
        int populationSize;
        int numIterations;
        int dataSize;
        readonly Random _random;

        public GeneticAlgorithm(double crossoverRate, double mutationRate, bool elitism, int populationSize, int dataSize, int numIterations)
        {
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.elitism = elitism;
            this.populationSize = populationSize;
            this.dataSize = dataSize;
            this.numIterations = numIterations;
            _random = new Random();
        }

        public Ind Run(Func<Ind> createIndividual, Func<Ind,double> computeFitness, Func<Ind[],double[],Func<Tuple<Ind,Ind>>> selectTwoParents, 
            Func<Tuple<Ind,Ind>,Tuple<Ind,Ind>> crossover, Func<Ind, double, Ind> mutation)
        {
            // initialize the first population
            var initialPopulation = Enumerable.Range(0, populationSize).Select(i => createIndividual()).ToArray();


            var currentPopulation = initialPopulation;
            
            for (int generation = 0; generation < numIterations; generation++)
            {
                // compute fitness of each individual in the population
                var fitnesses = Enumerable.Range(0, populationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();

                var nextPopulation = new Ind[populationSize]; 

                // apply elitism
                int startIndex; 
                if(elitism)
                {
                    startIndex = 1;
                    var populationWithFitness = currentPopulation.Select((individual, index) => new Tuple<Ind,double>(individual,fitnesses[index]));
                    var populationSorted = populationWithFitness.OrderByDescending(tuple => tuple.Item2); // item2 is the fitness
                    var bestIndividual = populationSorted.First();
                    nextPopulation[0] = bestIndividual.Item1;
                } else
                {
                    startIndex = 0;
                }

                // initialize the selection function given the current individuals and their fitnesses
                var getTwoParents = selectTwoParents(currentPopulation, fitnesses);
                
                // create the individuals of the next generation
                for (int newInd = startIndex; newInd < populationSize; newInd++)
                {
                    // select two parents
                    var parents = getTwoParents();

                    // do a crossover between the selected parents to generate two children (with a certain probability, crossover does not happen and the two parents are kept unchanged)
                    Tuple<Ind,Ind> offspring;
                    if (_random.NextDouble() < crossoverRate)
                        offspring = crossover(parents);
                    else
                        offspring = parents;

                    // save the two children in the next population (after mutation)
                    nextPopulation[newInd++] = mutation(offspring.Item1, mutationRate);
                    if (newInd < populationSize) //there is still space for the second children inside the population
                        nextPopulation[newInd] = mutation(offspring.Item2, mutationRate);
                }

                // the new population becomes the current one
                currentPopulation = nextPopulation;

                // in case it's needed, check here some convergence condition to terminate the generations loop earlier
            }

            // recompute the fitnesses on the final population and return the best individual
            var finalFitnesses = Enumerable.Range(0, populationSize).Select(i => computeFitness(currentPopulation[i])).ToArray();
            return currentPopulation.Select((individual, index) => new Tuple<Ind, double>(individual, finalFitnesses[index])).OrderByDescending(tuple => tuple.Item2).First().Item1;
        }

        public int[] CreateIndividual()
        {
            int[] individual = new int[dataSize];

            // Generate a random binary number for each index in the data size to create an individual.
            for (int i = 0; i < dataSize; i++)
            {
                individual[i] = ((_random.Next() % 2 == 0) ? 0 : 1);
            }
            return individual;
        }

        public double ComputeFitness(int[] individual)
        {
            //Count the amount of 1's in the set of data
            return individual.Count<int>(i => i == 1);
        }

        public Func<Tuple<int[], int[]>> SelectTwoParents(int[][] currentPopulation, double[] fitness)
        {
            double totalFitness = fitness.Sum();
            double[] cummulativeProbabilities = new double[populationSize];

            // Loop through all individuals in the population and calculate their probability values.
            // We notate all probability values cummulatively. The last value will be 1, or very close to it.
            // This allows us to choose a random number between 0 and 1, and see wich value in the cummulative array contains
            // the random value in its range, where range is: previousIndividual < x < chosenIndividual;
            for (int individual = 0; individual < currentPopulation.Length; individual++)
            {
                double probability = fitness[individual] / totalFitness;

                if (individual == 0)
                {
                    cummulativeProbabilities[individual] = probability;
                }
                else
                {
                    cummulativeProbabilities[individual] = cummulativeProbabilities[individual - 1] + probability;
                }
            }

            return () => GetTwoParents(cummulativeProbabilities, currentPopulation);
        }

        public Tuple<int[], int[]> GetTwoParents(double[] cummulativeProbabilities, int[][] currentPopulation)
        {
            int[] parents = new int[2];
            bool firstParentFound = false;

            // We're looking for two parents
            while (true)
            {

                double randomDouble = _random.NextDouble();

                // Loop through all cummulative probabilities. If the probability is bigger than the random number,
                // it means we found the individual that has the randomDouble in its range so we pick it to be a parent.
                for (int probability = 0; probability < cummulativeProbabilities.Length; probability++)
                {
                    if (cummulativeProbabilities[probability] > randomDouble)
                    {
                        // Make sure we don't return two clones as parents.
                        if (firstParentFound == true)
                        {
                            // First parent found and current individual is not the same one as the first parent.
                            if (parents[0] != probability)
                            {
                                parents[1] = probability;
                                goto returnStatement;
                            }
                            // First parent found, but current individual is the same one as the first parent.
                            else
                            {
                                break;
                            }
                        }
                        // First parent not found yet.
                        else
                        {
                            parents[0] = probability;
                            firstParentFound = true;
                            break;
                        }
                    }
                }
            }

            returnStatement:

            return new Tuple<int[], int[]>(currentPopulation[parents[0]], currentPopulation[parents[1]]);
        } 

        public Tuple<int[], int[]> Crossover(Tuple<int[], int[]> parents){
            int[][] offspring = new int[2][];

            int crossoverPoint = _random.Next(dataSize);

            // First offspring
            var firstString = parents.Item1.Take(crossoverPoint);
            var secondString = parents.Item2.Reverse().Take(dataSize-crossoverPoint).Reverse();

            offspring[0] = firstString.Concat(secondString).ToArray();

            // Second offspring
            firstString = parents.Item2.Take(crossoverPoint);
            secondString = parents.Item1.Reverse().Take(dataSize - crossoverPoint).Reverse();

            offspring[1] = firstString.Concat(secondString).ToArray();

            return new Tuple<int[], int[]>(offspring[0], offspring[1]);
        }

        public int[] Mutation(int[] individual, double mutationRate)
        {
            int counter = 0;
            for (int i = 0; i < individual.Length; i++)
            {
                if (_random.NextDouble() <= mutationRate)
                {
                    counter++;
                    // Mirror value; 1 -> 0 or 1 -> 0
                    individual[i] = individual[i] == 0 ? 1 : 0;
                }
            }
            return individual;
        }

    }

}