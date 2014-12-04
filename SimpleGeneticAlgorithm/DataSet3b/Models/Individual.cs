namespace DataSet3b.Models
{
    using System.Collections.Generic;

    using DataSet3b.Helpers;
    public class Individual
    {
        /// <summary>
        /// The genome of an individual - the ruleset!
        /// </summary>
        public List<int> Genes { get; set; }

        /// <summary>
        /// The fitness of an individual.
        /// </summary>
        public int Fitness { get; set; }

        /// <summary>
        /// Overriden for use in file output.
        /// </summary>
        /// <returns>String representation of an <see cref="Individual"/></returns>
        public override string ToString()
        {
            return string.Join(", ", this.Genes);
        }

        /// <summary>
        /// Calculates the fitness of the individual.
        /// </summary>
        /// <param name="testData">Data to test fitness against.</param>
        /// <param name="ruleSize">Size of the rules used.</param>
        public void CalculateFitness(IDictionary<IList<double>, int> testData, int ruleSize)
        {
            this.Fitness = 0;

            foreach (var testRow in testData)
            {
                for (var i = 0; i < this.Genes.Count; i += 7)
                {
                    var rule = this.Genes.GetRange(i, 7);
                    var match = true;

                    for (var j = 0; j < ruleSize; j++)
                    {
                        if (rule[j] == testRow.Key[j] || rule[j] == 2)
                        {
                            continue;
                        }

                        match = false;
                        break;
                    }

                    if (match)
                    {
                        if (rule[ruleSize] == testRow.Value)
                        {
                            this.Fitness++;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Applies mutation to the current individual.
        /// Utilises either an XOR or a swap depending on the current gene.
        /// </summary>
        /// <param name="mutationRate">Rate of mutation.</param>
        public void Mutate(double mutationRate)
        {
            var random = RandomHelper.Random;

            for (var i = 0; i < this.Genes.Count; i++)
            {
                if (random.NextDouble() > mutationRate)
                {
                    continue;
                }

                if ((i + 1) % 7 == 0)
                {
                    var newGene = this.Genes[i];
                    newGene ^= 1;
                    this.Genes[i] = newGene;

                    continue;
                }

                var mutation = random.Next(2);

                if (mutation == Genes[i])
                {
                    this.Genes[i] = 2;
                }
                else
                {
                    this.Genes[i] = mutation;
                }
            }
        }
    }
}
