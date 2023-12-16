using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem14 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        private readonly IDictionary<string, int> Memory = new Dictionary<string, int>();

        int CurrentRound = 0;
        
        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<char>(testData);
            var matrixCopy = Matrix.FromTestInput<char>(testData);
            this.PrintResult(this.NorthLoad(matrix));
            int result;
            while (true)
            {
                CurrentRound++;
                DoCycle(matrixCopy);
                var stringified = this.Stringyfy(matrixCopy);
                if (Memory.TryGetValue(stringified, out var previousIndex))
                {
                    result = CurrentRound - previousIndex;
                    break;
                }

                this.Memory.Add(stringified, CurrentRound);
            }

            var remainingRounds = (1000000000 - CurrentRound) % result;
            for (var i = 0; i < remainingRounds; i++)
            {
                DoCycle(matrixCopy);
            }

            this.PrintResult(this.NorthLoad(matrixCopy));
        }

        private void DoCycle(Matrix<char> matrix)
        {
            this.TiltNorth(matrix);
            this.TiltWest(matrix);
            this.TiltSouth(matrix);
            this.TiltEast(matrix);
        }

        private string Stringyfy(Matrix<char> matrix)
        {
            return string.Join("", matrix.AllValues());
        }

        private void TiltNorth(Matrix<char> original)
        {
            for (var col = 0; col < original.ColumnCount; col++)
            {
                var column = original.GetColumn(col).ToList();
                var offset = 0;
                for (var pos = 0; pos < column.Count; pos++)
                {
                    var current = column[pos];
                    if (current == 'O')
                    {
                        original[col, pos] = '.';
                        original[col, offset] = 'O';
                        offset++;
                    }
                    else if (current == '#')
                    {
                        offset = pos + 1;
                    }
                }
            }
        }

        private void TiltSouth(Matrix<char> original)
        {
            for (var col = 0; col < original.ColumnCount; col++)
            {
                var column = original.GetColumn(col).ToList();
                var offset = original.ColumnCount - 1;
                for (var pos = original.ColumnCount - 1; pos >= 0; pos--)
                {
                    var current = column[pos];
                    if (current == 'O')
                    {
                        original[col, pos] = '.';
                        original[col, offset] = 'O';
                        offset--;
                    }
                    else if (current == '#')
                    {
                        offset = pos - 1;
                    }
                }
            }
        }

        private void TiltEast(Matrix<char> original)
        {
            for (var row = 0; row < original.RowCount; row++)
            {
                var fullRow = original.GetRow(row).ToList();
                var offset = original.RowCount - 1;
                for (var pos = original.ColumnCount - 1; pos >= 0; pos--)
                {
                    var current = fullRow[pos];
                    if (current == 'O')
                    {
                        original[pos, row] = '.';
                        original[offset, row] = 'O';
                        offset--;
                    }
                    else if (current == '#')
                    {
                        offset = pos - 1;
                    }
                }
            }
        }

        private void TiltWest(Matrix<char> original)
        {
            for (var row = 0; row < original.RowCount; row++)
            {
                var fullRow = original.GetRow(row).ToList();
                var offset = 0;
                for (var pos = 0; pos < fullRow.Count; pos++)
                {
                    var current = fullRow[pos];
                    if (current == 'O')
                    {
                        original[pos, row] = '.';
                        original[offset, row] = 'O';
                        offset++;
                    }
                    else if (current == '#')
                    {
                        offset = pos + 1;
                    }
                }
            }
        }

        private long NorthLoad(Matrix<char> matrix)
        {
            long total = 0;
            for (var row = 0; row < matrix.RowCount; row++)
            {
                total += matrix.GetRow(row).Sum(x => x == 'O' ? 1 : 0) * (matrix.RowCount - row);
            }

            return total;
        }
    }
}
