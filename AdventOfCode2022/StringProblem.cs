using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public abstract class StringProblem : Problem<string>
	{
		protected override string ParseDataLine(string line)
		{
			return line;
		}
	}
}
