using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem26 : ObjectProblem<DigInstruction>
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public override void Solve(IEnumerable<DigInstruction> testData)
        {
            var trenches = new Dictionary<int, List<int>>();
            var trenchesSet = new HashSet<(int x, int y)>();
            var maxY = 0;
            var maxX = 0;
            var minY = 0;
            var minX = 0;
            var currentX = 0;
            var currentY = 0;
            AddTrench(0, 0);
            foreach (var instruction in testData)
            {
                var delta = instruction.Direction.GetDelta();
                for (var i = 0; i < instruction.Steps; i++)
                {
                    currentX += delta.x;
                    currentY += delta.y;
                    AddTrench(currentX, currentY);
                }
            }

            var outsideCount = 0;
            var outsideKnown = new HashSet<(int x, int y)>();
            var matrix = Matrix.InitWithStartValue(maxY - minY + 1, maxX - minX + 1, '.');
            var outsideQueue = new Queue<(int x, int y)>();
            foreach (var t in trenchesSet)
            {
                matrix[t.x - minX, t.y - minY] = '#';
            }

            this.Print(matrix.ToString(x => x.ToString(), ""));
            Console.ReadLine();
            for (var y = 0; y < matrix.RowCount; y++)
            {
                var x = 0;
                while (matrix[x, y] == '.')
                {
                    FoundOutside(x, y);
                    x++;
                }

                x = matrix.ColumnCount - 1;
                while (matrix[x, y] == '.')
                {
                    FoundOutside(x, y);
                    x--;
                }
            }

            for (var x = 0; x < matrix.ColumnCount; x++)
            {
                var y = 0;
                while (matrix[x, y] == '.')
                {
                    FoundOutside(x, y);
                    y++;
                }

                y = matrix.RowCount - 1;
                while (matrix[x, y] == '.')
                {
                    FoundOutside(x, y);
                    y--;
                }
            }

            while (outsideQueue.Any())
            {
                var (x, y) = outsideQueue.Dequeue();
                TryAdd(Direction.Left);
                TryAdd(Direction.Right);
                TryAdd(Direction.Up);
                TryAdd(Direction.Down);

                void TryAdd(Direction dir)
                {
                    var delta = dir.GetDelta();
                    var newX = x + delta.x;
                    var newY = y + delta.y;
                    if (matrix.IsInBounds(newX, newY) && !outsideKnown.Contains((newX, newY)) && matrix[newX, newY] == '.')
                    {
                        outsideQueue.Enqueue((newX, newY));
                        outsideKnown.Add((newX, newY));
                        outsideCount++;
                    }
                }
            }

            this.Print(outsideCount.ToString());
            this.PrintResult(matrix.RowCount * matrix.ColumnCount - outsideCount);
            /*var counter = 0;
            var isCounting = false;
            foreach (var k in trenches.Keys)
            {
                var sorted = trenches[k].OrderBy(x => x).Distinct().ToArray();
                var pointer = 0;
                
                while (pointer < sorted.Length)
                {
                    var target = sorted[pointer];
                    counter++;
                    pointer++;
                    while (pointer < sorted.Length)
                    {
                        var next = sorted[pointer];
                        if (next == target + 1)
                        {
                            target = next;
                            counter++;
                            pointer++;
                        }
                        else
                        {
                            if (isCounting)
                            {
                                counter += next - target - 1;
                            }

                            isCounting = !isCounting;
                            break;
                        }
                    }
                }

                isCounting = !isCounting;
                this.Print("Total: " + counter);
            }

            this.PrintResult(counter);*/

            void AddTrench(int x, int y)
            {
                trenchesSet.Add((x, y));
                maxX = Math.Max(x, maxX);
                maxY = Math.Max(y, maxY);
                minX = Math.Min(x, minX);
                minY = Math.Min(y, minY);
                if (!trenches.ContainsKey(y))
                {
                    trenches[y] = new List<int>();
                }

                trenches[y].Add(x);
            }

            void FoundOutside(int x, int y)
            {
                if (!outsideKnown.Contains((x, y)))
                {
                    outsideCount++;
                    outsideKnown.Add((x, y));
                    outsideQueue.Enqueue((x, y));
                }
            }
        }
    }
}
