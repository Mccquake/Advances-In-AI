namespace DataSet3b.Helpers
{
    using System;

    /// <summary>
    /// Holds a static instance of the <see cref="Random"/> object for usage.
    /// Useful over instantiating multiple <see cref="Random"/> objects as the seed is based on clock time.
    /// </summary>
    public static class RandomHelper
    {
        private static readonly Random RandomInternal = new Random();

        public static Random Random
        {
            get { return RandomInternal ?? new Random(); }
        }
    }
}