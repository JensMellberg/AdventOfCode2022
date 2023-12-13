using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem11 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;
        private const int ExpandedSizePart1 = 2;
        private const int ExpandedSizePart2 = 1000000;
        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<char>(testData);
            var expandedRows = new HashSet<int>();
            var expandedCols = new HashSet<int>();
            var galaxies = new List<(int x, int y)>();
            for (var i = 0; i < matrix.RowCount; i++)
            {
                var row = matrix.GetRow(i).ToList();
                var hasGalaxy = false;
                for (var pos = 0; pos < row.Count; pos++)
                {
                    var isGalaxy = row[pos] == '#';
                    if (isGalaxy)
                    {
                        galaxies.Add((pos, i));
                    }

                    hasGalaxy = isGalaxy || hasGalaxy;
                }

                if (!hasGalaxy)
                {
                    expandedRows.Add(i);
                }
            }

            for (var i = 0; i < matrix.ColumnCount; i++)
            {
                var row = matrix.GetColumn(i);
                if (!row.Any(x => x == '#'))
                {
                    expandedCols.Add(i);
                }
            }

            this.SolveProblem(expandedRows, expandedCols, galaxies, ExpandedSizePart1);
            this.SolveProblem(expandedRows, expandedCols, galaxies, ExpandedSizePart2);
        }

        public void SolveProblem(HashSet<int> expandedRows, HashSet<int> expandedCols, List<(int x, int y)> galaxies, int expandedSize)
        {
            long totalDist = 0;
            for (var i = 0; i < galaxies.Count; i++)
            {
                for (var x = i + 1; x < galaxies.Count; x++)
                {
                    var from = galaxies[i];
                    var to = galaxies[x];
                    totalDist += DistanceFrom(from.x, from.y, to.x, to.y);
                }
            }

            this.PrintResult(totalDist);

            long DistanceFrom(int x, int y, int toX, int toY)
            {
                var dx = GetDelta(x, toX);
                var dy = GetDelta(y, toY);
                long xDist = 0, yDist = 0;
                while (x != toX)
                {
                    xDist += expandedCols.Contains(x) ? expandedSize : 1;
                    x += dx;
                }

                while (y != toY)
                {
                    yDist += expandedRows.Contains(y) ? expandedSize : 1;
                    y += dy;
                }

                return xDist + yDist;
            }

            int GetDelta(int from, int to)
            {
                if (to > from)
                {
                    return 1;
                }
                else if (from > to)
                {
                    return -1;
                }

                return 0;
            }
        }
     }
}
