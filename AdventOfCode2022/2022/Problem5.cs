using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem5 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var stacks = new List<Stack>();
			var input = new Queue<string>(testData);
			var line = input.Dequeue();
			while (line[1] != '1')
			{
				var pointer = 1;
				var currentStack = 0;
				while (pointer < line.Length)
				{
					if (stacks.Count <= currentStack)
					{
						stacks.Add(new Stack());
					}

					if (line[pointer] != ' ')
					{
						stacks[currentStack].PutFirst(line[pointer]);
					}
				
					currentStack++;
					pointer += 4;
				}

				line = input.Dequeue();
			}

			var stacksCopy = stacks.Select(x => x.Copy()).ToList();
			while (input.Any())
			{
				line = input.Dequeue();
				var tokens = line.Split(' ');
				var source = stacks[int.Parse(tokens[3]) - 1];
				var target = stacks[int.Parse(tokens[5]) - 1];
				var amount = int.Parse(tokens[1]);

				var source2 = stacksCopy[int.Parse(tokens[3]) - 1];
				var target2 = stacksCopy[int.Parse(tokens[5]) - 1];

				target.Put(source.Pop(amount));
				target2.Put(source2.Pop(amount).Reverse());
			}

			Print(stacks);
			Print(stacksCopy);

			void Print(IEnumerable<Stack> stacks)
			{
				this.PrintResult(stacks.Select(x => x.Last).Aggregate((a, b) => a + b));
			}
		}
	}

	public class Stack : Parsable
	{
		private LinkedList<char> items;
		public Stack() 
		{
			this.items = new LinkedList<char>();
		}

		public Stack(LinkedList<char> list)
		{
			this.items = list;
		}

		public Stack Copy()
		{
			var newList = new LinkedList<char>(this.items);
			return new Stack(newList);
		}

		public void Put(IEnumerable<char> items)
		{
			foreach (var c in items)
			{
				this.items.AddLast(c);
			}
		}

		public void PutFirst(char c)
		{
			this.items.AddFirst(c);
		}

		public IEnumerable<char> Pop(int number)
		{
			for (var i = 0; i < number; i++)
			{
				var last = this.items.Last.Value;
				this.items.RemoveLast();
				yield return last;
			}
		}

		public string Last => this.items.Any() ? this.items.Last.Value.ToString() : " ";
	}
}
