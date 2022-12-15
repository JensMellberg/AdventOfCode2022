using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem15 : ObjectProblem<Sensor>
	{
		public override void Solve(IEnumerable<Sensor> testData)
		{
			const int yLine = 2000000;
			var result = new List<int>();
			var allIndices = testData.Select(x => x.InvalidIndices(yLine)).Aggregate((a, b) => a.Concat(b).ToList()).Distinct().ToList();
			testData.Select(x => x.ClosestBeacon).Where(x => x.Y == yLine).ForEach(x => allIndices.Remove(x.X));
			this.PrintResult(allIndices.Count());
			foreach (var s in testData)
			{
				var potentialPoints = s.PotentialPoints();
				var afterFilter = FilterPoints(potentialPoints, testData.Where(x => x != s));
				if (afterFilter != null)
				{
					this.PrintResult((long)afterFilter.X * 4000000 + afterFilter.Y);
					break;
				}
			}
		}

		private Point FilterPoints(List<Point> points, IEnumerable<Sensor> others)
		{
			foreach (var p in points)
			{
				var found = false;
				foreach (var s in others)
				{
					if (p.ManhattanDistance(s.Position) <= s.DistanceToClosestBeacon)
					{
						found = true;
						break;
					}
				}

				if (!found)
				{
					return p;
				}
			}

			return null;
		}
	}

	public class Sensor : Parsable
	{
		public Point Position { get; set; }

		public Point ClosestBeacon { get; set; }

		public override void ParseFromLine(string line)
		{
			var tokens = line.Replace(",", "").Replace(":", "").Split(" ");
			this.Position = new Point(ReadNumber(tokens[2]), ReadNumber(tokens[3]));
			this.ClosestBeacon = new Point(ReadNumber(tokens[8]), ReadNumber(tokens[9]));
			base.ParseFromLine(line);

			static int ReadNumber(string exp) => int.Parse(exp.Split('=')[1]);
		}

		public int DistanceToClosestBeacon => this.Position.ManhattanDistance(this.ClosestBeacon);

		public List<int> InvalidIndices(int yLine)
		{
			var result = new List<int>();
			var distToLine = Math.Abs(this.Position.Y - yLine);
			var remainingDist = this.DistanceToClosestBeacon - distToLine;
			
			for (var i = this.Position.X - remainingDist; i <= this.Position.X + remainingDist; i++)
			{
				result.Add(i);
			}

			return result;
		}

		public List<Point> PotentialPoints()
		{
			var result = new List<Point>();
			var x = this.Position.X;
			var y = this.Position.Y - this.DistanceToClosestBeacon - 1;
			TryAdd(x, y);
			while (y < this.Position.Y)
			{
				x++;
				y++;
				TryAdd(x, y);
			}

			while (y < this.Position.Y + this.DistanceToClosestBeacon + 1)
			{
				x--;
				y++;
				TryAdd(x, y);
			}

			while (y > this.Position.Y)
			{
				x--;
				y--;
				TryAdd(x, y);
			}

			while (y > this.Position.Y - this.DistanceToClosestBeacon)
			{
				x++;
				y--;
				TryAdd(x, y);
			}

			void TryAdd(int xx, int yy)
			{
				if (xx < 0 || xx > 4000000 || yy < 0 || yy > 4000000)
				{
					return;
				}

				result.Add(new Point(xx, yy));
			}

			return result;
		}
	}
}
