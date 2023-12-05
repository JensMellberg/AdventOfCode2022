using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem25 : ObjectProblem<SnafuNumber>
	{
		public override void Solve(IEnumerable<SnafuNumber> testData)
		{
			var sum = testData.Sum(x => x.ToBase10());
			this.PrintResult(SnafuNumber.FromBase10(sum));
		}
	}
	public class SnafuNumber : Parsable
	{
		private string line;
		public override void ParseFromLine(string line)
		{
			this.line = line;
		}

		public long ToBase10()
		{
			var pointer = this.line.Length - 1;
			long result = 0;
			long multiplier = 1;
			while (pointer >= 0)
			{
				long value = line[pointer] switch
				{
					'-' => -1,
					'=' => -2,
					_ => line[pointer] - '0',
				};

				result += value * multiplier;
				pointer--;
				multiplier *= 5;
			}

			return result;
		}

		public static string FromBase10(long value)
		{
			var start = (long)(Math.Pow(5, 21));
			var currentPower = 21;
			var result = new int[21];
			while (start > value)
			{
				start /= 5;
				currentPower--;
			}

			while (currentPower >= 0)
			{
				var times = (int)(value / start);
				if (times == 4)
				{
					result[currentPower]--;
					result[currentPower + 1]++;
				}
				else if (times == 3)
				{
					result[currentPower]-=2;
					result[currentPower + 1]++;
				}
				else
				{
					result[currentPower] += times;
				}

				value -= times * start;
				currentPower--;
				start /= 5;
			}

			for (var i = 0; i < result.Length; i++)
			{
				if (result[i] > 2)
				{
					result[i + 1]++;
					if (result[i] == 3)
					{
						result[i] = -2;
					}
				}
			}

			var number = "";
			var hasFound = false;
			for (var i = result.Length - 1; i>= 0; i--)
			{
				if (!hasFound && result[i] == 0)
				{
					continue;
				}

				hasFound = true;
				number += result[i] switch
				{
					-2 => "=",
					-1 => "-",
					_ => result[i].ToString(),
				};
			}

			return number;
		}
	}

}
