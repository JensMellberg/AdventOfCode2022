using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem4 : ObjectProblem<SectionPair>
	{
		public override void Solve(IEnumerable<SectionPair> testData)
		{
			this.PrintResult(testData.Sum(x => x.OneRangeContainsOther() ? 1 : 0));
			this.PrintResult(testData.Sum(x => x.OneRangeOverlapsOther() ? 1 : 0));
		}
	}

	public class SectionPair : Parsable
	{
		private SectionRange firstRange;
		private SectionRange secondRange;

		public override void ParseFromLine(string line)
		{
			var ranges = line.Split(',');
			this.firstRange = new SectionRange(ranges[0]);
			this.secondRange = new SectionRange(ranges[1]);
		}

		public bool OneRangeContainsOther()
		{
			return this.firstRange.ContainsOther(this.secondRange) || this.secondRange.ContainsOther(this.firstRange);
		}

		public bool OneRangeOverlapsOther()
		{
			return this.firstRange.OverlapsOther(this.secondRange) || this.secondRange.OverlapsOther(this.firstRange);
		}
	}

	public class SectionRange
	{
		private int firstSection;
		private int lastSection;

		public SectionRange(string line)
		{
			var sections = line.Split('-');
			this.firstSection = int.Parse(sections[0]);
			this.lastSection = int.Parse(sections[1]);
		}

		public bool ContainsOther(SectionRange other)
		{
			return this.firstSection <= other.firstSection && this.lastSection >= other.lastSection;
		}

		public bool OverlapsOther(SectionRange other)
		{
			return this.firstSection <= other.firstSection && this.lastSection >= other.firstSection;
		}
	}
}
