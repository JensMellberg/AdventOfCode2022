using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem21 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        private Func<Point, Matrix<char>> GenerateNewMatrix;

        public static Dictionary<(int startX, int startY, bool isEven), long> KnownMapResults = new Dictionary<(int startX, int startY, bool isEven), long>();

        private static Dictionary<(int x, int y), MapResult> MapMapping = new Dictionary<(int x, int y), MapResult>();

        public override void Solve(IEnumerable<string> testData)
        {
            this.GenerateNewMatrix = (Point p) =>
            {
                var matrix = Matrix.FromTestInput<char>(testData, x => p == null ? x : (x == 'S' ? '.' : x));
                if (p != null)
                {
                    matrix[p.X, p.Y] = 'S';
                }

                return matrix;
            };

            var map = new MapResult(this.GenerateNewMatrix(null));
            map.SetupMatrix(64, false);
            this.PrintResult(map.ReachableCount());
        }

        private class MapResult
        {
            private bool setupWasEven;
            public Matrix<char> Matrix { get; set; }

            public Dictionary<(int x, int y, bool isEven), int> Memory = new Dictionary<(int x, int y, bool isEven), int>();

            public Dictionary<(int x, int y, bool isEven), (int steps, MapResult result)> OtherMaps = new Dictionary<(int x, int y, bool isEven), (int steps, MapResult result)>();

            public MapResult(Matrix<char> matrix)
            {
                this.Matrix = matrix;
            }

            public long ReachableCount()
            {
                return this.Memory.Keys.Where(x => x.isEven == this.setupWasEven).Count();
            }

            public void SetupMatrix(int stepsCount, bool isInfinite)
            {
                var matrix = this.Matrix;
                var steps = new List<(int x, int y, int steps)>();
                var (sX, sY) = matrix.Find('S');
                steps.Add((sX, sY, 0));
                this.setupWasEven = stepsCount % 2 == 0;
                for (var i = 0; i < stepsCount + 1; i++)
                {
                    var toDo = steps.ToArray();
                    steps.Clear();
                    foreach (var (x, y, stepCount) in toDo)
                    {
                        var isEven = stepCount % 2 == 0;
                        if (this.Memory.ContainsKey((x, y, isEven)))
                        {
                            continue;
                        }

                        this.Memory.Add((x, y, isEven), stepCount);
                        foreach (var dir in AllDirections)
                        {
                            var delta = dir.GetDelta();
                            var newX = x + delta.x;
                            var newY = y + delta.y;
                            if (matrix.IsInBounds(newX, newY) && !(matrix[newX, newY] == '#'))
                            {
                                steps.Add((newX, newY, stepCount + 1));
                            }
                        }
                    }

                    /* var copy = Matrix.FromTestInput<char>(testData);
                     var isEvenP = i % 2 == 0;
                     memory.Where(x => x.isEven == isEvenP).ForEach(x => copy[x.x, x.y] = 'O');
                     this.Print(copy.ToString(x => x.ToString(), ""));*/
                }
            }
        }
    }
}
