using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem16 : StringProblem
    {
        public override void Solve(IEnumerable<string> testInput)
        {
            var matrix = Matrix.FromTestInput<char>(testInput);
            var start = matrix.Find('S');
            var end = matrix.Find('E');
            matrix[end.x, end.y] = '.';
            matrix[start.x, start.y] = '.';
            var queue = new Queue<(int x, int y, Direction dir, long score, HashSet<(int x, int y)> path)>();
            var visited = new Dictionary<(int x, int y), long>();
            var initial = new HashSet<(int x, int y)>
            {
                start
            };

            queue.Enqueue((start.x, start.y, Direction.Right, 0, initial));
            var bestScore = long.MaxValue;
            var bestPaths = new HashSet<(int x, int y)>();
            while (queue.Any())
            {
                var (x, y, dir, score, path) = queue.Dequeue();
                if (x == 3 && y == 9)
                {
                    var a = 5;
                }
                if ((x, y) == end)
                {
                    if (score < bestScore)
                    {
                        bestPaths = path;
                        bestScore = score;
                    }
                    else if (score == bestScore)
                    {
                        path.ForEach(x => bestPaths.Add(x));
                    }

                    continue;
                }

                if (visited.TryGetValue((x, y), out var prevScore)) {
                    if (prevScore < score - 1000)
                    {
                        continue;
                    }
                    else
                    {
                        visited[(x, y)] = score;
                    }
                }
                else
                {
                    visited.Add((x, y), score);
                }

                var currentDelta = dir.GetDelta();
                var turnedDelta = dir.TurnClockwise().GetDelta();
                var turnedDelta2 = dir.TurnClockwise().Reverse().GetDelta();
                TryAdd(x + currentDelta.x, y + currentDelta.y, dir, score + 1);
                TryAdd(x + turnedDelta.x, y + turnedDelta.y, dir.TurnClockwise(), score + 1001);
                TryAdd(x + turnedDelta2.x, y + turnedDelta2.y, dir.TurnClockwise().Reverse(), score + 1001);

                void TryAdd(int xx, int yy, Direction newDir, long newScore)
                {
                    if (matrix.IsInBounds(xx, yy) && matrix[xx, yy] == '.')
                    {
                        var newPath = path.ToHashSet();
                        newPath.Add((x, y));
                        queue.Enqueue((xx, yy, newDir, newScore, newPath));
                    }
                }
            }

            bestPaths.Add(end);
            this.PrintResult(bestScore);
            this.PrintResult(bestPaths.Count);
        }
    }
}
