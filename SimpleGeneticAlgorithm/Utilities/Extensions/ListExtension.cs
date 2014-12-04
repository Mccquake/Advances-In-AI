namespace Utilities.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class ListExtension
    {
        /// <summary>
        /// Performs a shuffling operation on the list. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(this IList<T> list)
        {
            var x = list.Count;
            while (x > 1)
            {
                x--;
                var i = new Random().Next(x + 1);
                var value = list[i];
                list[i] = list[x];
                list[x] = value;
            }
        }
    }
}
