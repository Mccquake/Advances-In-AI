namespace DataSet3.Models
{
    using System.Collections.Generic;

    using DataSet3.Helpers;

    public class Individual
    {
        /// <summary>
        /// The genome of an individual - the ruleset!
        /// </summary>
        public List<double> Genes { get; set; }

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

            foreach (var row in testData)
            {
                for (var i = 0; i < this.Genes.Count; i += 13)
                {
                    var rule = this.Genes.GetRange(i, 13);
                    var match = true;
                    var x = 0;

                    for (var j = 0; j < ruleSize; j+= 2)
                    {
                        double lowest, highest;

                        if (rule[j] < rule[j + 1])
                        {
                            lowest = rule[j];
                            highest = rule[j + 1];
                        }
                        else
                        {
                            lowest = rule[j + 1];
                            highest = rule[j];
                        }

                        if (lowest < row.Key[x] && row.Key[x] < highest)
                        {
                            x++;
                            continue;
                        }

                        match = false;
                        break;
                    }

                    if (match)
                    {
                        if ((int)rule[12] == row.Value)
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
        /// Utilises either an XOR or a shift depending on the current gene.
        /// </summary>
        /// <param name="mutationRate">Rate of mutation.</param>
        public void Mutate(double mutationRate)
        {
            var random = RandomHelper.Random;

            for (var i = 0; i < Genes.Count; i++)
            {
                if (random.NextDouble() > mutationRate)
                {
                    continue;
                }

                if ((i + 1) % 13 == 0)
                {
                    var newGene = (int)this.Genes[i];
                    newGene ^= 1;
                    this.Genes[i] = newGene;

                    continue;
                }

                var mutator = random.NextDouble() * 0.2;

                if (random.Next(2) == 0)
                {
                    if (this.Genes[i] + mutator > 1.0)
                    {
                        this.Genes[i] = 1.0;
                    }
                    else
                    {
                        this.Genes[i] += mutator;
                    }
                }
                else
                {
                    if (this.Genes[i] - mutator < 0.0)
                    {
                        this.Genes[i] = 0.0;
                    }
                    else
                    {
                        this.Genes[i] -= mutator;
                    }
                }
            }
        }
    }
}
