using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class StringParsable : Parsable
	{
		public string Value { get; set; }

		public override void ParseFromLine(string line)
		{
			this.Value = line;
		}
	}
}
