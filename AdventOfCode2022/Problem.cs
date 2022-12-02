using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2022
{
	public abstract class Problem
	{
		private int AnswersGiven = 0;
		public abstract void Solve(IEnumerable<string> testData);

		protected void PrintResult(string result)
		{
			if (AnswersGiven == 0)
			{
				Console.Write("First answer: ");
			}
			else if (AnswersGiven == 1)
			{
				Console.Write("Second answer: ");
			}

			Console.WriteLine(result);
			this.AnswersGiven++;
		}

		protected void PrintResult(int result)
		{
			this.PrintResult(result.ToString());
		}
	}
}
