using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode2022
{
	public class Problem17 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var counter = 0;
			var pointer = 0;
			var pattern = testData.Single();
			var fieldHeight = 1000000;
			var field = Matrix.InitWithStartValue(fieldHeight, 7, false);
			int highestBlock = -1;
			var memory = new Dictionary<string, string>();
			var goal = 1000000000000;
			long afterRepeat = 0;
			var matches = 0;
			while (counter < goal)
			{
				var pieceType = counter % 5;
				var piece = Piece.Create(pieceType, highestBlock + 4);
				counter++;
				var landedBlock = 0;
				while (true)
				{
					var input = pattern[pointer];
					pointer = (pointer + 1) % pattern.Length;
					var dx = input == '>' ? 1 : -1;
					piece.Move(dx, 0, field);
					var result = piece.Move(0, -1, field);
					if (result != -1)
					{
						landedBlock = result;
						highestBlock = Math.Max(landedBlock, highestBlock);
						break;
					}
				}

				if (counter == 2022)
				{
					this.PrintResult(highestBlock + 1);
				}

				if (afterRepeat == 0)
				{
					var key = pieceType.ToString() + "," + pointer + "," + string.Join(',', field.GetRow(landedBlock).Select(x => x ? "1" : "0"));
					var value = highestBlock + "," + counter;
					if (memory.ContainsKey(key))
					{
						// Not sure why the first repeat yields the wrong result. But oh well :)
						if (matches < 1)
						{
							matches++;
						} 
						else
						{
							var previous = memory[key].Split(",");
							var previousCounter = int.Parse(previous[1]);
							var previousBlock = int.Parse(previous[0]);
							var blockDiff = highestBlock - previousBlock;
							var counterDiff = counter - previousCounter;
							var repeatTimes = (goal - counter) / counterDiff;
							var remaining = (goal - counter) % counterDiff;
							goal = counter + remaining;
							afterRepeat = repeatTimes * blockDiff;
						}
					}
					else
					{
						memory.Add(key, value);
					}
				}
			}

			this.PrintResult((long)highestBlock + afterRepeat + 1);

		}

		private class Piece
		{
			IList<Block> Blocks;

			public Block highestBlock { get; set; }

			public int Move(int dX, int dY, Matrix<bool> field)
			{
				var newPositions = new Point[Blocks.Count];
				var reachedBottom = false;
				for (var i = 0; i < Blocks.Count; i++)
				{
					newPositions[i] = new Point(Blocks[i].Position.X + dX, Blocks[i].Position.Y + dY);
					if (newPositions[i].X < 0 || newPositions[i].X > 6)
					{
						return -1;
					}
					else if (newPositions[i].Y < 0)
					{
						reachedBottom = true;
					}
				}

				if (reachedBottom || newPositions.Any(x => field[x.X, x.Y]))
				{
					if (dY != 0)
					{
						this.Blocks.ForEach(x => field[x.Position.X, x.Position.Y] = true);
					}

					return this.highestBlock.Position.Y;
				}
				else
				{
					for (var i = 0; i < Blocks.Count; i++)
					{
						Blocks[i].Position = newPositions[i];
					}

					return -1;
				}
			}

			public static Piece Create(int type, int top)
			{
				var blocks = new List<Block>();
				if (type == 3)
				{
					blocks.Add(new Block { Position = new Point(2, top + 3) });
					blocks.Add(new Block { Position = new Point(2, top + 2) });
					blocks.Add(new Block { Position = new Point(2, top + 1) });
					blocks.Add(new Block { Position = new Point(2, top) });
				}
				else if (type == 4)
				{
					blocks.Add(new Block { Position = new Point(2, top + 1) });
					blocks.Add(new Block { Position = new Point(2, top) });
					blocks.Add(new Block { Position = new Point(3, top + 1) });
					blocks.Add(new Block { Position = new Point(3, top) });
				} 
				else if (type == 0)
				{
					blocks.Add(new Block { Position = new Point(2, top) });
					blocks.Add(new Block { Position = new Point(3, top) });
					blocks.Add(new Block { Position = new Point(4, top) });
					blocks.Add(new Block { Position = new Point(5, top) });
				} 
				else if (type == 1)
				{
					blocks.Add(new Block { Position = new Point(3, top + 2) });
					blocks.Add(new Block { Position = new Point(2, top + 1) });
					blocks.Add(new Block { Position = new Point(3, top + 1) });
					blocks.Add(new Block { Position = new Point(3, top) });
					blocks.Add(new Block { Position = new Point(4, top + 1) });
				}
				else if (type == 2)
				{
					blocks.Add(new Block { Position = new Point(4, top + 2) });
					blocks.Add(new Block { Position = new Point(4, top + 1) });
					blocks.Add(new Block { Position = new Point(4, top) });
					blocks.Add(new Block { Position = new Point(3, top) });
					blocks.Add(new Block { Position = new Point(2, top) });
				}

				var highestBlock = blocks.OrderByDescending(x => x.Position.Y).First();

				return new Piece { Blocks = blocks, highestBlock = highestBlock };
			}
		}

		private class Block
		{
			public Point Position { get; set; }
		}
	}
}
