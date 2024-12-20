using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using AdventOfCode2022.TwentyTwo;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem15 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public override void Solve(IEnumerable<string> testInput)
        {
            var testList = testInput.ToList();
            var matrixPart = new List<string>
            {
                testList[0]
            };

            var index = 1;
            while (testList[index].Any(x => x != '#')) {
                matrixPart.Add(testList[index]);
                index++;
            }

            matrixPart.Add(testList[index]);
            var instructions = string.Join("", testList.Skip(index + 1));
            var matrix = Matrix.FromTestInput<char>(matrixPart);
            var wideMatrix = Matrix.InitWithStartValue(matrix.RowCount, matrix.ColumnCount * 2, '.');
            for (var x = 0; x < matrix.ColumnCount; x++)
            {
                for (var y = 0; y < matrix.RowCount; y++)
                {
                    if (matrix[x, y] == '#')
                    {
                        wideMatrix[x * 2, y] = '#';
                        wideMatrix[x * 2 + 1, y] = '#';
                    }
                    else if (matrix[x, y] == 'O')
                    {
                        wideMatrix[x * 2, y] = '[';
                        wideMatrix[x * 2 + 1, y] = ']';
                    }
                    else
                    {
                        wideMatrix[x * 2, y] = matrix[x, y];
                    }
                }
            }

            this.Part1(matrix, instructions);
            this.Part2(wideMatrix, instructions);
        }

        private void Part1(Matrix<char> matrix, IEnumerable<char> instructions)
        {
            var playerPos = matrix.Find('@');
            matrix[playerPos.x, playerPos.y] = '.';
            foreach (var instr in instructions)
            {
                var delta = DirectionFromChar(instr).GetDelta();
                (int x, int y) target = (playerPos.x + delta.x, playerPos.y + delta.y);
                while (matrix[target.x, target.y] == 'O')
                {
                    target.x += delta.x;
                    target.y += delta.y;
                }

                if (matrix[target.x, target.y] == '#')
                {
                    continue;
                }

                while (target != playerPos)
                {
                    matrix[target.x, target.y] = matrix[target.x - delta.x, target.y - delta.y];
                    target.x -= delta.x;
                    target.y -= delta.y;
                }

                playerPos.x += delta.x;
                playerPos.y += delta.y;
            }

            this.PrintResult(this.CalculateGpsScore(matrix));
        }

        private void Part2(Matrix<char> matrix, IEnumerable<char> instructions)
        {
            var playerPos = matrix.Find('@');
            matrix[playerPos.x, playerPos.y] = '.';
            foreach (var instr in instructions)
            {
                var delta = DirectionFromChar(instr).GetDelta();
                (int x, int y) target = (playerPos.x + delta.x, playerPos.y + delta.y);
                
                if (matrix[target.x, target.y] == '[' || matrix[target.x, target.y] == ']')
                {
                    var boxesToMove = new List<(int x, int y)>();
                    var boxesChecked = new HashSet<(int x, int y)>();
                    var boxesToCheck = new Queue<(int x, int y)>();
                    boxesToCheck.Enqueue(target);
                    boxesToCheck.Enqueue((target.x + (matrix[target.x, target.y] == '[' ? 1 : -1), target.y));
                    var mayMove = true;
                    while (boxesToCheck.Any())
                    {
                        var box = boxesToCheck.Dequeue();
                        if (!boxesChecked.Add(box))
                        {
                            continue;
                        }

                        boxesToMove.Add(box);
                        (int x, int y) newTarget = (box.x + delta.x, box.y + delta.y);

                        if (matrix[box.x, box.y] == '[' && delta.x == 1
                            || matrix[box.x, box.y] == ']' && delta.x == -1)
                        {
                            //The box is being pushed horizontally so we don't need to check for both its sides.
                            continue;
                        }

                        if (matrix[newTarget.x, newTarget.y] == '[' || matrix[newTarget.x, newTarget.y] == ']')
                        {
                            boxesToCheck.Enqueue(newTarget);
                            boxesToCheck.Enqueue((newTarget.x + (matrix[newTarget.x, newTarget.y] == '[' ? 1 : -1), newTarget.y));
                        }
                        else if (matrix[newTarget.x, newTarget.y] == '#')
                        {
                            mayMove = false;
                            break;
                        }
                    }

                    if (mayMove)
                    {
                        boxesToMove.Reverse();
                        foreach (var box in boxesToMove)
                        {
                            matrix[box.x + delta.x, box.y + delta.y] = matrix[box.x, box.y];
                            matrix[box.x, box.y] = '.';
                        }
                    }
                    else
                    {
                        continue;
                    }
                }

                if (matrix[target.x, target.y] == '#')
                {
                    continue;
                }

                playerPos.x += delta.x;
                playerPos.y += delta.y;
            }

            this.PrintResult(this.CalculateGpsScore(matrix));
        }

        private long CalculateGpsScore(Matrix<char> field)
        {
            long gpsScore = 0;
            for (var x = 0; x < field.ColumnCount; x++)
            {
                for (var y = 0; y < field.RowCount; y++)
                {
                    if (field[x, y] == 'O' || field[x, y] == '[')
                    {
                        gpsScore += 100 * y + x;
                    }
                }
            }

            return gpsScore;
        }

        private static Direction DirectionFromChar(char c)
        {
            switch(c)
            {
                case '^': return Direction.Up;
                case '<': return Direction.Left;
                case '>': return Direction.Right;
                case 'v': return Direction.Down;
                default: throw new Exception($"Invalid direction {c}");
            }
        }
    }
}
