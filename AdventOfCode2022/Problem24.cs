using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022
{
	public class Problem24 : StringProblem
	{
		public static Point FinishPoint; 

		public int Width { get; set; }

		public int Height { get; set; }

		public override void Solve(IEnumerable<string> testData)
		{
			var input = testData.Skip(1).SkipLast(1).Select(x => x.Substring(1, x.Length - 2)).ToList();
			this.Height = input.Count;
			this.Width = input[0].Length;
			FinishPoint = new Point(Width - 1, Height - 1);
			var initialField = Matrix.InitWithStartValue(Height, Width, true);
			List<Blizzard> Blizzards = new List<Blizzard>();
			for (var y = 0; y < input.Count; y++)
			{
				for (var x = 0; x < input[y].Length; x++)
				{
					if (input[y][x] != '.')
					{
						Blizzards.Add(new Blizzard(input[y][x], new Point(x, y)));
						initialField[x, y] = false;
					}
				}
			}

			this.FieldByDay[0] = initialField;
			var firstTrip = this.SolveOnce(Blizzards, false, 0);
			this.PrintResult(firstTrip);
			var secondTrip = this.SolveOnce(Blizzards, true, firstTrip + 1);
			var thirdTrip = this.SolveOnce(Blizzards, false, secondTrip + 1);
			this.PrintResult(thirdTrip);
		}

		public int SolveOnce(List<Blizzard> blizzards, bool startAtGoal, int time)
		{
			var queue = new SortedSet<QueueItem>(new Comparer());
			var visited = new HashSet<string>();
			queue.Add(new QueueItem { Point = null, Time = time + 1 });
			var initialField = this.GetFieldForDay(time, blizzards);
			var startX = startAtGoal ? Width - 1 : 0;
			var startY = startAtGoal ? Height - 1 : 0;
			var endX = startAtGoal ? 0 : Width - 1;
			var endY = startAtGoal ? 0 : Height - 1;

			if (initialField[startX, startY])
			{
				queue.Add(new QueueItem { Point = new Point(startX, startY), Time = time + 1 });
			}

			while (queue.Any())
			{
				var current = queue.First();
				queue.Remove(current);
				var day = current.Time;
				var position = current.Point;
				var field = this.GetFieldForDay(day, blizzards);
				var key = (position ?? new Point(0, -1)).ToString() + "," + day;
				if (visited.Contains(key))
				{
					continue;
				}

				visited.Add(key);
				if (position == null)
				{
					if (!startAtGoal && field[startX, startY])
					{
						queue.Add(new QueueItem { Point = new Point(startX, startY), Time = day + 1 });
					}

					queue.Add(new QueueItem { Point = null, Time = day + 1 });
					continue;
				}

				if (position.X == endX && position.Y == endY)
				{
					return day;
				}

				AddIfFree(-1, 0);
				AddIfFree(1, 0);
				AddIfFree(0, -1);
				AddIfFree(0, 1);
				AddIfFree(0, 0);

				void AddIfFree(int dX, int dY)
				{
					var newPoint = new Point(position.X + dX, position.Y + dY);
					if (field.IsInBounds(newPoint.X, newPoint.Y)
						&& field[newPoint.X, newPoint.Y])
					{
						queue.Add(new QueueItem { Point = newPoint, Time = day + 1 });
					}
				}
			}

			throw new Exception("no path reached the end.");
		}

		private Dictionary<int, Matrix<bool>> FieldByDay = new Dictionary<int, Matrix<bool>>();

		private Matrix<bool> GetFieldForDay(int day, List<Blizzard> blizzards)
		{
			if (FieldByDay.TryGetValue(day, out var val))
			{
				return val;
			}

			var field = Matrix.InitWithStartValue(Height, Width, true);
			blizzards.ForEach(x => x.Move(field));
			FieldByDay.Add(day, field);
			return field;
		}

		private class Comparer : IComparer<QueueItem>
		{
			public int Compare(QueueItem x, QueueItem y)
			{
				return x.CompareTo(y);
			}
		}

		private class QueueItem : IComparable
		{
			public int Time { get; set; }

			public Point Point { get; set; }

			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 1;
				}
				if (this.Equals(obj))
				{
					return 0;
				}

				var other = obj as QueueItem;

				var dist = (this.Point ?? new Point(0, 0)).ManhattanDistance(FinishPoint) + Time;
				var otherDist = (other.Point ?? new Point(0, 0)).ManhattanDistance(FinishPoint) + other.Time;
				var result = dist.CompareTo(otherDist);
				return result == 0 ? -1 : result;
			}

			public override bool Equals(object obj)
			{
				var other = obj as QueueItem;
				var thisPoint = this.Point ?? new Point(-500, -500);
				var otherPoint = other.Point ?? new Point(-500, -500);
				return thisPoint.X == otherPoint.X
					&& thisPoint.Y == otherPoint.Y
					&& this.Time == other.Time;
			}
		}
	}

	
	public class Blizzard
	{
		public Point Position { get; set; }

		private char Direction { get; set; }

		public Blizzard(char c, Point position)
		{
			this.Direction = c;
			this.Position = position;
		}

		public void Move(Matrix<bool> field)
		{
			var deltaX = 0;
			var deltaY = 0;
			switch (this.Direction)
			{
				case '^': deltaY = -1; break;
				case 'v': deltaY = 1; break;
				case '>': deltaX = 1; break;
				case '<': deltaX = -1; break;
				default: throw new Exception("Unknown input");
			}

			this.Position.X += deltaX;
			this.Position.Y += deltaY;
			if (this.Position.X < 0)
			{
				this.Position.X = field.ColumnCount - 1;
			}
			else if (this.Position.X >= field.ColumnCount)
			{
				this.Position.X = 0;
			}
			else if (this.Position.Y < 0)
			{
				this.Position.Y = field.RowCount - 1;
			}
			else if (this.Position.Y >= field.RowCount)
			{
				this.Position.Y = 0;
			}

			field[this.Position.X, this.Position.Y] = false;
		}
	}

}
