using System;
using System.Linq;

public static class SRandom
{
	public static bool IsLuckyChanceWithPercent(int value, Random random)
	{
		return random.Next(1, 101) <= value ? true : false;
	}

	public static int GetWeightedRandomItemIndex(int[] array, Random random)
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
}