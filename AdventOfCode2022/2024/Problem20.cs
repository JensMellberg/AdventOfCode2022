using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem20 : StringProblem
    {
        public override void Solve(IEnumerable<string> testInput)
        {
            var matrix = Matrix.FromTestInput<char>(testInput);
            var distFromEnd = Matrix.InitWithStartValue(matrix.RowCount, matrix.ColumnCount, -1);
            var start = matrix.Find('S');
            var end = matrix.Find('E');
            var queue = new Queue<(int x, int y, int dist)>();
            queue.Enqueue((end.x, end.y, 0));
            while (queue.Any())
            {
                var (x, y, dist) = queue.Dequeue();
                if (distFromEnd[x, y] != -1)
                {
                    continue;
                }

                distFromEnd[x, y] = dist;
                foreach (var (xx, yy) in matrix.GetAdjacentCoordinates(x, y).Where(m => matrix[m.x, m.y] != '#'))
                {
                    queue.Enqueue((xx, yy, dist + 1));
                }
            }

            long part1 = 0;
            long part2 = 0;
            for (var x = 0; x < matrix.ColumnCount; x++)
            {
                for (var y = 0; y < matrix.RowCount; y++)
                {
                    var startLength = distFromEnd[x, y];
                    if (startLength == -1)
                    {
                        continue;
                    }

                    part1 += CheatsFromPoint(x, y, 2);
                    part2 += CheatsFromPoint(x, y, 20);
                }
            }

            this.PrintResult(part1);
            this.PrintResult(part2);
            
            long CheatsFromPoint(int x, int y, int cheatDistance)
            {
                long result = 0;
                var startLength = distFromEnd[x, y];
                var ends = new HashSet<(int x, int y)>();
                for (var x2 = x - cheatDistance; x2 <= x + cheatDistance; x2++)
                {
                    for (var y2 = y - cheatDistance; y2 <= y + cheatDistance; y2++)
                    {
                        if (!matrix.IsInBounds(x2, y2) || matrix[x2, y2] == '#')
                        {
                            continue;
                        }

                        var xDiff = Math.Abs(x - x2);
                        var yDiff = Math.Abs(y - y2);
                        var stepBreakpoint = 100;
                        var stepsTaken = xDiff + yDiff;
                        var stepsSaved = startLength - distFromEnd[x2, y2] - stepsTaken;
                        if (stepsTaken <= cheatDistance && stepsSaved >= stepBreakpoint && ends.Add((x2, y2)))
                        {
                            result++;
                        }
                    }
                }

                return result;
            }
        }
    }
}
