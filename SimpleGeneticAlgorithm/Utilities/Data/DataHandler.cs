namespace Utilities.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using Utilities.Interfaces;

    public class DataHandler : IDataHandler
    {
        /// <summary>
        /// Reads in appropriate data and outputs as required.
        /// </summary>
        /// <param name="path">The path of the file to be read in.</param>
        /// <returns>Dictionary that holds all of the relevant data.</returns>
        public IDictionary<IList<string>, int> ReadBinaryData(string path)
        {
            if(!FileOrDirectoryExists(path))
            {
                throw new IOException("The file cannot be found at: " + path);
            }

            var inputData = new Dictionary<IList<string>, int>();

            using(var reader = new StreamReader(path))
            {
                while(!reader.EndOfStream)
                {
                    var inputLine = reader.ReadLine();

                    var splitString = inputLine.Split(' ');

                    var key = new List<string>();

                    key.AddRange(splitString[0].Select(x => x.ToString(CultureInfo.CurrentCulture)));

                    var value = int.Parse(splitString[1]);

                    inputData.Add(key, value);
                }
            }
            return inputData;
        }

        /// <summary>
        /// Reads in appropriate real data relating to data set 3.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Dictionary containing all data separated into input/output.</returns>
        public IDictionary<IList<double>, int> ReadRealData(string path)
        {
            if (!FileOrDirectoryExists(path))
            {
                throw new IOException("The file cannot be found at: " + path);
            }

            var inputData = new Dictionary<IList<double>, int>();

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var inputLine = reader.ReadLine();

                    var splitString = inputLine.Split(' ');

                    var key = splitString.Take(6).Select(Double.Parse).ToList();

                    var value = int.Parse(splitString[6]);

                    inputData.Add(key, value);
                }
            }
            return inputData;
        }

        /// <summary>
        /// Reads in appropriate real data, and rounds it to the nearest whole number.
        /// </summary>
        /// <param name="path">Path to the file.</param>
        /// <returns>Dictionary containing all data separated into input/output.</returns>
        public IDictionary<IList<double>, int> ReadRoundedRealData(string path)
        {
            if (!FileOrDirectoryExists(path))
            {
                throw new IOException("The file cannot be found at: " + path);
            }

            var inputData = new Dictionary<IList<double>, int>();

            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var inputLine = reader.ReadLine();

                    var splitString = inputLine.Split(' ');

                    var key = splitString.Take(6).Select(x => Math.Round(Double.Parse(x))).ToList();

                    var value = int.Parse(splitString[6]);

                    inputData.Add(key, value);
                }
            }
            return inputData;
        }

        /// <summary>
        /// Very basic data writing method that handles exceptions.
        /// </summary>
        /// <param name="data">Data to be output to the stream.</param>
        /// <param name="path">Path of the output file.</param>
        /// <returns>Success status</returns>
        public bool TryWriteData(string data, string path)
        {
            try
            {
                using (var writer = new StreamWriter(path))
                {
                    writer.WriteLine(data);
                }
            }
            catch(Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Internal check for the existence of files and directories.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Whether there is an error or not</returns>
        private static bool FileOrDirectoryExists(string name)
        {
            return (Directory.Exists(name) || File.Exists(name));
        }
    }
}
