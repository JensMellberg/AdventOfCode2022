using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem8 : StringProblem
    {
        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<char>(testData);
            var antennaCoords = new Dictionary<char, List<(int x, int y)>>();
            var usedCoords = new HashSet<(int x, int y)>();
            var usedResonantCoords = new HashSet<(int x, int y)>();
            var totalCoords = 0;
            var totalResonantCoords = 0;
            for (var x = 0; x < matrix.ColumnCount;x++)
            {
                for (var y = 0; y < matrix.RowCount; y++)
                {
                    var antenna = matrix[x, y];
                    if (antenna != '.')
                    {
                        if (!antennaCoords.TryGetValue(antenna, out var list))
                        {
                            list = new List<(int x, int y)>();
                            antennaCoords[antenna] = list;
                        }

                        list.Add((x, y));
                    }
                }
            }

            foreach (var key in antennaCoords.Keys)
            {
                var allCoords = antennaCoords[key];
                for (var f = 0; f < allCoords.Count; f++)
                {
                    for (var f2 = f + 1; f2 < allCoords.Count; f2++)
                    {
                        var first = allCoords[f];
                        var second = allCoords[f2];
                        TryAdd(first, true);
                        TryAdd(second, true);
                        (int x, int y) delta = (second.x - first.x, second.y - first.y);
                        (int x, int y) firstPoint = (first.x - delta.x, first.y - delta.y);
                        (int x, int y) secondPoint = (second.x + delta.x, second.y + delta.y);
                        TryAdd(firstPoint, false);
                        TryAdd(secondPoint, false);
                        while (matrix.IsInBounds(firstPoint.x, firstPoint.y))
                        {
                            firstPoint.x -= delta.x;
                            firstPoint.y -= delta.y;
                            TryAdd(firstPoint, true);
                        }

                        while (matrix.IsInBounds(secondPoint.x, secondPoint.y))
                        {
                            secondPoint.x += delta.x;
                            secondPoint.y += delta.y;
                            TryAdd(secondPoint, true);
                        }

                        void TryAdd((int x, int y) coords, bool onlyResonant)
                        {
                            if (!onlyResonant && matrix.IsInBounds(coords.x, coords.y) && usedCoords.Add(coords))
                            {
                                totalCoords++;
                            }

                            if (matrix.IsInBounds(coords.x, coords.y) && usedResonantCoords.Add(coords))
                            {
                                totalResonantCoords++;
                            }
                        }
                    }
                }
            }

            this.PrintResult(totalCoords);
            this.PrintResult(totalResonantCoords);
        }    
    }
}
