using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem16 : ObjectProblem<Valve>
	{
		public override void Solve(IEnumerable<Valve> testData)
		{
			var input = testData.ToList();
			var allValves = input.ToDictionary(x => x.Name, x => x);
			var realNodes = allValves.Values.Where(x => x.FlowRate > 0).ToList();
			input.ForEach(x => x.SetTargets(allValves));
			input.ForEach(x => this.ShortestPath(x, allValves, realNodes));
			this.PrintResult(allValves["AA"].BestResult(new List<string>(), 31, realNodes.Select(x => x.Name)));
			var allCombos = AllCombinations(realNodes.Select(x => x.Name));
			var max = 0;
			var counter = 0;
			foreach (var playerNodes in allCombos)
			{
				var elephantNodes = realNodes.Where(x => !playerNodes.Contains(x.Name)).Select(x => x.Name);
				var playerResult = allValves["AA"].BestResult(new List<string>(), 27, playerNodes);
				var elephantResult = allValves["AA"].BestResult(new List<string>(), 27, elephantNodes);
				max = Math.Max(max, playerResult + elephantResult);
				counter++;
			}

			this.PrintResult(max);

			IEnumerable<IEnumerable<string>> AllCombinations(IEnumerable<string> remaining)
			{
				if (!remaining.Any())
				{
					return Enumerable.Empty<IEnumerable<string>>();
				}

				var remaningCombinations = AllCombinations(remaining.Skip(1));
				var afterValueAdded = remaningCombinations.Concat(remaningCombinations.Select(x => x.Concat(new[] { remaining.First() })));
				return afterValueAdded.Concat(new[] { new[] { remaining.First() } });
			} 
		}

		public Dictionary<Valve, int> ShortestPath(Valve source, Dictionary<string, Valve> allValves, IEnumerable<Valve> realNodes)
		{
			var result = allValves.Values.ToDictionary(x => x, x => int.MaxValue);
			result[source] = 0;
			var queue = new Queue<Valve>();
			queue.Enqueue(source);
			while (queue.Any())
			{
				var current = queue.Dequeue();
				foreach (var t in current.TargetValves.Where(x => result[x] == int.MaxValue))
				{
					result[t] = result[current] + 1;
					queue.Enqueue(t);
				}
			}

			source.WeightedTargets = realNodes.Where(x => x!=source).Select(x => (result[x], x)).ToList();
			return result;
		}
	}

	public class Valve : Parsable
	{
		public int FlowRate { get; set; }

		public string Name { get; set; }

		IList<string> targetValveNames;

		public IList<Valve> TargetValves { get; set; }

		public override void ParseFromLine(string line)
		{
			var tokens = line.Replace(",", "").Replace(";", "").Split(' ');
			this.Name = tokens[1];
			this.FlowRate = int.Parse(tokens[4].Split('=')[1]);
			this.targetValveNames = tokens[9..];
			base.ParseFromLine(line);
		}

		public IList<(int, Valve)> WeightedTargets;

		public void SetTargets(IDictionary<string, Valve> allValves)
		{
			this.TargetValves = this.targetValveNames.Select(x => allValves[x]).ToList();
		}

		public int BestResult(List<string> alreadyOpened, int time, IEnumerable<string> allowedVisits)
		{
			var copy = new List<string>(alreadyOpened);
			copy.Add(this.Name);
			time -= 1;
			var targets = this.WeightedTargets.Where(x => x.Item1 < time && !alreadyOpened.Contains(x.Item2.Name) && allowedVisits.Contains(x.Item2.Name));
			var bestResult = targets.Any() ? targets.Max(x => x.Item2.BestResult(copy, time - x.Item1, allowedVisits)) : 0;
			return (time) * this.FlowRate + bestResult;
		}
	}
}
