using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem3 : ObjectProblem<Rucksack>
	{
		public override void Solve(IEnumerable<Rucksack> testData)
		{
			this.PrintResult(testData.Sum(x => x.Score));

			var list = testData.ToList();
			int total = 0;
			for (var i = 0; i < list.Count; i+=3)
			{
				foreach (var c in list[i].AllItems)
				{
					if (list[i + 1].AllItems.Contains(c) && list[i + 2].AllItems.Contains(c))
					{
						
						total += list[0].ItemPriority(c);
						break;
					}
				}
			}

			this.PrintResult(total);
		}
	}

	public class Rucksack : Parsable
	{
		public string comp1;
		public string comp2;

		public override void ParseFromLine(string line)
		{
			comp1 = line.Substring(0, line.Length / 2);
			comp2 = line.Substring(line.Length / 2);
		}

		public string AllItems => comp1 + comp2;

		public int Score => this.ItemPriority(this.ItemInBothCompartments());

		public char ItemInBothCompartments()
		{
			foreach (var c in comp1)
			{
				if (comp2.Contains(c))
				{
					return c;
				}
			}

			foreach (var c in comp1)
			{
				if (comp2.Contains(c, StringComparison.OrdinalIgnoreCase))
				{
					return c;
				}
			}

			throw this.Exception;
		}

		public int ItemPriority(char item)
		{
			if (item > 'Z')
			{
				return (item + 1) - 'a';
			}

			return (item + 27) - 'A';
		}
	}
}
