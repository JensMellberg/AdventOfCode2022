using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public abstract class PatternProblem<T> : Problem<T>
		where T : new()
	{
		protected abstract string Pattern { get; }

		private PatternParser parser;
		protected override T ParseDataLine(string line)
		{
			var parser = this.parser ??= new PatternParser(this.Pattern);
			return parser.ParseObject<T>(line);
		}
	}
}
