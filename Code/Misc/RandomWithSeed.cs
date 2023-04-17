using System;

namespace Rosie.Code.Misc
{
    /// <summary>
    /// The sole random number generator used by this game
    /// </summary>
    public static class RandomWithSeed
    {

        /// <summary>
        /// Generate a random seed in the format xxxx-xxxx-xxxx where x is a to z
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomSeed()
        {
            var random = new Random();
            var seedBytes = new byte[12];
            random.NextBytes(seedBytes);

            var seedChars = new char[12];
            for (int i = 0; i < seedBytes.Length; i++)
            {

                seedChars[i] = GetRandomLetter(random);

            }

            return new string(seedChars, 0, 4) + "-" +
                   new string(seedChars, 4, 4) + "-" +
                   new string(seedChars, 8, 4);
        }


        private static char GetRandomLetter(Random random)
        {
            const string letters = "abcdefghijklmnopqrstuvwxyz";
            int index = random.Next(letters.Length);
            return letters[index];
        }


        private static Random _random;

        public static void SetSeed(string seed)
        {
            // Remove any non-alphanumeric characters from the seed and convert to bytes
            var seedBytes = new byte[seed.Length];
            for (int i = 0; i < seed.Length; i++)
            {
                seedBytes[i] = (byte)char.ToUpperInvariant(seed[i]);
            }

            // Initialize the random number generator with the seed bytes
            _random = new Random(seedBytes.GetHashCode());
        }

        public static int Next()
        {
            return _random.Next();
        }

        public static int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public static double NextDouble()
        {
            return _random.NextDouble();
        }
    }

}
