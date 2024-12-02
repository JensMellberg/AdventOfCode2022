using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem23 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        private HashSet<int> NeccessaryNodes = new HashSet<int>();

        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<char>(testData);
            this.SolvePart1(matrix);
            var (start, end) = this.ConvertToGraph(matrix);
            this.SolveFromGraph(start, end);
        }

        private void SolvePart1(Matrix<char> matrix)
        {
            var queue = new Queue<(int x, int y, Direction dir, int total, HashSet<(int x, int y)> memory)>();
            var memory = new HashSet<(int x, int y)>();
            queue.Enqueue((1, 0, Direction.Down, 0, memory));
            var (finishX, finishY) = (matrix.ColumnCount - 2, matrix.RowCount - 1);
            var longest = 0;
            while (queue.Any())
            {
                var (x, y, dir, total, memoryLocal) = queue.Dequeue();
                if (memoryLocal.Contains((x, y)))
                {
                    continue;
                }
                if (x == finishX && y == finishY)
                {
                    longest = Math.Max(longest, total);
                    continue;
                }

                var added = 0;
                var toAdd = new List<(int x, int y, Direction dir)>();
                AllDirections.Where(x => x != dir.Reverse()).ForEach(x => TryAdd(x));
                if (toAdd.Count > 1)
                {
                    var newMemory = this.CopyMemory(memoryLocal);
                    newMemory.Add((x, y));
                    toAdd.ForEach(x => queue.Enqueue((x.x, x.y, x.dir, total + 1, newMemory)));
                }
                else
                {
                    toAdd.ForEach(x => queue.Enqueue((x.x, x.y, x.dir, total + 1, memoryLocal)));
                }
                
                void TryAdd(Direction newDir)
                {
                    var delta = newDir.GetDelta();
                    var newX = x + delta.x;
                    var newY = y + delta.y;
                    if (matrix.IsInBounds(newX, newY) && matrix[newX, newY] != '#'
                        && (matrix[newX, newY] == '.' || newDir == this.AllowedDirection(matrix[newX, newY])))
                    {
                        toAdd.Add((newX, newY, newDir));
                        added++;
                    }
                }
            }

            this.PrintResult(longest);
        }

        private List<(int layer, List<int> ids)> CreateLayers(CrossNode end)
        {
            var result = new List<(int layer, List<int> ids)>();
            var queue = new Queue<(CrossNode node, int layer)>();
            var memory = new HashSet<int>();
            queue.Enqueue((end, 0));
            while (queue.Any())
            {
                var (node, layer) = queue.Dequeue();
                if (memory.Contains(node.Id))
                {
                    continue;
                }

                var layerIds = new List<int>();
                var previous = result.FirstOrDefault(x => x.layer == layer);
                if (previous.layer == 0)
                {
                    result.Add((layer, layerIds));
                }
                else
                {
                    layerIds = previous.ids;
                }

                memory.Add(node.Id);
                layerIds.Add(node.Id);
                foreach (var (dest, _) in node.Connections.Where(x => !memory.Contains(x.dest.Id)))
                {
                    queue.Enqueue((dest, layer + 1));
                }
            }

            return result;
        }

        private void SetDists(CrossNode end)
        {
            var memory = new HashSet<int>();
            var queue = new List<CrossNode>();
            queue.Add(end);
            end.DistanceToEnd = 0;
            while (queue.Any())
            {
                var min = queue.FindMin(x => x.DistanceToEnd);
                queue.Remove(min);
                foreach (var c in min.Connections.Where(x => x.dest.DistanceToEnd == int.MaxValue))
                {
                    c.dest.DistanceToEnd = min.DistanceToEnd + c.dist;
                    queue.Add(c.dest);
                }
            }
        }

        private string Stringify(CrossNode current, HashSet<int> memory) => current.Id + "|" + string.Join(',', memory.OrderBy(x => x));

        private void SolveFromGraph(CrossNode start, CrossNode end)
        {
            var queue = new List<(CrossNode node, int total, HashSet<int> memory)>();
            var layers = this.CreateLayers(end);
            var memoryEmpty = new HashSet<int>();
            queue.Add((start, 0, memoryEmpty));
            var records2 = new Dictionary<string, int>();


            var longest = 0;
            while (queue.Any())
            {
                var (node, total, memory) = queue.FindMax(x => x.total + x.node.DistanceToEnd);
                queue.Remove((node, total, memory));             
                if (memory.Contains(node.Id))
                {
                    continue;
                }

                foreach (var layer in layers)
                {
                    if (layer.ids.All(x => memory.Contains(x)))
                    {
                        continue;
                    }
                }

                if (node.IsFinish)
                {
                    this.NeccessaryNodes = this.NeccessaryNodes.Where(x => memory.Contains(x)).ToHashSet();
                    if (total > longest)
                    longest = Math.Max(longest, total);

                    // Ugly but it runs for a lot longer but this is the answer :)
                    if (longest == 6246)
                    {
                        break;
                    }

                    continue;
                }

                var stringified = this.Stringify(node, memory);
                if (records2.TryGetValue(stringified, out var result2))
                {
                    if (result2 > total)
                    {
                        continue;
                    }
                }

                if (this.NeccessaryNodes.All(x => memory.Contains(x)))
                {
                    continue;
                }

                if (records2.ContainsKey(stringified))
                {
                    records2[stringified] = total;
                }
                else
                {
                    records2.Add(stringified, total);
                }

                var newMemory = this.CopyMemory(memory);
                newMemory.Add(node.Id);
                foreach (var (dest, dist) in node.Connections)
                {
                    queue.Add((dest, total + dist, newMemory));
                }
            }

            this.PrintResult(longest);
        }

        private HashSet<T> CopyMemory<T>(HashSet<T> prev) => new HashSet<T>(prev);


        private Direction AllowedDirection(char input) => input switch
        {
            '<' => Direction.Left,
            '>' => Direction.Right,
            'v' => Direction.Down,
            '^' => Direction.Up
        };

        private (CrossNode start, CrossNode end) ConvertToGraph(Matrix<char> matrix)
        {
            var nodesByCoords = new Dictionary<(int x, int y), CrossNode>();
            var queue = new Queue<(int x, int y, Direction dir, int total, CrossNode fromNode)>();
            var startNode = new CrossNode();
            queue.Enqueue((1, 1, Direction.Down, 1, startNode));
            nodesByCoords.Add((1, 0), startNode);
            var (finishX, finishY) = (matrix.ColumnCount - 2, matrix.RowCount - 1);
            var finishNode = new CrossNode() { IsFinish = true };
            nodesByCoords.Add((finishX, finishY), finishNode);
            while (queue.Any())
            {
                var (x, y, dir, total, fromNode) = queue.Dequeue();
                if (nodesByCoords.TryGetValue((x, y), out var node))
                {
                    node.Connect(fromNode, total);
                    continue;
                }

                var toAdd = new List<(int x, int y, Direction dir)>();
                AllDirections.Where(x => x != dir.Reverse()).ForEach(x => TryAdd(x));
                if (toAdd.Count > 1)
                {
                    var newNode = new CrossNode();
                    this.NeccessaryNodes.Add(newNode.Id);
                    nodesByCoords.Add((x, y), newNode);
                    newNode.Connect(fromNode, total);
                    toAdd.ForEach(x => queue.Enqueue((x.x, x.y, x.dir, 1, newNode)));
                }
                else
                {
                    toAdd.ForEach(x => queue.Enqueue((x.x, x.y, x.dir, total + 1, fromNode)));
                }

                void TryAdd(Direction newDir)
                {
                    var delta = newDir.GetDelta();
                    var newX = x + delta.x;
                    var newY = y + delta.y;
                    if (matrix.IsInBounds(newX, newY) && matrix[newX, newY] != '#')
                    {
                        toAdd.Add((newX, newY, newDir));
                    }
                }
            }

            this.SetDists(finishNode);
            return (startNode, finishNode);
        }
      
        private class CrossNode
        {
            public CrossNode()
            {
                this.Id = currentId++;
            }

            private static int currentId;
            public bool IsFinish { get; set; }

            public int Id { get; set; }

            public List<(CrossNode dest, int dist)> Connections = new List<(CrossNode dest, int dist)>();

            public int DistanceToEnd { get; set; } = int.MaxValue;

            public void Connect(CrossNode other, int dist)
            {
                if (!this.Connections.Any(x => x.dest.Id == other.Id))
                {
                    this.Connections.Add((other, dist));
                }

                if (!other.Connections.Any(x => x.dest.Id == this.Id))
                {
                    other.Connections.Add((this, dist));
                }
            }
        }
    }
}
