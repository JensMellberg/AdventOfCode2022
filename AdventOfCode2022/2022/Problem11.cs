using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem11 : StringProblem
	{
		public static bool DoDivide = true;

		public static int DivisorProduct { get; set; }
		public override void Solve(IEnumerable<string> testData)
		{
			var data = new Queue<string>(testData);
			var monkeys = new List<Monkey>();
			while (data.Any())
			{
				var currentMonkey = new Monkey();
				monkeys.Add(currentMonkey);
				data.Dequeue();
				
				var itemsLine = data.Dequeue();
				currentMonkey.InitItems(itemsLine.Substring(itemsLine.IndexOf("items:") + 6).Replace(" ", "").Split(',').Select(x => long.Parse(x)).ToList());
				var operationLine = data.Dequeue();
				currentMonkey.SetOperators(operationLine);
				var divisibleLine = data.Dequeue();
				currentMonkey.DivisibleBy = int.Parse(divisibleLine.Substring(divisibleLine.IndexOf("by ") + 3));
				currentMonkey.TrueMonkey = ParseMonkeyNbr(data.Dequeue());
				currentMonkey.FalseMonkey = ParseMonkeyNbr(data.Dequeue());
				int ParseMonkeyNbr(string line)
				{
					return int.Parse(line.Substring(line.IndexOf("monkey ") + 7));
				}
			}

			DivisorProduct = monkeys.Select(x => x.DivisibleBy).Aggregate((a, b) => a * b);

			this.DoRounds(20, monkeys);
			PrintResult();
			DoDivide = false;
			monkeys.ForEach(x => x.Reset());
			this.DoRounds(10000, monkeys);
			PrintResult();
			void PrintResult()
			{
				this.PrintResult(monkeys.Select(x => x.InspectionCount).OrderByDescending(x => x).Take(2).Aggregate((a, b) => a * b));
			}
		}

		private void DoRounds(int rounds, List<Monkey> monkeys)
		{
			for (var i = 0; i < rounds; i++)
			{
				monkeys.ForEach(x => x.InspectItems(monkeys));
			}
		}

		private class Monkey
		{
			private Func<long, long> Operation;

			private Func<long, long> Left;

			private Func<long, long> Right;

			public int DivisibleBy { get; set; }
			public List<long> Items { get; set; }

			public void InitItems(List<long> items)
			{
				this.Items = items;
				this.OriginalItems = this.Items.Select(x => x).ToList();
			}

			public List<long> OriginalItems;

			public int TrueMonkey { get; set; }

			public int FalseMonkey { get; set; }

			public long InspectionCount { get; set; }

			public void Reset()
			{
				this.Items = this.OriginalItems;
				this.InspectionCount = 0;
			}

			public void SetOperators(string line)
			{
				var expression = line.Substring(line.IndexOf("new = ") + 6);
				var tokens = expression.Split(' ');
				this.Left = CreateTermGetter(tokens[0]);
				this.Right = CreateTermGetter(tokens[2]);
				if (tokens[1] == "+")
				{
					this.Operation = (x) => Left(x) + Right(x);
				}
				else if (tokens[1] == "*")
				{
					this.Operation = (x) => Left(x) * Right(x);
				}

				Func<long, long> CreateTermGetter(string term)
				{
					if (term == "old")
					{
						return (x) => x;
					}

					return (x) => long.Parse(term);
				}
			}

			public void InspectItems(List<Monkey> monkeys)
			{
				for (int i = 0; i < this.Items.Count; i++)
				{
					this.InspectionCount++;
					Items[i] = this.Operation(Items[i]) % DivisorProduct;
					Items[i] = DoDivide ? (long)Math.Floor((double)Items[i] / 3) : Items[i];
					var toMonkey = Items[i] % this.DivisibleBy == 0 ? TrueMonkey : FalseMonkey;
					monkeys[toMonkey].Items.Add(Items[i]);
					Items.RemoveAt(i);
					i--;
				}
			}
		}
	}
}
