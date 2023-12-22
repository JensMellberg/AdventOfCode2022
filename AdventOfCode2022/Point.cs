using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2022
{
	public class Point
	{
		public int X { get; set; }

		public int Y { get; set; }

		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}

		public Point Copy() => new Point(this.X, this.Y);

		public int XDistance(Point other, bool absolute)
		{
			var distance = other.X - this.X;
			return absolute ? Math.Abs(distance) : distance;
		}

		public int YDistance(Point other, bool absolute)
		{
			var distance = other.Y - this.Y;
			return absolute ? Math.Abs(distance) : distance;
		}

		public bool IsBetweenX(Point p1, Point p2)
		{
			Point start;
			Point end;
			if (p1.X < p2.X)
			{
				start = p1;
				end = p2;
			}
			else
			{
				start = p2;
				end = p1;
			}

			return X > start.X || X < end.X;
		}

		public int ManhattanDistance(Point other) => this.XDistance(other, true) + this.YDistance(other, true);

		public override string ToString() => X + "," + Y;

		public static Point FromString(string line)
		{
			var tokens = line.Split(',');
			return new Point(int.Parse(tokens[0]), int.Parse(tokens[1]));
		}
	}
}
