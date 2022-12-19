using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2022
{
	public class Problem18 : ObjectProblem<Cube>
	{
		public override void Solve(IEnumerable<Cube> testData)
		{
			var input = testData.ToList();
			var total = 0;
			var adjacentCount = 0;
			for (var i = 0; i < input.Count; i++)
			{
				var cube = input[i];
				for (var x = i + 1; x < input.Count; x++)
				{
					if (cube.IsAdjacent(input[x]))
					{
						adjacentCount++;
					}
				}

				total += 6;
			}

			total -= adjacentCount * 2;
			this.PrintResult(total);

			var field = new Dictionary<string, int>();
			input.ForEach(x => field.Add(x.ToString(), 1));
			var queue = new Queue<string>();
			FillWithSteam(0, 0, 0);
			while (queue.Any())
			{
				var tokens = queue.Dequeue().Split(",");
				FillWithSteam(int.Parse(tokens[0]), int.Parse(tokens[1]), int.Parse(tokens[2]));
			}
			var adjacentCount2 = 0;

			for (var x = 0; x < 22; x++)
			{
				for (var y = 0; y < 22; y++)
				{
					for (var z = 0; z < 22; z++)
					{
						if (!field.ContainsKey(Stringify(x, y, z)))
						{
							var cube = new Cube { X = x, Y = y, Z = z };
							for (var f = 0; f < input.Count; f++)
							{
								if (cube.IsAdjacent(input[f]))
								{
									adjacentCount2++;
								}
							}
						}
					}
				}
			}

			this.PrintResult(total - adjacentCount2);

			void FillWithSteam(int x, int y, int z)
			{
				if (x < 0 || x > 21 || y < 0 || y > 21 || z < 0 || z > 21)
				{
					return;
				}

				var key = Stringify(x, y, z);
				if (!field.TryGetValue(key, out var value))
				{
					field.Add(key, 2);
					queue.Enqueue(Stringify(x + 1, y, z));
					queue.Enqueue(Stringify(x - 1, y, z));
					queue.Enqueue(Stringify(x, y + 1, z));
					queue.Enqueue(Stringify(x, y - 1, z));
					queue.Enqueue(Stringify(x, y, z + 1));
					queue.Enqueue(Stringify(x, y, z - 1));
				} 
			}

			string Stringify(int x, int y, int z)
			{
				return $"{x},{y},{z}";
			}
		}
	}

	public class Cube : Parsable
	{
		public int X { get; set; }

		public int Y { get; set; }

		public int Z { get; set; }


		public override void ParseFromLine(string line)
		{
			var tokens = line.Split(",");
			this.X = int.Parse(tokens[0]);
			this.Y = int.Parse(tokens[1]);
			this.Z = int.Parse(tokens[2]);
			base.ParseFromLine(line);
		}

		public bool IsAdjacent(Cube other)
		{
			if (Math.Abs(this.X - other.X) == 1 && this.Y == other.Y && this.Z == other.Z)
			{
				return true;
			}

			if (Math.Abs(this.Y - other.Y) == 1 && this.X == other.X && this.Z == other.Z)
			{
				return true;
			}

			return Math.Abs(this.Z - other.Z) == 1 && this.X == other.X && this.Y == other.Y;
		}

		public override string ToString()
		{
			return this.originalLine;
		}
	}

}
