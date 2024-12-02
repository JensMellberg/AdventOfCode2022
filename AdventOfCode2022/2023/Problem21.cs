using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Schema;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem21 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        private Func<Point, Matrix<char>> GenerateNewMatrix;

        public static int width;

        private int height;

        public static int radius => (width - 1) / 2;

        private static long totalSteps = 26501365;

        public static int ReachableSquares;

        public static long EvenReachable;

        public static long OddReachable;

        private Dictionary<(int x, int y, int stepsLeft, bool isEven), int> ReachableSquaresByEntryPoint = new Dictionary<(int x, int y, int stepsLeft, bool isEven), int>();

        public static Dictionary<(int x, int y, bool isEven), long> AllReachable = new Dictionary<(int x, int y, bool isEven), long>();

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

            AllReachable.Clear();
            var matrix = this.GenerateNewMatrix(null);
            width = matrix.ColumnCount;
            this.height = matrix.RowCount;
            ReachableSquares = matrix.AllValues().Count(x => x == '.' || x == 'S') - 2;

            var map = new MapResult(matrix);
            map.SetupMatrix(64);
            this.PrintResult(map.ReachableCount());

            this.Part2();
        }

        private void Part2()
        {
            new[]
            {
                new Point(0, 0),
                new Point(0, this.height - 1),
                new Point(width - 1, 0),
                new Point(width - 1, this.height - 1),
                new Point(width - 1, radius),
                new Point(0, radius),
                new Point(radius, height - 1),
                new Point(radius, 0)
            }.ForEach(x =>
            {
                var matrix = this.GenerateNewMatrix(x);
                var map = new MapResult(matrix);
                map.SetupMatrix(300);
                map.ReachableStepsByBreakpoint.ForEach(
                    e => this.ReachableSquaresByEntryPoint
                    .Add((x.X, x.Y, e.Key.breakPoint, e.Key.isEven), (int)e.Value));
            });

            EvenReachable = AllReachable[AllReachable.Keys.First(x => x.isEven)];
            OddReachable = AllReachable[AllReachable.Keys.First(x => !x.isEven)];

            // How many full matrices (not including middle) you can pass.
            var reachout = (totalSteps - 66) / width;
            var middleRow = (reachout + 1) * EvenReachable + reachout * OddReachable;
            var reached = middleRow;

            reached += this.ReachableSquaresByEntryPoint[(width - 1, radius, 130, true)];
            reached += this.ReachableSquaresByEntryPoint[(0, radius, 130, true)];
            reached += this.ReachableSquaresByEntryPoint[(radius, height - 1, 130, true)];
            reached += this.ReachableSquaresByEntryPoint[(radius, 0, 130, true)];
            
            var reachoutNew = reachout;
            var middleIsEven = true;
            while (reachoutNew > 0)
            {
                reachoutNew--;
                long addedPerRow = 0;
                if (reachoutNew % 2 == 0)
                {
                    addedPerRow = reachoutNew * EvenReachable + reachoutNew * OddReachable + EvenReachable;
                }
                else
                {
                    addedPerRow += (reachoutNew + 1) * EvenReachable + reachoutNew * OddReachable;
                }

                reached += addedPerRow * 2;
                reached += this.ReachableSquaresByEntryPoint[(width - 1, height - 1, 64, true)];
                reached += this.ReachableSquaresByEntryPoint[(width - 1, height - 1, 195, false)];
                reached += this.ReachableSquaresByEntryPoint[(0, height - 1, 64, true)];
                reached += this.ReachableSquaresByEntryPoint[(0, height - 1, 195, false)];

                reached += this.ReachableSquaresByEntryPoint[(width - 1, 0, 64, true)];
                reached += this.ReachableSquaresByEntryPoint[(width - 1, 0, 195, false)];
                reached += this.ReachableSquaresByEntryPoint[(0, 0, 64, true)];
                reached += this.ReachableSquaresByEntryPoint[(0, 0, 195, false)];

                middleIsEven = !middleIsEven;
            }

            reached += this.ReachableSquaresByEntryPoint[(width - 1, height - 1, 64, true)];
            reached += this.ReachableSquaresByEntryPoint[(0, height - 1, 64, true)];

            reached += this.ReachableSquaresByEntryPoint[(width - 1, 0, 64, true)];
            reached += this.ReachableSquaresByEntryPoint[(0, 0, 64, true)];

            this.PrintResult(reached);
        }

        private class MapResult
        {
            private bool setupWasEven;
            public Matrix<char> Matrix { get; set; }

            public Dictionary<(int x, int y, bool isEven), int> Memory = new Dictionary<(int x, int y, bool isEven), int>();

            public Dictionary<(int breakPoint, bool isEven), long> ReachableStepsByBreakpoint = new Dictionary<(int breakPoint, bool isEven), long>();

            public MapResult(Matrix<char> matrix)
            {
                this.Matrix = matrix;
            }

            public long ReachableCount()
            {
                return this.Memory.Keys.Where(x => x.isEven == this.setupWasEven).Count();
            }

            public void SetupMatrix(int stepsCount)
            {
                var matrix = this.Matrix;
                var steps = new List<(int x, int y, int steps)>();
                var (sX, sY) = matrix.Find('S');
                steps.Add((sX, sY, 0));
                this.setupWasEven = stepsCount % 2 == 0;
                var breakPoints = new[] { 130, 64, 195 };
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
                        if (this.Memory.Count == ReachableSquares)
                        {
                            AllReachable.Add((sX, sY, true), this.Memory.Keys.Where(x => x.isEven).Count());
                            AllReachable.Add((sX, sY, false), this.Memory.Keys.Where(x => !x.isEven).Count());
                            return;
                        }

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

                    if (breakPoints.Any(x => x == i))
                    {
                        this.ReachableStepsByBreakpoint.Add((i, true), this.Memory.Keys.Where(x => x.isEven).Count());
                        this.ReachableStepsByBreakpoint.Add((i, false), this.Memory.Keys.Where(x => !x.isEven).Count());
                    }
                }
            }
        }
    }
}
