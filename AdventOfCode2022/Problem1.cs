using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem1 : Problem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var inventories = new List<ElfInventory>();
			var currentInventory = new ElfInventory();
			inventories.Add(currentInventory);
			foreach (var calories in testData)
			{
				if (string.IsNullOrEmpty(calories))
				{
					currentInventory = new ElfInventory();
					inventories.Add(currentInventory);
				} else
				{
					currentInventory.FoodCalories.Add(int.Parse(calories));
				}
			}

			this.PrintResult(inventories.Max(x => x.TotalCalories));

			this.PrintResult(inventories.OrderByDescending(x => x.TotalCalories).Take(3).Sum(x => x.TotalCalories));
		}

		private class ElfInventory
		{
			public List<int> FoodCalories = new List<int>();

			public int TotalCalories => this.FoodCalories.Sum();
		}
	}

}
