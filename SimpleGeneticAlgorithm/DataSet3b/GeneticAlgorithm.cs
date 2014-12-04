namespace DataSet3b
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using DataSet3b.Helpers;
    using DataSet3b.Models;

    using Utilities.Extensions;

    public class GeneticAlgorithm
    {
        private int PopulationSize { get; set; }

        private int RuleSize { get; set; }

        private int RuleAmount { get; set; }

        private double MutationRate { get; set; }

        private double CrossoverRate { get; set; }

        public IDictionary<IList<double>, int> InputData { get; set; }

        public GeneticAlgorithm()
        {
            this.CrossoverRate = Convert.ToDouble(ConfigurationManager.AppSettings["CrossoverRate"]);
            this.MutationRate = Convert.ToDouble(ConfigurationManager.AppSettings["MutationRate"]);
            this.PopulationSize = Convert.ToInt32(ConfigurationManager.AppSettings["PopulationSize"]);
            this.RuleAmount = Convert.ToInt32(ConfigurationManager.AppSettings["RuleAmount"]);
            this.RuleSize = Convert.ToInt32(ConfigurationManager.AppSettings["RuleSize"]);
        }

        /// <summary>
        /// Creates a randomised population to be ran through the genetic process.
        /// </summary>
        /// <returns></returns>
        public IList<Individual> CreateInitialPopulation()
        {
            var population = new Individual[PopulationSize];
            var random = RandomHelper.Random;

            // For each individual in the population...
            for (var i = 0; i < PopulationSize; i++)
            {
                population[i] = new Individual { Fitness = 0, Genes = new List<int>() };

                var loop = RuleAmount * (RuleSize + 1);

                for (var j = 0; j < loop; j++)
                {
                    if ((j + 1) % 7 == 0)
                    {
                        population[i].Genes.Add(random.Next(2));
                        continue;
                    }

                    population[i].Genes.Add(random.Next(3));
                }
            }

            foreach (var individual in population)
            {
                individual.CalculateFitness(this.InputData, this.RuleSize);
            }

            return population;
        }

        /// <summary>
        /// Runs the tournament selection process.
        /// </summary>
        /// <param name="oldPopulation"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IList<Individual> RunSelection(IList<Individual> oldPopulation, int size)
        {
            var resultingOffspring = new Individual[PopulationSize];

            for (var i = 0; i < PopulationSize; i++)
            {
                var parents = new Individual[size];
                var random = RandomHelper.Random;

                for (var j = 0; j < size; j++)
                {
                    var parentIndex = random.Next(PopulationSize);
                    parents[j] = oldPopulation[parentIndex];
                }

                resultingOffspring[i] = parents.Aggregate((x, y) => x.Fitness > y.Fitness ? x : y);
            }

            return resultingOffspring;
        }

        /// <summary>
        /// Breeds new offspring using crossover and mutation.
        /// </summary>
        /// <param name="oldPopulation">Population generated from a selection process.</param>
        /// <returns></returns>
        public IList<Individual> CreateOffspring(IList<Individual> oldPopulation)
        {
            var newPopulation = new List<Individual>(PopulationSize);

            oldPopulation.Shuffle();

            for (var i = 0; i < PopulationSize; i += 2)
            {
                var parent1 = i;
                var parent2 = i + 1;

                if (RandomHelper.Random.Next(1, 101) > this.CrossoverRate)
                {
                    var oldParent1 = new Individual { Genes = oldPopulation[parent1].Genes.ToList() };
                    var oldParent2 = new Individual { Genes = oldPopulation[parent2].Genes.ToList() };

                    newPopulation.Add(oldParent1);
                    newPopulation.Add(oldParent2);

                    continue;
                }

                var ruleSize = (this.RuleSize + 1);
                var totalRulesetSize = this.RuleAmount * ruleSize;

                var crossoverPoint = RandomHelper.Random.Next(totalRulesetSize);

                var child1Ruleset = oldPopulation[parent1].Genes.Take(crossoverPoint).Concat(oldPopulation[parent2].Genes.Skip(crossoverPoint));
                var child2Ruleset = oldPopulation[parent2].Genes.Take(crossoverPoint).Concat(oldPopulation[parent1].Genes.Skip(crossoverPoint));

                var child1 = new Individual { Fitness = 0, Genes = child1Ruleset.ToList() };
                var child2 = new Individual { Fitness = 0, Genes = child2Ruleset.ToList() };

                newPopulation.AddRange(new[] { child1, child2 });
            }

            foreach (var individual in newPopulation)
            {
                individual.Mutate(this.MutationRate);
                individual.CalculateFitness(this.InputData, this.RuleSize);
            }

            return newPopulation;
        }
    }
}
