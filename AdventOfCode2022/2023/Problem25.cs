using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem25 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        private Dictionary<string, Component> Components = new Dictionary<string, Component>();

        private Dictionary<string, long> TimesPassedEachConnection = new Dictionary<string, long>();

        private HashSet<string> LongestConnectionEdges = new HashSet<string>();

        private long LongestDist = 0;

        private (Component from, Component to) LongestPair;

        public override void Solve(IEnumerable<string> testData)
        {
            foreach (var line in testData)
            {
                var parser = new TokenParser(line);
                var name = parser.Pop()[..^1];
                if (!Components.TryGetValue(name, out var component))
                {
                    component = new Component(name);
                    Components.Add(name, component);
                }

                var connectedTo = parser.PopUntil('_');
                foreach (var c in connectedTo)
                {
                    if (!Components.TryGetValue(c, out var connected))
                    {
                        connected = new Component(c);
                        Components.Add(c, connected);
                    }

                    component.Connections.Add(connected);
                    connected.Connections.Add(component);
                }
            }

            Components.Values.ForEach(x => this.ShortestPath(x, true, false));
            var timesPassedEachConnectionCopy = new Dictionary<string, long>(TimesPassedEachConnection);
            this.ShortestPath(LongestPair.from, true, true);

            var mostVisitedConnections = timesPassedEachConnectionCopy.OrderByDescending(x => x.Value).Select(x => x.Key).ToList();
            for (var i = 0; i < mostVisitedConnections.Count; i++)
            {
                var c = mostVisitedConnections[i];
                for (var i2 = i + 1; i2 < mostVisitedConnections.Count; i2++)
                {
                    var c2 = mostVisitedConnections[i2];

                    for (var i3 = i2 + 1; i3 < mostVisitedConnections.Count; i3++)
                    {
                        var c3 = mostVisitedConnections[i3];
                        if (!new[] { c, c2, c3}.Any(x => LongestConnectionEdges.Contains(x)))
                        {
                            continue;
                        }

                        Inactivate(c);
                        Inactivate(c2);
                        Inactivate(c3);
                        var totalReached = this.ShortestPath(this.LongestPair.from, false, false);
                        if (totalReached != Components.Keys.Count)
                        {
                            this.PrintResult(totalReached * (Components.Keys.Count - totalReached));
                            return;
                        }

                        Activate(c3);
                        Activate(c2);
                        Activate(c);
                    }
                }
            }

            void Inactivate(string connection)
            {
                var tokens = connection.Split('|');
                var from = tokens[0];
                var to = tokens[1];
                Components[from].InactiveConnection = to;
                Components[to].InactiveConnection = from;
            }

            void Activate(string connection)
            {
                var tokens = connection.Split('|');
                var from = tokens[0];
                var to = tokens[1];
                Components[from].InactiveConnection = null;
                Components[to].InactiveConnection = null;
            }
        }

        private long ShortestPath(Component from, bool countEdges, bool saveEdges)
        {
            var queue = new Queue<(Component comp, int dist, HashSet<string> visited)>();
            var visitedBig = new HashSet<string>();
            queue.Enqueue((from, 0, new HashSet<string>()));
            while (queue.Any())
            {
                var (comp, dist, visited) = queue.Dequeue();

                if (countEdges && !saveEdges)
                {
                    if (dist > LongestDist)
                    {
                        LongestDist = dist;
                        LongestPair = (from, comp);
                    }
                }

                if (saveEdges && comp.Name == LongestPair.to.Name)
                {
                    this.LongestConnectionEdges = visited;
                    return 0;
                }

                if (countEdges)
                {
                    foreach (var v in visited)
                    {
                        if (!TimesPassedEachConnection.ContainsKey(v))
                        {
                            TimesPassedEachConnection.Add(v, 0);
                        }

                        TimesPassedEachConnection[v]++;
                    }
                }

                foreach (var c in comp.ActiveConnections.Where(x => !visitedBig.Contains(x.Name)))
                {
                    HashSet<string> copy = null;
                    if (countEdges)
                    {
                        copy = new HashSet<string>(visited)
                        {
                            StringifyConnection(comp, c)
                        };
                    }
                  
                    visitedBig.Add(c.Name);
                    queue.Enqueue((c, dist + 1, copy));
                }
            }

            return visitedBig.Count;
        }

        private string StringifyConnection(Component from, Component to) => string.Join("|", new[] { from.Name, to.Name }.OrderBy(x => x));

        private class Component
        {
            public Component(string name)
            {
                this.Name = name;
            }

            public string Name { get; set; }

            public string InactiveConnection { get; set; }

            public List<Component> Connections { get; set; } = new List<Component>();

            public IEnumerable<Component> ActiveConnections => Connections.Where(x => x.Name != InactiveConnection);
        }
    }
}
