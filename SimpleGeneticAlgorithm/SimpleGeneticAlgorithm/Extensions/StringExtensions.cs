namespace SimpleGeneticAlgorithm.Extensions
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Holds all extensions to the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string into an IEnumerable of substrings, of a given length.
        /// </summary>
        /// <param name="str">Input string to be split.</param>
        /// <param name="size">Length of each substring.</param>
        /// <returns>IEnumerable containing each substring as a new entry.</returns>
        public static IEnumerable<string> SubstringSplit(this string str, int size)
        {
            return Enumerable.Range(0, str.Length / size)
                             .Select(x => str.Substring(x * size, size));
        }
    }
}
