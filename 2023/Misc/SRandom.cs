using System;
using System.Collections.Generic;
using System.Linq;

public static class SRandom
{
    public static bool IsLuckyChanceWithPercent(int value, Random random)
    {
        return random.Next(1, 101) <= value ? true : false;
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

    /// <summary>
    /// Reliable algorithm
    /// </summary>
    /// <param name="weights"></param>
    /// <returns></returns>
    public static int GetRandomWeightedIndex(int[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        int t = 0;
        int i;
        int w;
        for (i = 0; i < weights.Length; i++)
        {
            if (weights[i] >= 0) t += weights[i];
        }

        float r = UnityEngine.Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            if (weights[i] <= 0f) continue;

            s += (float)weights[i] / t;
            if (s >= r) return i;
        }

        return -1;
    }

    private static Random rng = new Random();

    // USAGE:
    // List<Product> products = GetProducts();
    // products.Shuffle();
    public static void Shuffle<T>(this IList<T> list, Random random)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static IList<T> Shuffle1<T>(this IList<T> list, Random random)
    {
        return list.OrderBy(x => random.Next()).ToArray();
    }
}