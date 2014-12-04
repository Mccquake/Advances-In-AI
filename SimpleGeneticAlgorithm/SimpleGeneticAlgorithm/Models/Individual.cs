namespace SimpleGeneticAlgorithm.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    using SimpleGeneticAlgorithm.Helpers;

    /// <summary>
    /// States the data structure for an individual in a population.
    /// </summary>
    public class Individual
    {
        public IList<string> Rules { get; set; }

        public int Fitness { get; set; }

        /// <summary>
        /// Wonderful technique for seeing all variables easily in Locals.
        /// </summary>
        /// <returns>Overridden string representation of an <see cref="Individual"/></returns>
        public override string ToString()
        {
            var strBuild = new StringBuilder();

            strBuild.AppendFormat("{0}{1}{2}", string.Join(",", Rules), ",", Fitness);

            return strBuild.ToString();
        }

        /// <summary>
        /// Calculates the fitness of the current ruleset.
        /// </summary>
        public void CalculateFitness(IDictionary<IList<string>, int> testData, int ruleSize)
        {
            this.Fitness = 0;

            foreach(var testRow in testData)
            {
                foreach (var rule in Rules)
                {
                    var ruleSections = rule.ToCharArray(0, ruleSize);
                    var match = true;

                    for (var i = 0; i < ruleSize; i++)
                    {
                        var ruleSection = ruleSections[i].ToString(CultureInfo.InvariantCulture);

                        if (ruleSection == testRow.Key[i] || ruleSection == "2")
                        {
                            continue;
                        }

                        match = false;
                        break;
                    }

                    if (match)
                    {
                        if (rule.EndsWith(testRow.Value.ToString(CultureInfo.InvariantCulture)))
                        {
                            this.Fitness++;
                        }

                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Mutates the current ruleset.
        /// </summary>
        public void Mutate(double mutationRate)
        {
            for (var i = 0; i < this.Rules.Count; i++)
            {
                var mutatedRule = string.Empty;
                var ruleChars = this.Rules[i].ToCharArray();

                for (var j = 0; j < ruleChars.Length; j++)
                {
                    var mutationChance = RandomHelper.Random.NextDouble();
                    
                    if (mutationChance > mutationRate)
                    {
                        mutatedRule += ruleChars[j];
                        continue;
                    }

                    if (j == ruleChars.Length - 1)
                    {
                        mutatedRule += ruleChars[j].Equals('1') ? '0' : '1';
                    }
                    else
                    {
                        var mutation = RandomHelper.Random.Next(2);

                        if (mutation == (int)Char.GetNumericValue(ruleChars[j]))
                        {
                            mutation = 2;
                        }

                        mutatedRule += mutation;
                    }
                }

                this.Rules[i] = mutatedRule;
            }
        }
    }
}
