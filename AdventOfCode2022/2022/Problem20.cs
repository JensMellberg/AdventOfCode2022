using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem20 : ObjectProblem<MixNumber>
	{
		public const long Key = 811589153;
		public override void Solve(IEnumerable<MixNumber> testData)
		{
			var input = testData.ToList();
			this.DoInterations(input, 1);
			input.ForEach(x => x.Number *= Key);
			this.DoInterations(input, 10);
		}

		public void DoInterations(IEnumerable<MixNumber> data, int iterations)
		{
			var input = new LinkedList<MixNumber>(data);
			var queueOriginal = new Queue<MixNumber>(input);
			var listCount = input.Count;
			for (var x = 0; x < iterations; x++)
			{
				var queue = new Queue<MixNumber>(queueOriginal);
				while (queue.Any())
				{
					var current = queue.Dequeue();
					if (current.Number == 0)
					{
						continue;
					}

					var node = input.Find(current);
					for (int i = 0; i < current.Number % (listCount - 1); i++)
					{
						if (node == input.Last)
						{
							node = input.First;
						}
						else
						{
							node = node.Next;
						}

						if (node.Value == current)
						{
							if (node == input.Last)
							{
								node = input.First;
							}
							else
							{
								node = node.Next;
							}
						}
					}

					if (current.Number < 0)
					{
						for (int i = 0; i < ((current.Number * -1) % (listCount - 1)); i++)
						{
							if (node == input.First)
							{
								node = input.Last;
							}
							else
							{
								node = node.Previous;
							}

							if (node.Value == current)
							{
								if (node == input.First)
								{
									node = input.Last;
								}
								else
								{
									node = node.Previous;
								}
							}
						}
					}

					input.Remove(current);
					if (current.Number < 0)
					{
						if (node == input.First)
						{
							input.AddAfter(input.Last, current);
						}
						else
						{
							input.AddBefore(node, current);
						}
					}
					else
					{
						if (node == input.Last)
						{
							input.AddBefore(input.First, current);
						}
						else
						{
							input.AddAfter(node, current);
						}
					}
				}
			}

			long result = 0;
			var zero = input.Single(x => x.Number == 0);
			var currentNode = input.Find(zero);
			for (var i = 0; i < 3001; i++)
			{
				if (i % 1000 == 0)
				{
					result += currentNode.Value.Number;
				}
				if (currentNode == input.Last)
				{
					currentNode = input.First;
				}
				else
				{
					currentNode = currentNode.Next;
				}
			}
			this.PrintResult(result);
		}
	}

	public class MixNumber : Parsable
	{
		private static int currentId;
		private static int GetNextId() => currentId++;

		public int Id { get; set; }

		public long Number { get; set; }

		public override void ParseFromLine(string line)
		{
			this.Id = GetNextId();
			this.Number = int.Parse(line);
			base.ParseFromLine(line);
		}

		public override string ToString()
		{
			return this.Number.ToString();
		}
	}
}
