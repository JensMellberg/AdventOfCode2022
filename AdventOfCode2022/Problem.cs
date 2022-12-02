using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public abstract class Problem<T>
	{
		private int AnswersGiven = 0;

		protected virtual EmptyStringBehavior EmptyStringBehavior => EmptyStringBehavior.Reject;
		public abstract void Solve(IEnumerable<T> testData);

		public virtual IEnumerable<T> ParseData(string testData)
		{
			return testData.Split('\n')
				.Where(x => this.EmptyStringBehavior == EmptyStringBehavior.Keep || !string.IsNullOrEmpty(x))
				.Select(ParseDataLine);
		}

		protected abstract T ParseDataLine(string line);

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

	public enum EmptyStringBehavior
	{
		Reject,
		Keep
	}
}
