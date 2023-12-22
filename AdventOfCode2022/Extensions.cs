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

		public static T FindMin<T>(this IEnumerable<T> enumerable, Func<T, long> predicate)
		{
			var min = default(T);
			var minValue = long.MaxValue;
			foreach (var item in enumerable)
			{
				var value = predicate(item);
                if (value < minValue)
				{
					min = item;
					minValue = value;
				}
			}

			return min;
		}

		public static string ReplaceAtIndex(this string self, int index, char replacement)
		{
			return self.Substring(0, index) + replacement + self[(index + 1)..];
		}

		public static bool IsHorizontal(this Direction self) => self == Direction.Left || self == Direction.Right;

		public static bool IsVertical(this Direction self) => !self.IsHorizontal();

		public static Direction Reverse(this Direction self)
		{
            return self switch
            {
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                _ => default,
            };
        }

		public static (int x, int y) GetDelta(this Direction self)
		{
            return self switch
            {
                Direction.Left => (-1, 0),
                Direction.Right => (1, 0),
                Direction.Up => (0, -1),
                Direction.Down => (0, 1),
                _ => default,
            };
        }
    }
}
