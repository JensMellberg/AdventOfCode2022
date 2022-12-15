using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem14 : ObjectProblem<StoneLine>
	{
		public override void Solve(IEnumerable<StoneLine> testData)
		{
			var input = testData.ToList();
			var field = Matrix.InitWithStartValue(180, 1000, false);
			input.ForEach(x => x.DrawRocks(field));
			var counter = 0;
			while (SpawnSand(field))
			{
				counter++;
			}

			this.PrintResult(counter);
			var maxY = input.Max(x => x.Corners.Max(x => x.Y));
			for (var i = 0; i < field.ColumnCount; i++)
			{
				field[i, maxY + 2] = true;
			}

			while (!field[500, 0])
			{
				SpawnSand(field);
				counter++;
			}
			this.PrintResult(counter);
		}

		private bool SpawnSand(Matrix<bool> field)
		{
			var x = 500;
			var y = 0;
			while (y < field.RowCount - 1)
			{
				if (TryPlace(x, y + 1))
				{
					continue;
				}

				if (TryPlace(x - 1, y + 1))
				{
					continue;
				}

				if (TryPlace(x + 1, y + 1))
				{
					continue;
				}

				field[x, y] = true;
				return true;
			}

			return false;

			bool TryPlace(int xPos, int yPos)
			{
				if (!field[xPos, yPos])
				{
					x = xPos;
					y = yPos;
					return true;
				}

				return false;
			}
		}
	}

	public class StoneLine : Parsable
	{
		public List<Point> Corners;

		public override void ParseFromLine(string line)
		{
			var tokens = line.Split(" -> ");
			this.Corners = tokens.Select(x => x.Split(',')).Select(x => new Point(int.Parse(x[0]), int.Parse(x[1]))).ToList();
			base.ParseFromLine(line);
		}

		public void DrawRocks(Matrix<bool> field)
		{
			for (var i = 0; i < Corners.Count - 1; i++)
			{
				var current = Corners[i].Copy();
				var next = Corners[i + 1].Copy();
				var deltaX = Normalize(current.X, next.X);
				var deltaY = Normalize(current.Y, next.Y);
				field[current.X, current.Y] = true;
				while (current.X != next.X || current.Y != next.Y)
				{
					current.X += deltaX;
					current.Y += deltaY;
					field[current.X, current.Y] = true;
				}
			}

			int Normalize(int a, int b)
			{
				if (a < b)
				{
					return 1;
				}
				else if (a == b)
				{
					return 0;
				}

				return -1;
			}
		}
	}
}
