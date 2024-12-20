using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem18 : PatternProblem<(int x, int y)>
    {
        protected override string Pattern => "¤x¤,¤y¤";

        public override void Solve(IEnumerable<(int x, int y)> testInput)
        {
            var field = Matrix.InitWithStartValue(71, 71, '.');
            foreach (var (posX, posY) in testInput.Take(1024))
            {
                field[posX, posY] = '#';
            }

            var path = GetShortestPath(field);
            this.PrintResult(path.Count);
            foreach (var (x, y) in testInput.Skip(1024))
            {
                field[x, y] = '#';
                if (path.Contains((x, y))) {
                    path = GetShortestPath(field);
                    if (path == null)
                    {
                        this.PrintResult($"{x},{y}");
                        return;
                    }
                }
            }
        }

        private HashSet<(int x, int y)> GetShortestPath(Matrix<char> field)
        {
            var queue = new Queue<(int x, int y, int dist, HashSet<(int x, int y)> visitedLocal)>();
            queue.Enqueue((0, 0, 0, new HashSet<(int x, int y)>()));
            var visited = new HashSet<(int x, int y)>();
            while (queue.Any())
            {
                var (x, y, dist, visitedLocal) = queue.Dequeue();
                if (!visited.Add((x, y)))
                {
                    continue;
                }

                visitedLocal.Add((x, y));
                if (x == 70 && y == 70)
                {
                    return visitedLocal;
                }

                foreach (var (x2, y2) in field.GetAdjacentCoordinates(x, y).Where(m => field[m.x, m.y] == '.'))
                {
                    queue.Enqueue((x2, y2, dist + 1, visitedLocal.ToHashSet()));
                }
            }

            return null;
        }
    }
}
