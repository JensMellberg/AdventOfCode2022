using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem6 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var input = testData.First();
			this.PrintResult(this.GetAnswer(input, 4));
			this.PrintResult(this.GetAnswer(input, 14));
		}

		private int GetAnswer(string input, int number)
		{
			int pointer = 0;
			while (true)
			{
				int pointerOffset = 1;
				while (IsUnique(input[pointer + pointerOffset], pointer + pointerOffset))
				{
					pointerOffset++;
					if (pointerOffset == number)
					{
						return pointer + pointerOffset;
					}
				}

				pointer++;
			}

			bool IsUnique(char value, int position)
			{
				while (position > pointer)
				{
					position--;
					if (input[position] == value)
					{
						return false;
					}

				}

				return true;
			}
		}
	}
}
