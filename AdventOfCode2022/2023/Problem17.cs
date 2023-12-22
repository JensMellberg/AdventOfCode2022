using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem17 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<int>(testData);
            this.SolveProblem(matrix, 0, 3);
            this.SolveProblem(matrix, 4, 10);
        }
      
        private void SolveProblem(Matrix<int> matrix, int minSteps, int maxSteps)
        {
            var result = Matrix.InitWithStartValue(matrix.RowCount, matrix.ColumnCount, int.MaxValue);

            var queue = new List<(int x, int y, Direction dir, int count, int total)>
            {
                (0, 0, Direction.Right, 0, 0),
                (0, 0, Direction.Down, 0, 0)
            };

            var visited = new Dictionary<(int x, int y, Direction dir, int count), int>();
            while (queue.Any())
            {
                var minIndex = MinIndex();
                (int x, int y, Direction dir, int count, int total) = queue[minIndex];
                var currentCount = count;
                var flag = false;
                while (currentCount > minSteps || currentCount == count)
                {
                    if (HasLower(currentCount))
                    {
                        queue.RemoveAt(minIndex);
                        flag = true;
                        break;
                    }

                    currentCount--;
                }

                if (flag)
                {
                    continue;
                }

                if (visited.ContainsKey((x, y, dir, count)))
                {
                    visited[(x, y, dir, count)] = total;
                }
                else
                {
                    visited.Add((x, y, dir, count), total);
                }

                queue.RemoveAt(minIndex);
                if (x == matrix.RowCount - 1 && y == matrix.ColumnCount - 1 && count < minSteps)
                {
                    continue;
                }

                result[x, y] = Math.Min(total, result[x, y]);
                if (count < minSteps)
                {
                    var delta = dir.GetDelta();
                    TryAdd(x + delta.x, y + delta.y, dir);
                    continue;
                }

                TryAdd(x - 1, y, Direction.Left);
                TryAdd(x + 1, y, Direction.Right);
                TryAdd(x, y - 1, Direction.Up);
                TryAdd(x, y + 1, Direction.Down);

                bool HasLower(int countX)
                {
                    if (visited.TryGetValue((x, y, dir, countX), out var totalX))
                    {
                        return totalX <= total;
                    }

                    return false;
                }

                void TryAdd(int xx, int yy, Direction dirr)
                {
                    if (dirr != dir.Reverse() && matrix.IsInBounds(xx, yy) && !(dir == dirr && count == maxSteps))
                    {
                        var destinationValue = matrix[xx, yy];
                        queue.Add((xx, yy, dirr, dir == dirr ? count + 1 : 1, total + destinationValue));
                    }
                }

                int MinIndex()
                {
                    var minIndex = -1;
                    var minValue = int.MaxValue;
                    for (var i = 0; i < queue.Count; i++)
                    {
                        if (queue[i].total < minValue)
                        {
                            minIndex = i;
                            minValue = queue[i].total;
                        }
                    }

                    return minIndex;
                }
            }

            this.PrintResult(result[matrix.ColumnCount - 1, matrix.RowCount - 1]);
        }
    }
}
