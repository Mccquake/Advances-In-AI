namespace SimpleGeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    using SimpleGeneticAlgorithm.Extensions;
    using SimpleGeneticAlgorithm.Helpers;
    using SimpleGeneticAlgorithm.Models;

    using Utilities.Extensions;

    public class GeneticAlgorithm
    {
        private int PopulationSize { get; set; }

        private int RuleSize { get; set; }

        private int RuleAmount { get; set; }

        private double MutationRate { get; set; }

        private double CrossoverRate { get; set; }

        public IDictionary<IList<string>, int> InputData { get; set; }

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
            var startingPopulation = new Individual[PopulationSize];

            // For each individual in the population...
            for (var i = 0; i < PopulationSize; i++)
            {
                startingPopulation[i] = new Individual { Fitness = 0, Rules = new List<string>() };

                // For each rule in the individual...
                for (var j = 0; j < RuleAmount; j++)
                {
                    var ruleInput = string.Empty;
                    var ruleOutput = RandomHelper.Random.Next() % 2;

                    // For each section of the rule...
                    for (var x = 0; x < RuleSize; x++)
                    {
                        ruleInput += RandomHelper.Random.Next() % 3;
                    }

                    startingPopulation[i].Rules.Add(ruleInput + ruleOutput);
                }
            }

            foreach (var individual in startingPopulation)
            {
                individual.CalculateFitness(this.InputData, this.RuleSize);
            }

            return startingPopulation;
        }

        /// <summary>
        /// Runs the tournament selection process.
        /// </summary>
        /// <param name="oldPopulation"></param>
        /// <returns></returns>
        public IList<Individual> RunTournamentSelection(IList<Individual> oldPopulation, int size)
        {
            var resultingOffspring = new Individual[PopulationSize];

            for (var i = 0; i < PopulationSize; i++)
            {
                var parents = new Individual[size];
                var random = RandomHelper.Random;

                for(var j = 0; j < size; j++)
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
        public IList<Individual> BreedOffspring(IList<Individual> oldPopulation)
        {
            var newPopulation = new List<Individual>(PopulationSize);

            oldPopulation.Shuffle();

            for (var i = 0; i < PopulationSize; i+= 2)
            {
                var parent1 = i;
                var parent2 = i + 1;

                if (RandomHelper.Random.Next(1, 101) > this.CrossoverRate)
                {
                    var oldParent1 = new Individual { Rules = oldPopulation[parent1].Rules.ToList() };
                    var oldParent2 = new Individual { Rules = oldPopulation[parent2].Rules.ToList() };

                    oldParent1.Mutate(this.MutationRate);
                    oldParent2.Mutate(this.MutationRate);

                    newPopulation.Add(oldParent1);
                    newPopulation.Add(oldParent2);

                    continue;
                }

                var totalRulesetSize = this.RuleAmount * (this.RuleSize + 1);

                var crossoverPoint = RandomHelper.Random.Next() % totalRulesetSize;

                var parent1Ruleset = String.Concat(oldPopulation[parent1].Rules);
                var parent2Ruleset = String.Concat(oldPopulation[parent2].Rules);

                var child1TempRuleset = String.Concat(parent1Ruleset.Substring(0, crossoverPoint), parent2Ruleset.Substring(crossoverPoint));
                var child2TempRuleset = String.Concat(parent2Ruleset.Substring(0, crossoverPoint), parent1Ruleset.Substring(crossoverPoint));

                var child1Ruleset = child1TempRuleset.SubstringSplit(this.RuleSize + 1);
                var child2Ruleset = child2TempRuleset.SubstringSplit(this.RuleSize + 1);

                var child1 = new Individual() { Fitness = 0, Rules = child1Ruleset.ToList()};
                var child2 = new Individual() { Fitness = 0, Rules = child2Ruleset.ToList()};

                child1.Mutate(this.MutationRate);
                child2.Mutate(this.MutationRate);

                newPopulation.AddRange(new [] {child1, child2});
            }

            foreach (var individual in newPopulation)
            {
                individual.CalculateFitness(this.InputData, this.RuleSize);
            }

            return newPopulation;
        }
    }
}
