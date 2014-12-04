namespace SimpleGeneticAlgorithm
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Text;

    using SimpleGeneticAlgorithm.Enumerations;
    using SimpleGeneticAlgorithm.Helpers;
    using SimpleGeneticAlgorithm.Models;

    using Utilities.Data;
    using Utilities.Helpers;

    public class Program
    {
        private static DataHandler dataHandler;

        private static StringBuilder stringBuilder;

        private static StringBuilder testingBuilder;

        private static StringBuilder ruleBuilder;

        private static bool toBeTested;

        private static InputFile type;

        private static IDictionary<IList<string>, int> testingData;

        /// <summary>
        /// Initial Setup.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Initialise();

            dataHandler = new DataHandler();
            stringBuilder = new StringBuilder();
            testingBuilder = new StringBuilder();
            ruleBuilder = new StringBuilder();

            var runAmount = 7;

            while (runAmount > 0)
            {
                runAmount--;
                var geneticAlgorithm = new GeneticAlgorithm();
                var bestIndividual = new Individual {Fitness = 0};

                var generations = Convert.ToInt32(ConfigurationManager.AppSettings["Generations"]);
                var ruleSize = Convert.ToInt32(ConfigurationManager.AppSettings["RuleSize"]);
                var tournSize = Convert.ToInt32(ConfigurationManager.AppSettings["TournamentSize"]);

                geneticAlgorithm.InputData = ArrangeInputData();

                var dataCount = geneticAlgorithm.InputData.Count;

                var currentPopulation = geneticAlgorithm.CreateInitialPopulation();

                for (var i = 1; i <= generations; i++)
                {
                    var tournamentPopulation = geneticAlgorithm.RunTournamentSelection(currentPopulation, tournSize);

                    var offspringPopulation = geneticAlgorithm.BreedOffspring(tournamentPopulation);

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

                    stringBuilder.AppendFormat("{0},{1},{2}{3}", i, bestFitness, meanFitness, Environment.NewLine);

                    if (bestFitness == dataCount)
                    {
                        Console.WriteLine("\n\n************ Optimal Solution Found ************");

                        ruleBuilder.AppendFormat("{1},{0}{2}", bestFitness, i, Environment.NewLine);

                        break;
                    }

                    currentPopulation = offspringPopulation;
                }

                if (toBeTested)
                {
                    bestIndividual.CalculateFitness(testingData, ruleSize);

                    Console.WriteLine("\nTests Passed: " + bestIndividual.Fitness + "/" + testingData.Count);

                    stringBuilder.AppendFormat("{0}{1}{2}{3}{4}", Environment.NewLine, "Tests Passed:",
                        bestIndividual.Fitness, "/", testingData.Count);

                    var percentage = ((double)bestIndividual.Fitness / testingData.Count) * 100;

                    testingBuilder.AppendFormat("{0}{1}{2}{3}{4}{5}{6}{7}", Environment.NewLine, "Tests Passed:",
                        bestIndividual.Fitness, "/", testingData.Count, " (", percentage, "%)");
                }

                ruleBuilder.AppendLine(bestIndividual.ToString());

                stringBuilder.AppendLine();
                Console.WriteLine();
            }

            Finalise();
        }

        /// <summary>
        /// Arranges the constant values required for each data file based on user input.
        /// </summary>
        private static void Initialise()
        {
            Console.WriteLine("Which data set are you intending to use:");
            Console.WriteLine("1 or 2?");

            var input = Console.ReadLine();
            switch (input)
            {
                case "2":
                    type = InputFile.Data2;
                    break;
                default:
                    type = InputFile.Data1;
                    break;
            }

            switch (type)
            {
                case InputFile.Data1:
                    AppSettingsHelper.UpdateAppSettings("RuleSize", "6");
                    AppSettingsHelper.UpdateAppSettings("FilePath", @"\Input\data1.txt");
                    AppSettingsHelper.UpdateAppSettings("PopulationSize", "50");
                    AppSettingsHelper.UpdateAppSettings("Generations", "250");
                    AppSettingsHelper.UpdateAppSettings("CrossoverRate", "90");
                    AppSettingsHelper.UpdateAppSettings("MutationRate", "0.02");
                    AppSettingsHelper.UpdateAppSettings("TournamentSize", "2");
                    AppSettingsHelper.UpdateAppSettings("RuleAmount","6");
                    break;
                case InputFile.Data2:
                    AppSettingsHelper.UpdateAppSettings("RuleSize", "11");
                    AppSettingsHelper.UpdateAppSettings("FilePath", @"\Input\data2.txt");
                    AppSettingsHelper.UpdateAppSettings("PopulationSize", "70");
                    AppSettingsHelper.UpdateAppSettings("Generations", "1000");
                    AppSettingsHelper.UpdateAppSettings("CrossoverRate", "90");
                    AppSettingsHelper.UpdateAppSettings("MutationRate", "0.01");
                    AppSettingsHelper.UpdateAppSettings("TournamentSize", "12");
                    break;
            }
        }

        /// <summary>
        /// Sets up the input data into either the whole set, or a training/testing set.
        /// </summary>
        /// <returns>The input data for training.</returns>
        private static IDictionary<IList<string>, int> ArrangeInputData()
        {
            IDictionary<IList<string>, int> inputData = null;

            var filePath = ConfigurationManager.AppSettings["FilePath"];

            inputData = dataHandler.ReadBinaryData(Environment.CurrentDirectory + filePath);

            var inputCount = inputData.Count;

            var input = type == InputFile.Data1 ? "1" : "2";

            switch (input)
            {
                case "1":
                    break;
                default:
                    var testingPoint = RandomHelper.Random.Next() % (inputCount - (inputCount / 5));

                    testingData = inputData.Skip(testingPoint)
                                           .Take(inputCount / 5)
                                           .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

                    foreach (var i in testingData)
                    {
                        inputData.Remove(i);
                    }

                    toBeTested = true;
                    break;
            }

            return inputData;
        }

        /// <summary>
        /// Performs finishing tasks for the application.
        /// </summary>
        private static void Finalise()
        {
            var ticks = DateTime.Now.Ticks;

            var writeStatus = dataHandler.TryWriteData(stringBuilder.ToString(), @"G:\University\Final Year\Advances in Artificial Intelligence\Assignment\Data2" + @"\Results_" + ticks + ".csv");
            var ruleStatus = dataHandler.TryWriteData(ruleBuilder.ToString(), @"G:\University\Final Year\Advances in Artificial Intelligence\Assignment\Data2" + @"\RuleResults_" + ticks + ".csv");
            //var writeStatus = dataHandler.TryWriteData(stringBuilder.ToString(), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Results_" + type + "_" + ticks + ".csv");
            //var ruleStatus = dataHandler.TryWriteData(ruleBuilder.ToString(), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Results_" + type + "_" + ticks + ".csv");

            if (toBeTested)
            {
                dataHandler.TryWriteData(testingBuilder.ToString(), @"G:\University\Final Year\Advances in Artificial Intelligence\Assignment\Data2\TestResults_" + ticks + ".txt");
                //dataHandler.TryWriteData(testingBuilder.ToString(), Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\TestResults_" + type + "_" + ticks + ".txt");
            }

            if (writeStatus)
            {
                Console.WriteLine("\nSuccessfully written to file!");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadLine();
        }
    }
}
