using System;
using System.Collections.Generic;

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
    }
}
