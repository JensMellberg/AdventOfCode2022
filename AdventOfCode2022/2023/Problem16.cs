using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem16 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<char>(testData);
           
            this.PrintResult(EnergizedSquares(matrix, new Beam { X = 0, Y = 0, Direction = Direction.Right }));
            var best = 0;
            for (var i = 0; i < matrix.ColumnCount; i++)
            {
                best = Math.Max(best, EnergizedSquares(matrix, new Beam { X = i, Y = 0, Direction = Direction.Down }));
                best = Math.Max(best, EnergizedSquares(matrix, new Beam { X = i, Y = matrix.ColumnCount - 1, Direction = Direction.Up }));
            }

            for (var i = 0; i < matrix.RowCount; i++)
            {
                best = Math.Max(best, EnergizedSquares(matrix, new Beam { X = 0, Y = i, Direction = Direction.Right }));
                best = Math.Max(best, EnergizedSquares(matrix, new Beam { X = matrix.ColumnCount - 1, Y = i, Direction = Direction.Left }));
            }

            this.PrintResult(best);
        }

        private int EnergizedSquares(Matrix<char> matrix, Beam initial)
        {
            var isEnergized = Matrix.InitWithStartValue(matrix.RowCount, matrix.ColumnCount, false);
            var visited = new HashSet<(int x, int y, Direction direction)>();
            var beams = new List<Beam>
            {
                initial
            };

            var isFirst = true;
            while (beams.Any())
            {
                for (var i = 0; i < beams.Count; i++)
                {
                    if (!beams[i].DoMove(isFirst, matrix, beams, isEnergized, visited))
                    {
                        i--;
                    }

                    isFirst = false;
                }

                //this.Print(isEnergized.ToString((x) => x ? "#" : ".", ""));
                //Console.ReadLine();
            }

            return isEnergized.AllValues().Sum(x => x ? 1 : 0);
        }


        private class Beam
        {
            public Direction Direction { get; set; }

            public int X { get; set; }

            public int Y { get; set; }

            public bool DoMove(bool isFirst, Matrix<char> matrix, List<Beam> beams, Matrix<bool> energizedSquares, HashSet<(int x, int y, Direction direction)> visited)
            {
                if (visited.Contains((X, Y, Direction))) {
                    beams.Remove(this);
                    return false;
                }

                energizedSquares[X, Y] = true;
                if (!isFirst)
                {
                    visited.Add((X, Y, Direction));
                    var delta = this.Direction.GetDelta();
                    this.X += delta.x;
                    this.Y += delta.y;
                }
                
                var isHorizontal = this.Direction.IsHorizontal();
                if (!matrix.IsInBounds(this.X, this.Y))
                {
                    beams.Remove(this);
                    return false;
                }

                var current = matrix[X, Y];
                if (current == '|' && isHorizontal)
                {
                    Direction = Direction.Up;
                    beams.Add(new Beam { X = this.X, Y = this.Y, Direction = Direction.Down });
                }
                else if (current == '-' && !isHorizontal)
                {
                    Direction = Direction.Left;
                    beams.Add(new Beam { X = this.X, Y = this.Y, Direction = Direction.Right });
                }
                else if (current == '\\')
                {
                    if (Direction == Direction.Left) Direction = Direction.Up;
                    else if (Direction == Direction.Right) Direction = Direction.Down;
                    else if (Direction == Direction.Up) Direction = Direction.Left;
                    else if (Direction == Direction.Down) Direction = Direction.Right;
                }
                else if (current == '/')
                {
                    if (Direction == Direction.Left) Direction = Direction.Down;
                    else if (Direction == Direction.Right) Direction = Direction.Up;
                    else if (Direction == Direction.Up) Direction = Direction.Right;
                    else if (Direction == Direction.Down) Direction = Direction.Left;
                }

                return true;
            }
        }
    }
}
