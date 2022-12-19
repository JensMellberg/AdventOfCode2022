using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode2022
{
	public class Problem19 : ObjectProblem<BluePrint>
	{
		public override void Solve(IEnumerable<BluePrint> testData)
		{
			//this.PrintResult(RunBluePrints(testData.ToList(), 24).Sum());
			//this.PrintResult(RunBluePrints(testData.Take(3).ToList(), 32).Aggregate((a, b) => a * b));
			//this.PrintResult(testData.Select(x => ResultWithBluePrint(x, 24)).Sum());
			this.PrintResult(testData.Take(3).Select(x => ResultWithBluePrint(x, 32)).Aggregate((a, b) => a * b));
		}

		public IEnumerable<int> RunBluePrints(IList<BluePrint> bluePrints, int totalTime)
		{
			var goal = bluePrints.Count;
			var results = new List<int>();
			Parallel.For(0, goal, i => results.Add(ResultWithBluePrint(bluePrints[i], totalTime)));
			/*foreach (var bluePrint in bluePrints)
			{
				new Thread(() =>
				{
					results.Add(ResultWithBluePrint(bluePrint, totalTime));
				}).Start();
			}*/

			while (results.Count < goal)
			{
				Thread.Sleep(2000);
			}

			return results;
		}

		public static int ResultWithBluePrint(BluePrint bluePrint, int totalTime)
		{
			var robots = new List<Robot>
			{
				new Robot { RobotType = ResourceType.Ore }
			};

			var resourceCounts = new Dictionary<ResourceType, int>()
			{
				{ ResourceType.Ore, 0 },
				{ ResourceType.Clay, 0 },
				{ ResourceType.Obsidian, 0 },
				{ ResourceType.Geode, 0 }
			};

			var bestResult = BestResult(bluePrint, robots, new Resources { ResourceCounts = resourceCounts }, totalTime, new List<ResourceType>(), null, new HashSet<string>(), new Dictionary<int, int>());
			return bluePrint.Id * bestResult;
		}

		public static int BestResult(BluePrint bluePrint, List<Robot> robots, Resources resources, int time, int timeSpent, List<ResourceType> pausedTypes, Robot newRobot, HashSet<string> visited, IDictionary<int, int> BestForTime)
		{
			time-=timeSpent;
			/*var robotKey = string.Join(" ", robots.OrderBy(x => x.RobotType).Select(x => (int)x.RobotType));
			var resourceKey = string.Join(',', resources.ResourceCounts.Values);
			var key = robotKey + "|" + resourceKey + "|" + time;
			if (visited.Contains(key))
			{
				return 0;
			}

			visited.Add(key);*/
			var newResourceCounts = new Dictionary<ResourceType, int>(resources.ResourceCounts);
			robots.ForEach(x => newResourceCounts[x.RobotType]+= timeSpent);
			var newResources = new Resources { ResourceCounts = newResourceCounts };
			var hasBoughtMandatory = false;
			if (time == 0)
			{
				return newResources.ResourceCounts[ResourceType.Geode];
			}

			if (!BestForTime.ContainsKey(time))
			{
				BestForTime.Add(time, 0);
			}

			if (BestForTime[time] > newResources.ResourceCounts[ResourceType.Geode] + 3)
			{
				return BestForTime[time];
			}

			BestForTime[time] = Math.Max(BestForTime[time], newResources.ResourceCounts[ResourceType.Geode]);

			var newList = new List<Robot>(robots);
			if (newRobot != null)
			{
				newList.Add(newRobot);
			}

			var result = 0;
			var pausedTypesCopy = new List<ResourceType>(pausedTypes);
			foreach (var kv in bluePrint.RobotCosts)
			{
				if (!pausedTypesCopy.Contains(kv.Key) && kv.Value.All(x => newResources.ResourceCounts[x.RobotType] >= x.Cost))
				{
					result = Math.Max(result, BestResult(bluePrint, newList, newResources.Copy(kv.Value), time, new List<ResourceType>()/*pausedTypesCopy.Where(x => !pausedTypes.Contains(x)).ToList()*/, new Robot { RobotType = kv.Key }, visited, BestForTime));
					if (kv.Key == ResourceType.Geode || kv.Key == ResourceType.Obsidian)
					{
						hasBoughtMandatory = true;
					}
					else
					{
						pausedTypesCopy.Add(kv.Key);
					}
				}
			}

			if (!hasBoughtMandatory)
			{
				result = Math.Max(result, BestResult(bluePrint, newList, newResources, time, pausedTypesCopy, null, visited, BestForTime));
			}

			return result;
		}
	}
	public class BluePrint : Parsable
	{
		public override void ParseFromLine(string line)
		{
			base.ParseFromLine(line);
			var tokens = line.Split(' ');
			this.Id = int.Parse(tokens[1].Substring(0, tokens[1].Length - 1));
			var oreCost = int.Parse(tokens[6]);
			var clayCost = int.Parse(tokens[12]);
			var obsidianOreCost = int.Parse(tokens[18]);
			var obsidianClayCost = int.Parse(tokens[21]);
			var geodeOreCost = int.Parse(tokens[27]);
			var geodeObsidianCost = int.Parse(tokens[30]);
			this.RobotCosts.Add(ResourceType.Geode, new[]
			{
				new RobotCost { RobotType = ResourceType.Ore, Cost = geodeOreCost },
				new RobotCost { RobotType = ResourceType.Obsidian, Cost = geodeObsidianCost }
			});
			this.RobotCosts.Add(ResourceType.Obsidian, new[]
			{
				new RobotCost { RobotType = ResourceType.Ore, Cost = obsidianOreCost },
				new RobotCost { RobotType = ResourceType.Clay, Cost = obsidianClayCost }
			});
			this.RobotCosts.Add(ResourceType.Ore, new[] { new RobotCost { RobotType = ResourceType.Ore, Cost = oreCost } });
			this.RobotCosts.Add(ResourceType.Clay, new[] { new RobotCost { RobotType = ResourceType.Ore, Cost = clayCost } });
		}

		public int Id { get; set; }

		public Dictionary<ResourceType, RobotCost[]> RobotCosts = new Dictionary<ResourceType, RobotCost[]>();
	}

	public class RobotCost
	{
		public ResourceType RobotType { get; set; }

		public int Cost { get; set; }
	}

	public class Robot
	{
		public ResourceType RobotType { get; set; }
	}

	public class Resources
	{
		public Dictionary<ResourceType, int> ResourceCounts;

		public Resources Copy(IEnumerable<RobotCost> costs)
		{
			var newResources = new Dictionary<ResourceType, int>(this.ResourceCounts);
			foreach (var c in costs)
			{
				newResources[c.RobotType] -= c.Cost;
			}

			return new Resources { ResourceCounts = newResources };
		}
	}

	public enum ResourceType
	{
		Ore,
		Clay,
		Obsidian,
		Geode
	}

}
