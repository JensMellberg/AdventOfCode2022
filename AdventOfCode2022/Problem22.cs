using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem22 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var inputMaze = testData.ToList();
			var instructions = ParseInstructions(inputMaze.Last());
			inputMaze.RemoveAt(inputMaze.Count - 1);
			var maxLength = inputMaze.Max(x => x.Length);
			for (var i = 0; i < inputMaze.Count; i++)
			{
				if (inputMaze[i].Length < maxLength)
				{
					inputMaze[i] = inputMaze[i] + string.Concat(Enumerable.Repeat(' ', maxLength - inputMaze[i].Length));
				}
			}

			var startX = inputMaze[0].IndexOf('.');
			var player = new Player() { Position = new Point(startX, 0) };
			var field = Matrix.FromTestInput<int>(inputMaze, (c) =>
			{
				return c switch
				{
					' ' => 0,
					'.' => 1,
					'#' => 2,
					_ => throw new Exception($"Unexpected input char {c}"),
				};
			});

			instructions.ForEach(x => x.Perform(player, field));

			this.PrintResult(player.Score);
			this.Part2(inputMaze, instructions);
		}

		private void GetSides(IList<string> input, bool isTest, out Side bottom, out Side back, out Side right, out Side front, out Side left, out Side top)
		{
			var SideLength = isTest ? 4 : 50;
			if (isTest)
			{
				bottom = ParseSide(SideLength, SideLength);
				back = ParseSide(SideLength, 0);
				right = ParseSide(SideLength * 2, 0);
				front = ParseSide(SideLength, SideLength * 2);
				left = ParseSide(0, SideLength * 2);
				top = ParseSide(0, SideLength * 3);
			}
			else
			{
				bottom = ParseSide(SideLength, SideLength);
				back = ParseSide(SideLength, 0);
				right = ParseSide(SideLength * 2, 0);
				front = ParseSide(SideLength, SideLength * 2);
				left = ParseSide(0, SideLength * 2);
				top = ParseSide(0, SideLength * 3);
			}

			back.RightSide = right;
			back.RightEntry = Direction.Left;
			back.LeftSide = left;
			back.LeftEntry = Direction.Left;
			back.UpSide = top;
			back.UpEntry = Direction.Left;
			back.DownSide = bottom;
			back.DownEntry = Direction.Up;

			right.LeftSide = back;
			right.LeftEntry = Direction.Right;
			right.RightSide = front;
			right.RightEntry = Direction.Right;
			right.DownSide = bottom;
			right.DownEntry = Direction.Right;
			right.UpSide = top;
			right.UpEntry = Direction.Down;

			front.RightSide = right;
			front.RightEntry = Direction.Right;
			front.LeftSide = left;
			front.LeftEntry = Direction.Right;
			front.UpSide = bottom;
			front.UpEntry = Direction.Down;
			front.DownSide = top;
			front.DownEntry = Direction.Right;

			left.RightSide = front;
			left.RightEntry = Direction.Left;
			left.LeftSide = back;
			left.LeftEntry = Direction.Left;
			left.UpSide = bottom;
			left.UpEntry = Direction.Left;
			left.DownSide = top;
			left.DownEntry = Direction.Up;

			bottom.UpSide = back;
			bottom.UpEntry = Direction.Down;
			bottom.RightSide = right;
			bottom.RightEntry = Direction.Down;
			bottom.LeftSide = left;
			bottom.LeftEntry = Direction.Up;
			bottom.DownSide = front;
			bottom.DownEntry = Direction.Up;
			
			top.UpSide = left;
			top.UpEntry = Direction.Down;
			top.DownSide = right;
			top.DownEntry = Direction.Up;
			top.RightSide = front;
			top.RightEntry = Direction.Down;
			top.LeftSide = back;
			top.LeftEntry = Direction.Up;

			Side ParseSide(int startX, int startY)
			{
				var array = new int[SideLength, SideLength];
				for (var x = startX; x < startX + SideLength; x++)
				{
					for (var y = startY; y < startY + SideLength; y++)
					{
						array[x - startX, y - startY] = FromChar(input[y][x]);
					}
				}

				return new Side(new Matrix<int>(array), startX, startY);
			}

			int FromChar(char c)
			{
				return c switch
				{
					' ' => 0,
					'.' => 1,
					'#' => 2,
					_ => throw new Exception($"Unexpected input char {c}"),
				};
			}
		}

		private void Part2(IList<string> input, IEnumerable<Instruction> instructions)
		{
			var isTest = false;
			GetSides(input, isTest, out var bottom, out var back, out var right, out var front, out var left, out var top);
			var startX = 0;
			var player = new Player() { Position = new Point(startX, 0) };
			player.CurrentSide = back;
			instructions.ForEach(x => DoInstruction(x));
			//instructions.ForEach(x =>
			//{
			//	//DoInstruction(x);
			//	if (x is StepInstruction s)
			//	{
			//		DoInstruction(x);
			//		/*var toPrint = player.Direction.ToString() + ": " + s.Steps;
			//		DoInstruction(x);
			//		var field2 = player.CurrentSide.field;
			//		var old = field2[player.Position.X, player.Position.Y];
			//		field2[player.Position.X, player.Position.Y] = 0;
			//		this.Print(field2.ToString((x) => x == 0 ? "P" : (x == 1 ? "." : "#"), ""));
			//		field2[player.Position.X, player.Position.Y] = old;
			//		this.Print(toPrint);
			//		Console.ReadLine();*/
			//	}
			//	else
			//	{
			//		DoInstruction(x);
			//	}
			//});
			var score = (player.Position.Y + 1 + player.CurrentSide.StartY) * 1000 +
				(player.Position.X + 1 + player.CurrentSide.StartX) * 4 + (int)player.Direction;
			this.PrintResult(score);

			void DoInstruction(Instruction i)
			{
				if (i is TurnInstruction)
				{
					i.Perform(player, null);
					return;
				}

				var stepInstruction = (StepInstruction)i;
				for (var f = 0; f < stepInstruction.Steps; f++)
				{
					var delta = player.Delta();
					var field = player.CurrentSide.field;

					if (field.IsInBounds(player.Position.X + delta.x, player.Position.Y + delta.y)
						&& field[player.Position.X + delta.x, player.Position.Y + delta.y] == 2)
					{
						return;
					}
					else if (field.IsInBounds(player.Position.X + delta.x, player.Position.Y + delta.y)
						&& field[player.Position.X + delta.x, player.Position.Y + delta.y] == 1)
					{
						player.Position.X += delta.x;
						player.Position.Y += delta.y;
					}
					else
					{
						Side target = null;
						Direction entry = Direction.Left;
						Direction newDirection = Direction.Left;
						int entryIndex = 0;
						Point newPoint = null;
						if (player.Position.X + delta.x < 0)
						{
							target = player.CurrentSide.LeftSide;
							entryIndex = player.Position.Y;
							entry = player.CurrentSide.LeftEntry;
						}
						else if (player.Position.X + delta.x >= field.ColumnCount)
						{
							target = player.CurrentSide.RightSide;
							entryIndex = player.Position.Y;
							entry = player.CurrentSide.RightEntry;
						}
						else if (player.Position.Y + delta.y >= field.ColumnCount)
						{
							target = player.CurrentSide.DownSide;
							entryIndex = player.Position.X;
							entry = player.CurrentSide.DownEntry;
						}
						else if (player.Position.Y + delta.y < 0)
						{
							target = player.CurrentSide.UpSide;
							entryIndex = player.Position.X;
							entry = player.CurrentSide.UpEntry;
						}

						var shouldInvert = player.Direction == entry;
						
						if (shouldInvert)
						{
							entryIndex = (field.ColumnCount - 1) - entryIndex;
						}
						switch (entry)
						{
							case Direction.Left: newPoint = new Point(0, entryIndex);
								newDirection = Direction.Right; break;
							case Direction.Down: newPoint = new Point(entryIndex, field.ColumnCount - 1);
								newDirection = Direction.Up; break;
							case Direction.Up: newPoint = new Point(entryIndex, 0);
								newDirection = Direction.Down; break;
							case Direction.Right: newPoint = new Point(field.ColumnCount - 1, entryIndex);
								newDirection = Direction.Left; break;
						}

						if (target.field[newPoint.X, newPoint.Y] == 2)
						{
							return;
						}

						player.Position = newPoint;
						player.CurrentSide = target;
						player.Direction = newDirection;
					}
				}
			}
		}

		private static IEnumerable<Instruction> ParseInstructions(string line)
		{
			int pointer = 0;
			var startPoint = 0;
			
			while (pointer < line.Length)
			{
				if (ParsableUtils.IsNumber(line[pointer]))
				{
					while (pointer < line.Length && ParsableUtils.IsNumber(line[pointer]))
					{
						pointer++;
					}

					yield return new StepInstruction { Steps = int.Parse(line.Substring(startPoint, pointer - startPoint)) };
				} 
				else
				{
					yield return new TurnInstruction(line[pointer]);
					pointer++;
				}

				startPoint = pointer;
			}
		}

		private class Player
		{
			public Point Position { get; set; } = new Point(0, 0);

			public Direction Direction { get; set; } = Direction.Right;

			public (int x, int y) Delta()
			{
				return this.Direction switch
				{
					Direction.Left => (-1, 0),
					Direction.Right => (1, 0),
					Direction.Up => (0, -1),
					Direction.Down => (0, 1),
					_ => throw new Exception(),
				};
			}

			public int Score => (Position.Y + 1) * 1000 + (Position.X + 1) * 4 + (int)Direction;

			public Side CurrentSide { get; set; }
		}

		private interface Instruction
		{
			public void Perform(Player player, Matrix<int> field);
		}

		private class Side
		{
			public Matrix<int> field;

			public int StartX { get; set; }

			public int StartY { get; set; }
			public Side(Matrix<int> field, int startX, int startY)
			{
				this.field = field;
				this.StartX = startX;
				this.StartY = startY;
			}

			public Side LeftSide { get; set; }

			public Side RightSide { get; set; }

			public Side UpSide { get; set; }

			public Side DownSide { get; set; }

			public Direction LeftEntry { get; set; }

			public Direction RightEntry { get; set; }

			public Direction UpEntry { get; set; }

			public Direction DownEntry { get; set; }
		}

		private class TurnInstruction : Instruction
		{
			public TurnInstruction(char direction)
			{
				this.Direction = DirectionFromChar(direction);
			}

			public Direction Direction { get; set; }

			public void Perform(Player player, Matrix<int> field)
			{
				if (this.Direction == Direction.Left && player.Direction == Direction.Right)
				{
					player.Direction = Direction.Up;
				}
				else if (this.Direction == Direction.Right && player.Direction == Direction.Up)
				{
					player.Direction = Direction.Right;
				}
				else
				{
					var delta = this.Direction == Direction.Left ? -1 : 1;
					player.Direction += delta;
				}
			}
		}

		private class StepInstruction : Instruction
		{
			public int Steps { get; set; }

			public void Perform(Player player, Matrix<int> field)
			{
				var delta = player.Delta();
				var stepsTaken = 0;
				while (stepsTaken < this.Steps)
				{
					if (field.IsInBounds(player.Position.X + delta.x, player.Position.Y + delta.y)
						&& field[player.Position.X + delta.x, player.Position.Y + delta.y] == 2)
					{
						return;
					}
					else if (field.IsInBounds(player.Position.X + delta.x, player.Position.Y + delta.y)
						&& field[player.Position.X + delta.x, player.Position.Y + delta.y] == 1)
					{
						player.Position.X += delta.x;
						player.Position.Y += delta.y;
					}
					else
					{
						var inverse = (x: delta.x * -1, y: delta.y * -1);
						var startPos = player.Position.Copy();
						var newPoint = new Point(player.Position.X + inverse.x, player.Position.Y + inverse.y);
						while (field.IsInBounds(newPoint.X, newPoint.Y) && field[newPoint.X, newPoint.Y] != 0)
						{
							player.Position.X += inverse.x;
							player.Position.Y += inverse.y;
							newPoint.X += inverse.x;
							newPoint.Y += inverse.y;
						}

						if (field[player.Position.X, player.Position.Y] == 2)
						{
							player.Position = startPos;
							return;
						}
					}

					stepsTaken++;
				}
			}
		}

		private enum Direction
		{
			Right = 0,
			Down = 1,
			Left = 2,
			Up = 3
		}

		private static Direction DirectionFromChar(char c)
		{
			return c switch
			{
				'L' => Direction.Left,
				'R' => Direction.Right,
				_ => throw new Exception($"Uknown direction {c}"),
			};
		}
	}
}
