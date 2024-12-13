using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem12 : StringProblem
    { 
        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<char>(testData);
            var visited = new HashSet<(int x, int y)>();
            long total1 = 0;
            long total2 = 0;
            for (var y = 0; y < matrix.RowCount; y++)
            {
                for (var x = 0; x < matrix.ColumnCount; x++)
                {
                    if (!visited.Contains((x, y)))
                    {
                        var (score1, score2) = Visit(x, y);
                        total1 += score1;
                        total2 += score2;
                    }
                }
            }

            this.PrintResult(total1);
            this.PrintResult(total2);

            (long score1, long score2) Visit(int startX, int startY)
            {
                var queue = new Queue<(int x, int y, Direction? dir)>();
                var letter = matrix[startX, startY];
                queue.Enqueue((startX, startY, null));
                var area = 0;
                var walls = 0;
                var perimiter = 0;
                var wallConnectorSquares = new HashSet<(int x, int y, Direction dir)>();

                while (queue.Any())
                {
                    var (x, y, dir) = queue.Dequeue();
                    if (!matrix.IsInBounds(x, y) || matrix[x, y] != letter)
                    {
                        perimiter++;
                        if (!wallConnectorSquares.Contains((x, y, dir.Value)))
                        {
                            var directionFlipDelta = dir.Value.Reverse().GetDelta();
                            var clockWiseDir = dir.Value.TurnClockwise();
                            var clockWiseFlipDir = clockWiseDir.Reverse();
                            var clockWiseDelta = clockWiseDir.GetDelta();
                            var clockWiseFlipDelta = clockWiseFlipDir.GetDelta();
                            AddWholeWall(x, y, clockWiseDelta);
                            AddWholeWall(x, y, clockWiseFlipDelta);
                            walls++;

                            void AddWholeWall(int wallX, int wallY, (int x, int y) delta)
                            {
                                wallX += delta.x;
                                wallY += delta.y;

                                // Connect wall as long as it is not part of the same letter and it is walkable from the current direction.
                                while ((!matrix.IsInBounds(wallX, wallY) || matrix[wallX, wallY] != letter)
                                    && matrix.IsInBounds(wallX + directionFlipDelta.x, wallY + directionFlipDelta.y)
                                    && matrix[wallX + directionFlipDelta.x, wallY + directionFlipDelta.y] == letter)
                                {
                                    wallConnectorSquares.Add((wallX, wallY, dir.Value));
                                    wallX += delta.x;
                                    wallY += delta.y;
                                }
                            }
                        }

                        var nextDir = dir.Value.TurnClockwise();
                        var flippedDir = nextDir.Reverse();
                        var nextDelta = nextDir.GetDelta();
                        var flippedDelta = flippedDir.GetDelta();

                        continue;
                    }

                    if (!visited.Add((x, y)))
                    {
                        continue;
                    }

                    area++;
                    queue.Enqueue((x - 1, y, Direction.Left));
                    queue.Enqueue((x, y - 1, Direction.Up));
                    queue.Enqueue((x + 1, y, Direction.Right));
                    queue.Enqueue((x, y + 1, Direction.Down));
                }

                return (area * perimiter, area * walls);
            }
        }
    }
}
