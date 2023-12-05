using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem1 : GroupedObjectProblem<ElfInventory, IntegerParsable>
	{
		public override void Solve(IEnumerable<ElfInventory> testData)
		{
			this.PrintResult(testData.Max(x => x.TotalCalories));
			this.PrintResult(testData.OrderByDescending(x => x.TotalCalories).Take(3).Sum(x => x.TotalCalories));
		}
	}
	public class ElfInventory : ParsableGroup<IntegerParsable>
	{
		public int TotalCalories => this.Contents.Sum(x => x.Value);
	}

}
