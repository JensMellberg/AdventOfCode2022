using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public abstract class ParsableGroup<T>
		where T : Parsable, new()
	{
		public IList<T> Contents = new List<T>();

		public void CreateChild(string line)
		{
			this.Contents.Add(ParsableUtils.CreateFromLine<T>(line));
		}
	}
}
