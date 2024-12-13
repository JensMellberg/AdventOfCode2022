using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http.Headers;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem6 : StringProblem
    {
        public override void Solve(IEnumerable<string> testInput)
        {
            var matrix = Matrix.FromTestInput<char>(testInput);
            var walls = new WallPositions(matrix);
            var position = matrix.Find('^');
            var direction = Direction.Up;
            
            var visitedWithDirection = new HashSet<(int x, int y, Direction dir)>();
            var visitedCount = GetVisitedCount(matrix, walls, position, direction, visitedWithDirection, out var loops);

            this.PrintResult(visitedCount);
            this.PrintResult(loops);
        }

        private int GetVisitedCount(
            Matrix<char> matrix,
            WallPositions walls,
            (int x, int y) startPos,
            Direction startDirection,
            HashSet<(int x, int y, Direction dir)> visitedWithDirection,
            out int loops)
        {
            var visitedCount = 0;
            var direction = startDirection;
            var position = startPos;
            var visited = new HashSet<(int x, int y)>();
            var wallsTried = new HashSet<(int x, int y)>();
            loops = 0;
            while (matrix.IsInBounds(position.x, position.y))
            {
                if (visited.Add(position))
                {
                    visitedCount++;
                }

                var delta = direction.GetDelta();
                (int x, int y) nextPos = (position.x + delta.x, position.y + delta.y);
                if (!matrix.IsInBounds(nextPos.x, nextPos.y))
                {
                    break;
                }

                var nextIsWall = matrix[nextPos.x, nextPos.y] == '#';
                if (nextIsWall)
                {
                    visitedWithDirection.Add((position.x, position.y, direction));
                    direction = direction.TurnClockwise();
                }
                else
                {
                    if (nextPos != startPos && !wallsTried.Contains(nextPos))
                    {
                        wallsTried.Add(nextPos);
                        var newWalls = walls.MakeWall(nextPos);
                        if (this.IsLoop(matrix, newWalls, position, direction, visitedWithDirection.ToHashSet()))
                        {
                            loops++;
                        }
                    }

                    position = nextPos;
                }
            }

            return visitedCount;
        }

        private bool IsLoop(
            Matrix<char> matrix,
            WallPositions walls,
            (int x, int y) startPos,
            Direction startDirection,
            HashSet<(int x, int y, Direction dir)> visitedWithDirection)
        {
            var direction = startDirection;
            var position = startPos;
            while (true)
            {
                if (!visitedWithDirection.Add((position.x, position.y, direction)))
                {
                    return true;
                }

                direction = direction.TurnClockwise();
                if (direction.IsVertical())
                {
                    var colWalls = walls.WallsByColumn[position.x];
                    if (direction == Direction.Up)
                    {
                        var reversed = colWalls.ToList();
                        reversed.Reverse();
                        var coord = reversed.Where(x => x < position.y).FirstOrDefault();
                        if (coord == -1)
                        {
                            return false;
                        }

                        position.y = coord + 1; 
                    }
                    else
                    {
                        var coord = colWalls.Where(x => x > position.y).FirstOrDefault();
                        if (coord == 0)
                        {
                            return false;
                        }

                        position.y = coord - 1;
                    }
                }
                else
                {
                    var rowWalls = walls.WallsByRow[position.y];
                    if (direction == Direction.Left)
                    {
                        var reversed = rowWalls.ToList();
                        reversed.Reverse();
                        var coord = reversed.Where(x => x < position.x).FirstOrDefault();
                        if (coord == -1)
                        {
                            return false;
                        }

                        position.x = coord + 1;
                    }
                    else
                    {
                        var coord = rowWalls.Where(x => x > position.x).FirstOrDefault();
                        if (coord == 0)
                        {
                            return false;
                        }

                        position.x = coord - 1;
                    }
                }
            }
        }

        private class WallPositions
        {
            public List<int>[] WallsByColumn;

            public List<int>[] WallsByRow;

            public WallPositions(Matrix<char> original)
            {
                WallsByColumn = new List<int>[original.ColumnCount];
                WallsByRow = new List<int>[original.RowCount];
                for (var i = 0; i < WallsByColumn.Length; i++)
                {
                    WallsByColumn[i] = new List<int>
                    {
                        -1
                    };
                }

                for (var i = 0; i < WallsByRow.Length; i++)
                {
                    WallsByRow[i] = new List<int>
                    {
                        -1
                    };
                }

                for (var x = 0; x < original.ColumnCount; x++)
                {
                    for (var y = 0; y < original.RowCount; y++)
                    {
                        if (original[x, y] == '#')
                        {
                            WallsByColumn[x].Add(y);
                            WallsByRow[y].Add(x);
                        }
                    }
                }
            }

            public WallPositions() { }

            public WallPositions MakeWall((int x, int y) wall)
            {
                var newCols = WallsByColumn.ToArray();
                var newRows = WallsByRow.ToArray();
                newCols[wall.x] = newCols[wall.x].ToList();
                newCols[wall.x].Add(wall.y);
                newCols[wall.x].Sort();
                newRows[wall.y] = newRows[wall.y].ToList();
                newRows[wall.y].Add(wall.x);
                newRows[wall.y].Sort();
                return new WallPositions { WallsByColumn = newCols, WallsByRow = newRows };
            }
        }
    }
}
