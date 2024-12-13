using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem10 : StringProblem
    {
        public override void Solve(IEnumerable<string> testData)
        {
            var map = Matrix.FromTestInput<int>(testData);
            var uniqueCount = 0;
            var totalCount = 0;
            for (var x = 0; x < map.ColumnCount; x++)
            {
                for (var y = 0; y < map.RowCount; y++)
                {
                    if (map[x, y] == 0)
                    {
                        var counts = this.TrailHeadsFromStart((x, y), map);
                        uniqueCount += counts.unique;
                        totalCount += counts.total;
                    }                  
                }
            }

            this.PrintResult(uniqueCount);
            this.PrintResult(totalCount);
        }

        private (int unique, int total) TrailHeadsFromStart((int x, int y) start, Matrix<int> map)
        {
            var queue = new Queue<(int x, int y)>();
            queue.Enqueue(start);
            var count = 0;
            var uniqueCount = 0;
            var reachedHeads = new HashSet<(int x, int y)>();
            while (queue.Any())
            {
                (int x, int y) = queue.Dequeue();
                TryAdd(x - 1, y);
                TryAdd(x + 1, y);
                TryAdd(x, y - 1);
                TryAdd(x, y + 1);
                void TryAdd(int newX, int newY)
                {
                    var canWalk = map.IsInBounds(newX, newY) && map[newX, newY] == map[x, y] + 1;
                    if (canWalk && map[newX, newY] == 9)
                    {
                        count++;
                        if (reachedHeads.Add((newX, newY)))
                        {
                            uniqueCount++;
                        }
                    }
                    else if (canWalk)
                    {
                        queue.Enqueue((newX, newY));
                    }
                }
            }

            return (uniqueCount, count);
        }
    }
}
