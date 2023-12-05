using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem19 : ObjectProblem<BluePrint>
	{
		public override void Solve(IEnumerable<BluePrint> testData)
		{
			this.PrintResult(testData.Select(x => ResultWithBluePrint(x, 24, x.Id)).Sum());
			this.PrintResult(testData.Take(3).Select(x => ResultWithBluePrint(x, 32, 1)).Aggregate((a, b) => a * b));
		}

		public static int ResultWithBluePrint(BluePrint bluePrint, int totalTime, int multiplier)
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

			var bestResult = BestResult(bluePrint, robots, new Resources { ResourceCounts = resourceCounts }, totalTime, new List<ResourceType>(), null);
			Console.WriteLine(bluePrint.Id + ": " + bestResult);
			return multiplier * bestResult;
		}

		public static int BestResult(BluePrint bluePrint, List<Robot> robots, Resources resources, int time, List<ResourceType> pausedTypes, Robot newRobot)
		{
			time--;
			var newResourceCounts = new Dictionary<ResourceType, int>(resources.ResourceCounts);
			robots.ForEach(x => newResourceCounts[x.RobotType]++);
			var diffDict = resources.ResourceCounts.Keys.ToDictionary(x => x, x => newResourceCounts[x] - resources.ResourceCounts[x]);

			var newResources = new Resources { ResourceCounts = newResourceCounts };
			var hasBoughtMandatory = false;
			var newList = new List<Robot>(robots);
			if (newRobot != null)
			{
				newList.Add(newRobot);
			}

			var result = 0;
			var pausedTypesCopy = new List<ResourceType>(pausedTypes);
			foreach (var type in bluePrint.MaxResourceDiffs.Keys.Where(x => !pausedTypesCopy.Contains(x)))
			{
				if (diffDict[type] >= bluePrint.MaxResourceDiffs[type])
				{
					pausedTypesCopy.Add(type);
				}
			}

			foreach (var kv in bluePrint.RobotCosts)
			{
				if (!pausedTypesCopy.Contains(kv.Key) && kv.Value.All(x => newResources.ResourceCounts[x.RobotType] >= x.Cost))
				{
					result = Math.Max(result, BestResult(bluePrint, newList, newResources.Copy(kv.Value), time, new List<ResourceType>(), new Robot { RobotType = kv.Key }));
					if (kv.Key == ResourceType.Geode)
					{
						hasBoughtMandatory = true;
						break;
					}
					else
					{
						pausedTypesCopy.Add(kv.Key);
					}
				}
			}

			if (!hasBoughtMandatory)
			{
				result = Math.Max(result, BestResult(bluePrint, newList, newResources, time, pausedTypesCopy, null));
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

			foreach (var k in RobotCosts.Keys)
			{
				MaxResourceDiffs.Add(k, 0);
			}
			MaxResourceDiffs.Remove(ResourceType.Geode);
			var types = new[] { ResourceType.Ore, ResourceType.Clay, ResourceType.Obsidian };
			foreach (var type in types)
			{
				foreach (var cost in RobotCosts.SelectMany(x => x.Value).Where(x => x.RobotType == type))
				{
					MaxResourceDiffs[type] = Math.Max(MaxResourceDiffs[type], cost.Cost);
				}
			}
		}

		public Dictionary<ResourceType, int> MaxResourceDiffs = new Dictionary<ResourceType, int>();

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