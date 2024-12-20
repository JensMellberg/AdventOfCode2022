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

        protected virtual TabBehavior TabBehavior => TabBehavior.Keep;

        public abstract void Solve(IEnumerable<T> testData);

		protected bool supressPrints;

		private List<string> Answers = new List<string>();

		public void SetBenchmarkRun()
		{
			this.supressPrints = true;
		}

		public List<string> GetResults() => this.Answers;

		public virtual IEnumerable<T> ParseData(string testData)
		{
			return testData.Split('\n')
                .Select(x => this.TabBehavior == TabBehavior.Reject ? x.Replace("\r", "") : x)
                .Where(x => this.EmptyStringBehavior == EmptyStringBehavior.Keep || !string.IsNullOrEmpty(x))
				.Select(ParseDataLine);
		}

        public static Direction[] AllDirections = { Direction.Left, Direction.Right, Direction.Up, Direction.Down };

        protected abstract T ParseDataLine(string line);

		protected void PrintResult(string result)
		{
			if (AnswersGiven == 0)
			{
				this.PrintWithoutLineBreak("First answer: ");
			}
			else if (AnswersGiven == 1)
			{
				this.PrintWithoutLineBreak("Second answer: ");
			}

			this.Print(result);
			this.Answers.Add(result);
			this.AnswersGiven++;
		}

		protected void PrintResult(object result)
		{
			this.PrintResult(result.ToString());
		}

		protected void Print(string s)
		{
			this.PrintWithoutLineBreak(s + "\n");
		}

		protected void PrintWithoutLineBreak(string s)
		{
			if (this.supressPrints)
			{
				return;
			}

			Console.Write(s);
		}
	}

	public enum EmptyStringBehavior
	{
		Reject,
		Keep
	}

    public enum TabBehavior
    {
        Reject,
        Keep
    }

	public enum Direction
	{
		Left,
		Right,
		Up,
		Down
	}
}
