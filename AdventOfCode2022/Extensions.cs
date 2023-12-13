using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AdventOfCode2022
{
	public static class Extensions
	{
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (var item in enumerable)
			{
				action(item);
			}
		}

		public static void Print<T>(this IEnumerable<T> enumerable)
		{
			Console.WriteLine(string.Join(',', enumerable));
		}

		public static string ReplaceAtIndex(this string self, int index, char replacement)
		{
			return self.Substring(0, index) + replacement + self[(index + 1)..];
		}

		/*public static List<List<T>> GetPermutations<T>(this IList<T> list)
		{
			var result = new List<List<T>>();
			GetPermutations(list, result, 0, list.Count - 1);
			return result;
		}

		private static void GetPermutations<T>(List<T> original, List<List<T>> result, int firstIndex, int lastIndex)
		{
			if (firstIndex == lastIndex)
			{
				result.Add(new List<T>(original));
			}
			else
			{
				for (int i = firstIndex; i <= lastIndex; i++)
				{
					Swap(ref original[firstIndex], ref original[i]);
                    GetPermutations(original, result, firstIndex + 1, lastIndex);
					Swap(ref original[firstIndex], ref original[i]);
				}
			}

			void Swap(ref T a, ref T b)
			{
                var temp = a;
				a = b;
				b = temp;
			}
		}*/
	}
}
