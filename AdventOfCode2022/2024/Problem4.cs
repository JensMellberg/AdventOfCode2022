using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem4 : StringProblem
    {
        public override void Solve(IEnumerable<string> testData)
        {
            const string FullWord = "XMAS";
            var matrix = Matrix.FromTestInput<char>(testData);
            var counter = 0;
            var counter2 = 0;
            for (var x = 0; x < matrix.ColumnCount; x++)
            {
                for (var y = 0; y < matrix.RowCount; y++)
                {
                    if (matrix[x, y] == FullWord[0])
                    {
                        counter += matrix.GetAdjacentCoordinates(x, y).Count(tup => CheckForXmas(x, y, tup.x - x, tup.y - y));
                    }
                    else if (matrix[x, y] == 'A' && matrix.IsInBounds(x - 1, y - 1) && matrix.IsInBounds(x + 1, y + 1))
                    {
                        var score1 = CharScore(matrix[x - 1, y - 1]) + CharScore(matrix[x + 1, y + 1]);
                        var score2 = CharScore(matrix[x - 1, y + 1]) + CharScore(matrix[x + 1, y - 1]);
                        if (score1 == 3 && score2 == 3)
                        {
                            counter2++;
                        }
                    }
                }
            }

            this.PrintResult(counter);
            this.PrintResult(counter2);

            int CharScore(char c) => c == 'M' ? 1 : (c == 'S' ? 2 : 0);
            bool CheckForXmas(int x, int y, int deltaX, int deltaY)
            {
                var pointer = 0;
                while (pointer < FullWord.Length && matrix.IsInBounds(x, y) && matrix[x, y] == FullWord[pointer])
                {
                    pointer++;
                    x += deltaX;
                    y += deltaY;

                    if (pointer == FullWord.Length)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
