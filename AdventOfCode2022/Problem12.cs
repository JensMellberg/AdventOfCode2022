using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem12 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{

			var data = new List<string>(testData);
			var width = data[0].Length;
			var height = data.Count;
			var map = Matrix.FromTestInput<char>(testData);

			var start = map.Find('S');
			var firstResult = ShortestFrom(start);
			this.PrintResult(firstResult);

			var aList = new List<(int x, int y)>();
			for (var x = 0; x < map.RowCount; x++)
			{
				for (var y = 0; y < map.ColumnCount; y++)
				{
					if (map[y, x] == 'a')
					{
						aList.Add((y, x));
					}
				}
			}

			var secondResult = aList.Select(ShortestFrom).Min();
			this.PrintResult(secondResult);

			int ShortestFrom((int x, int y) startIndex)
			{
				var futureVisits = new List<(int x, int y)>();
				var distance = Matrix.InitWithStartValue(height, width, int.MaxValue);
				distance[startIndex.x, startIndex.y] = 0;

				futureVisits.Add(startIndex);
				while (futureVisits.Any())
				{
					var ordered = futureVisits.OrderBy(x => distance[x.x, x.y]);
					var current = ordered.First();
					futureVisits.Remove(current);
					var currentChar = map[current.x, current.y];
					tryAddToQueue(current.x - 1, current.y);
					tryAddToQueue(current.x + 1, current.y);
					tryAddToQueue(current.x, current.y - 1);
					tryAddToQueue(current.x, current.y + 1);

					void tryAddToQueue(int x, int y)
					{
						if (x >= 0 && x < width && y >= 0 && y < height && distance[x, y] == int.MaxValue && (map[x, y] <= currentChar + 1 || map[x, y] == 'E' || currentChar == 'S'))
						{
							futureVisits.Add((x, y));
							distance[x, y] = distance[current.x, current.y] + 1;
						}
					}
				}

				var endIndex = map.Find('E');
				return distance[endIndex.x, endIndex.y];
			}
		}
	}
}
