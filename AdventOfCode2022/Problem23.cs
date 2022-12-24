using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem23 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var occupiedPositions = new HashSet<string>();
			var elves = ParseInput(testData.ToList(), occupiedPositions);
			var handler = new InstructionHandler();
			for (var i = 0; i < 10; i++)
			{
				this.DoRound(elves, occupiedPositions, handler);
			}

			this.PrintResult(GetEmptyCount(elves));
			int counter = 10;
			while (true)
			{
				counter++;
				if (!this.DoRound(elves, occupiedPositions, handler))
				{
					break;
				}
			}

			this.PrintResult(counter);
		}

		private static int GetEmptyCount(IList<Elf> elves)
		{
			var minX = int.MaxValue;
			var maxX = int.MinValue;
			var minY = int.MaxValue;
			var maxY = int.MinValue;
			foreach (var elf in elves)
			{
				minX = Math.Min(elf.Position.X, minX);
				maxX = Math.Max(elf.Position.X, maxX);
				minY = Math.Min(elf.Position.Y, minY);
				maxY = Math.Max(elf.Position.Y, maxY);
			}

			var width = maxX - minX + 1;
			var height = maxY - minY + 1;
			var area = width * height;
			return area - elves.Count;
		}

		private bool DoRound(IList<Elf> elves, HashSet<string> occupiedPositions, InstructionHandler handler)
		{
			var proposedMoves = new Dictionary<string, IList<Elf>>();
			handler.DoInstructions(elves, occupiedPositions, proposedMoves);
			var hasMoved = false;
			foreach (var kv in proposedMoves)
			{
				if (kv.Value.Count != 1)
				{
					continue;
				}

				occupiedPositions.Remove(kv.Value[0].Position.ToString());
				kv.Value[0].Position = Point.FromString(kv.Key);
				occupiedPositions.Add(kv.Key);
				hasMoved = true;
			}

			return hasMoved;
		}

		private static IList<Elf> ParseInput(IList<string> input, HashSet<string> occupiedPositions)
		{
			var result = new List<Elf>();
			for (var y = 0; y < input.Count; y++)
			{
				var line = input[y];
				for (var x = 0; x < line.Length; x++)
				{
					if (line[x] == '#')
					{
						var position = new Point(x, y);
						result.Add(new Elf { Position = position});
						occupiedPositions.Add(position.ToString());
					}
				}
			}

			return result;
		}

		private class Elf
		{
			public Point Position { get; set; }

			public Point ProposedMove { get; set; }
		}

		private class InstructionHandler
		{
			private int firstInstruction = 0;

			public void DoInstructions(IList<Elf> elves, HashSet<string> occupiedPositions, Dictionary<string, IList<Elf>> proposedMoves)
			{
				foreach (var elf in elves)
				{
					var foundElf = false;
					elf.ProposedMove = null;
					for (var x = -1; x < 2 && !foundElf; x++)
					{
						for (var y = -1; y < 2; y++)
						{
							if (!(x == 0 && y == 0) && occupiedPositions.Contains(new Point(elf.Position.X + x, elf.Position.Y + y).ToString())) {
								foundElf = true;
								break;
							}
						}
					}

					if (!foundElf)
					{
						continue;
					}

					for (var i = 0; i < 4; i++)
					{
						if (DoInstruction((firstInstruction + i) % 4))
						{
							break;
						}
					}

					bool DoInstruction(int id)
					{
						var position = elf.Position;
						if (id == 0)
						{
							if (!occupiedPositions.Contains(new Point(position.X, position.Y - 1).ToString())
								&& !occupiedPositions.Contains(new Point(position.X + 1, position.Y - 1).ToString())
								&& !occupiedPositions.Contains(new Point(position.X - 1, position.Y - 1).ToString())) {
								ProposeMove(new Point(position.X, position.Y - 1));
								return true;
							}
						}
						else if (id == 1)
						{
							if (!occupiedPositions.Contains(new Point(position.X, position.Y + 1).ToString())
								&& !occupiedPositions.Contains(new Point(position.X + 1, position.Y + 1).ToString())
								&& !occupiedPositions.Contains(new Point(position.X - 1, position.Y + 1).ToString()))
							{
								ProposeMove(new Point(position.X, position.Y + 1));
								return true;
							}
						}
						else if (id == 2)
						{
							if (!occupiedPositions.Contains(new Point(position.X - 1, position.Y).ToString())
								&& !occupiedPositions.Contains(new Point(position.X - 1, position.Y + 1).ToString())
								&& !occupiedPositions.Contains(new Point(position.X - 1, position.Y - 1).ToString()))
							{
								ProposeMove(new Point(position.X - 1, position.Y));
								return true;
							}
						}
						else if (id == 3)
						{
							if (!occupiedPositions.Contains(new Point(position.X + 1, position.Y).ToString())
								&& !occupiedPositions.Contains(new Point(position.X + 1, position.Y + 1).ToString())
								&& !occupiedPositions.Contains(new Point(position.X + 1, position.Y - 1).ToString()))
							{
								ProposeMove(new Point(position.X + 1, position.Y));
								return true;
							}
						}

						return false;

						void ProposeMove(Point move)
						{
							elf.ProposedMove = move;
							var key = move.ToString();
							if (!proposedMoves.ContainsKey(key))
							{
								proposedMoves.Add(key, new List<Elf>());
							}

							proposedMoves[key].Add(elf);
						}
					}
				}

				this.firstInstruction = (this.firstInstruction + 1) % 4;

			}
		}
	}
}
