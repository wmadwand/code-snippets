using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniGames.Games.SpaceDefence.Core
{
    public static class SRandom
    {
        /// <summary>
        ///  Return a random int number between min [inclusive] and max [inclusive]
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetInt(int min, int max)
        {
            return rnd.Next(min, max + 1);
        }

        /// <summary>
        /// Return a random double number between min [inclusive] and max [exclusive]
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double GetDouble(double min, double max)
        {
            return rnd.NextDouble() * (max - min) + min;
        }

        public static bool IsLuckyChanceWithPercent(int value/*, Random random*/)
        {
            return rnd.Next(1, 101) <= value ? true : false;
        }

        public static int GetRandomWeightedItemIndex(int[] array, Random random)
        {
            int pick = random.Next(array.Sum());
            int sum = 0;

            for (int i = 0; i < array.Length; i++)
            {
                sum += array[i];
                if (sum >= pick)
                {
                    return i;
                }
            }

            return -1;
        }

        private static Random rnd = new Random();

        // USAGE:
        // List<Product> products = GetProducts();
        // products.Shuffle();
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static IEnumerable<T> Shuffle1<T>(this IEnumerable<T> collection, Random random)
        {
            return collection.OrderBy(x => random.Next()).ToArray();
        }

        public static T GetRandomItem<T>(this IEnumerable<T> collection)
        {
            var randomIndex = rnd.Next(0, collection.Count());
            return collection.ElementAt(randomIndex);
        }
    }
}