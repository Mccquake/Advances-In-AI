namespace DataSet3
{
    using DataSet3.Helpers;
    using DataSet3.Models;

    using Utilities.Data;
    using Utilities.Helpers;

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;

    public class Program
    {
        private static StringBuilder fullBuilder;
        private static StringBuilder testingBuilder;
        private static StringBuilder ruleBuilder;

        private static IDictionary<IList<double>, int> testingData;

        /// <summary>
        /// Entry point for the application.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            InitialiseSettings();

            fullBuilder = new StringBuilder();
            testingBuilder = new StringBuilder();
            ruleBuilder = new StringBuilder();

            for(var i = 0; i < 10; i++)
            {
                Run();
            }

            Finalise();
        }

        /// <summary>
        /// Runs the main bulk of the application.
        /// </summary>
        private static void Run()
        {
            var geneticAlgorithm = new GeneticAlgorithm();
            var bestIndividual = new Individual { Fitness = 0 };

            var generations = Convert.ToInt32(ConfigurationManager.AppSettings["Generations"]);
            var ruleSize = Convert.ToInt32(ConfigurationManager.AppSettings["RuleSize"]);
            var tournamentSize = Convert.ToInt32(ConfigurationManager.AppSettings["TournamentSize"]);

            geneticAlgorithm.InputData = ArrangeTestingData();

            var dataCount = geneticAlgorithm.InputData.Count;

            var currentPopulation = geneticAlgorithm.CreateInitialPopulation();

            for (var i = 1; i <= generations; i++)
            {
                var tournamentPopulation = geneticAlgorithm.RunSelection(currentPopulation, tournamentSize);

                var offspringPopulation = geneticAlgorithm.CreateOffspring(tournamentPopulation);

                var bestFitness = offspringPopulation.Max(x => x.Fitness);

                if (bestIndividual.Fitness < bestFitness)
                {
                    bestIndividual = offspringPopulation.Aggregate((x, y) => x.Fitness > y.Fitness ? x : y);
                }
                else if (bestIndividual.Fitness > bestFitness)
                {
                    var worstIndividual = offspringPopulation.Aggregate((x, y) => x.Fitness < y.Fitness ? x : y);

                    offspringPopulation.Remove(worstIndividual);
                    offspringPopulation.Add(bestIndividual);

                    bestFitness = offspringPopulation.Max(x => x.Fitness);
                }

                var meanFitness = offspringPopulation.Average(x => x.Fitness);

                Console.WriteLine("\n----- Generation " + i + "-----");
                Console.WriteLine("Best Fitness is: " + bestFitness);
                Console.WriteLine("Mean Fitness is: " + meanFitness);

                fullBuilder.AppendFormat("{0},{1},{2}{3}", i, bestFitness, meanFitness, Environment.NewLine);

                if (bestFitness == dataCount)
                {
                    Console.WriteLine("\n\n************ Optimal Solution Found ************");

                    fullBuilder.AppendFormat("{0}{1}{0}{0}", Environment.NewLine, bestIndividual);

                    break;
                }

                currentPopulation = offspringPopulation;
            }

            bestIndividual.CalculateFitness(testingData, ruleSize);

            Console.WriteLine("\nTests Passed: " + bestIndividual.Fitness + "/" + testingData.Count);

            var percentage = ((double)bestIndividual.Fitness / testingData.Count) * 100;

            ruleBuilder.AppendLine(bestIndividual.ToString());

            testingBuilder.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}", Environment.NewLine, "Tests Passed:",
                bestIndividual.Fitness, "/", testingData.Count, " (", percentage, "%)");
        }
        
        /// <summary>
        /// Changes settings in the app.config file for the program, mainly for quick changing.
        /// </summary>
        private static void InitialiseSettings()
        {
            AppSettingsHelper.UpdateAppSettings("PopulationSize", "120");
            AppSettingsHelper.UpdateAppSettings("Generations", "1500");
            AppSettingsHelper.UpdateAppSettings("CrossoverRate", "90");
            AppSettingsHelper.UpdateAppSettings("TournamentSize", "5");
            AppSettingsHelper.UpdateAppSettings("MutationRate", "0.012");
            AppSettingsHelper.UpdateAppSettings("RuleAmount", "10");
        }

        /// <summary>
        /// Sets up the input data into either the whole set, or a training/testing set.
        /// </summary>
        /// <returns>The input data for training.</returns>
        private static IDictionary<IList<double>, int> ArrangeTestingData()
        {
            var filePath = ConfigurationManager.AppSettings["FilePath"];
            var dataHandler = new DataHandler();

            var trainingData = dataHandler.ReadRealData(Environment.CurrentDirectory + filePath);

            var inputCount = trainingData.Count;

            var testingPoint = RandomHelper.Random.Next(inputCount - (inputCount / 5));

            testingData = trainingData.Skip(testingPoint)
                                    .Take(inputCount / 5)
                                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            foreach (var i in testingData)
            {
                trainingData.Remove(i);
            }

            return trainingData;
        }

        /// <summary>
        /// Performs finishing tasks for the application.
        /// </summary>
        private static void Finalise()
        {
            var dataHandler = new DataHandler();
            var ticks = DateTime.Now.Ticks;

            var writeFull = dataHandler.TryWriteData(fullBuilder.ToString(), @"G:\University\Final Year\Advances in Artificial Intelligence\Assignment\Data3\Results_" + ticks + ".csv");
            var writeTest = dataHandler.TryWriteData(testingBuilder.ToString(), @"G:\University\Final Year\Advances in Artificial Intelligence\Assignment\Data3\TestResult_" + ticks + ".txt");
            var writeRules = dataHandler.TryWriteData(ruleBuilder.ToString(), @"G:\University\Final Year\Advances in Artificial Intelligence\Assignment\Data3\RuleResult_" + ticks + ".csv");
            //var writeFull = dataHandler.TryWriteData(fullBuilder.ToString(), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Results_Data3_" + ticks + ".csv");
            //var writeTest = dataHandler.TryWriteData(testingBuilder.ToString(), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TestResult_Data3_" + ticks + ".txt");
            //var writeRules = dataHandler.TryWriteData(ruleBuilder.ToString(), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\RuleResult_Data3_" + ticks + ".txt");

            if (writeFull && writeTest)
            {
                Console.WriteLine("\nSuccessfully written to file!");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadLine();
        }
    }
}
