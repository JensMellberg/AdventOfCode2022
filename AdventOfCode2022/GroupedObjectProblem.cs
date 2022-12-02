using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public abstract class GroupedObjectProblem<T, TChild> : Problem<T>
		where T : ParsableGroup<TChild>, new()
		where TChild : Parsable, new()
	{
		private T currentGroup;

		public override IEnumerable<T> ParseData(string testData)
		{
			var lines = testData.Split('\n');
			currentGroup = new T();
			foreach (var line in lines)
			{
				if (string.IsNullOrEmpty(line))
				{
					var previousGroup = currentGroup;
					currentGroup = new T();
					yield return previousGroup;
				}
				else
				{
					currentGroup.CreateChild(line);
				}
			}
		}

		protected override T ParseDataLine(string line)
		{
			return null;
		}
	}
}
