using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem10Backup : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;
        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<char>(testData);
            var distance = Matrix.FromTestInput<long>(testData, _ => (long)-1);
            var mainLoop = Matrix.FromTestInput<char>(testData, _ => '.');

            var queue = new Queue<(int fromDist, int x, int y)>();
            var start = matrix.Find('S');
            queue.Enqueue((-1, start.x, start.y));
            var maxDist = 0;
            int minX = 100000, minY = 10000, maxX = 0, maxY = 0;
            while (queue.Any())
            {
                var (fromDist, x, y) = queue.Dequeue();
                var currentChar = matrix[x, y];
                maxDist = Math.Max(maxDist, fromDist + 1);
                distance[x, y] = fromDist + 1;
                mainLoop[x, y] = currentChar;
                minX = Math.Min(x, minX);
                minY = Math.Min(y, minY);
                maxX = Math.Max(x, maxX);
                maxY = Math.Max(y, maxY);
                //West
                if (x > 0 && distance[x - 1, y] == -1
                    && IsAnyOf(currentChar, 'S', '-', 'J', '7')
                    && IsAnyOf(matrix[x - 1, y], '-', 'L', 'F')) {
                    queue.Enqueue((fromDist + 1, x - 1, y));
                }
                //East
                if (x < matrix.ColumnCount - 1 && distance[x + 1, y] == -1
                    && IsAnyOf(currentChar, 'S', '-', 'L', 'F')
                    && IsAnyOf(matrix[x + 1, y], '-', 'J', '7'))
                {
                    queue.Enqueue((fromDist + 1, x + 1, y));
                }
                //North
                if (y > 0 && distance[x, y - 1] == -1
                    && IsAnyOf(currentChar, 'S', '|', 'L', 'J')
                    && IsAnyOf(matrix[x, y - 1], '|', '7', 'F'))
                {
                    queue.Enqueue((fromDist + 1, x, y - 1));
                }
                //South
                if (y < matrix.RowCount - 1 && distance[x, y + 1] == -1
                    && IsAnyOf(currentChar, 'S', '|', '7', 'F')
                    && IsAnyOf(matrix[x, y + 1], '|', 'L', 'J'))
                {
                    queue.Enqueue((fromDist + 1, x, y + 1));
                }
            }

            this.PrintResult(maxDist);


            var outsideQueue = new Queue<(int x, int y)>();
            outsideQueue.Enqueue((0, 0));
            outsideQueue.Enqueue((0, matrix.RowCount - 1));
           outsideQueue.Enqueue((matrix.ColumnCount - 1, 0));
            outsideQueue.Enqueue((matrix.ColumnCount - 1, matrix.RowCount - 1));
            //Hack to fill from middle bottom because it wasn't connected to a corner :)
            outsideQueue.Enqueue((70, matrix.RowCount - 1));
            outsideQueue.Enqueue((96, matrix.RowCount - 1));
            outsideQueue.Enqueue((112, matrix.RowCount - 1));
            var visitedOutside = new HashSet<(int x, int y)>();
            while (outsideQueue.Any())
            {
                var (x, y) = outsideQueue.Dequeue();
                mainLoop[x, y] = 'O';
                if (x > 0 && !visitedOutside.Contains((x - 1, y)) && mainLoop[x - 1, y] == '.')
                {
                    outsideQueue.Enqueue((x - 1, y));
                    visitedOutside.Add((x - 1, y));
                }

                if (x < matrix.ColumnCount - 1 && !visitedOutside.Contains((x + 1, y)) && mainLoop[x + 1, y] == '.')
                {
                    outsideQueue.Enqueue((x + 1, y));
                    visitedOutside.Add((x + 1, y));
                }

                if (y > 0 && !visitedOutside.Contains((x, y - 1)) && mainLoop[x, y - 1] == '.')
                {
                    outsideQueue.Enqueue((x, y - 1));
                    visitedOutside.Add((x, y - 1));
                }

                if (y < matrix.RowCount - 1 && !visitedOutside.Contains((x, y + 1)) && mainLoop[x, y + 1] == '.')
                {
                    outsideQueue.Enqueue((x, y + 1));
                    visitedOutside.Add((x, y + 1));
                }
            }

            this.Print(mainLoop.ToString(x => x.ToString(), ""));
            var bigQueue = new Queue<(int x, int y)>();

            var visitedOuter = new HashSet<(int x, int y)>();
            var reachedNodes = new Dictionary<(int x, int y), List<(int fromX, int fromY)>>();
            var isEnclosed = Matrix.FromTestInput<bool>(testData, _ => true);
            for (var x = 0; x < matrix.ColumnCount; x++)
            {
                for (var y = 0; y < matrix.RowCount; y++)
                {
                    if (mainLoop[x, y] == '.' && matrix[x, y] == '.')
                    {
                        bigQueue.Enqueue((x, y));
                    }

                    if (mainLoop[x, y] == 'O')
                    {
                        isEnclosed[x, y] = false;
                    }
                }
            }

            for (var x = 0; x < matrix.ColumnCount; x++)
            {
                for (var y = 0; y < matrix.RowCount; y++)
                {
                    if (matrix[x, y] != '.' && mainLoop[x, y] == '.')
                    {
                        bigQueue.Enqueue((x, y));
                    }
                }
            }

            while (bigQueue.Any())
            {
                var newQueue = new Queue<(int x, int y)>();
                var visited = new HashSet<(int x, int y)>();

                var tile = bigQueue.Dequeue();
                if (visitedOuter.Contains(tile))
                {
                    continue;
                }

                newQueue.Enqueue(tile);
                var currentIsNotEnclosed = false;
                while (newQueue.Any())
                {
                    var (x, y) = newQueue.Dequeue();
                    var currentChar = matrix[x, y];
                    visited.Add((x, y));
                    visitedOuter.Add((x, y));

                    if (currentIsNotEnclosed)
                    {
                        isEnclosed[x, y] = false;
                    }

                    if (!currentIsNotEnclosed && (x < minX || x > maxX || y < minY || y > maxY))
                    {
                        NotEnclosed();
                    }

                    //West
                    if (!currentIsNotEnclosed && x > 0 && !isEnclosed[x - 1, y])
                    {
                        NotEnclosed();
                    }

                    if (x > 0 && !visited.Contains((x - 1, y))
                        && (currentChar == '.' && matrix[x - 1, y] == '.'
                        || IsAnyOf(currentChar, '-', 'J', '7') && IsAnyOf(matrix[x - 1, y], '-', 'L', 'F')))
                    {
                        if (!currentIsNotEnclosed && !isEnclosed[x - 1, y])
                        {
                            NotEnclosed();
                        }

                        newQueue.Enqueue((x - 1, y));
                    }

                    //East
                    if (!currentIsNotEnclosed && x < matrix.ColumnCount - 1 && !isEnclosed[x + 1, y])
                    {
                        NotEnclosed();
                    }

                    if (x < matrix.ColumnCount - 1 && !visited.Contains((x + 1, y))
                        && (currentChar == '.' && matrix[x + 1, y] == '.'
                        || IsAnyOf(currentChar, '-', 'L', 'F') && IsAnyOf(matrix[x + 1, y], '-', 'J', '7')))
                    {
                        if (!currentIsNotEnclosed && !isEnclosed[x + 1, y])
                        {
                            NotEnclosed();
                        }

                        newQueue.Enqueue((x + 1, y));
                    }

                    //North
                    if (!currentIsNotEnclosed && y > 0 && !isEnclosed[x, y - 1])
                    {
                        NotEnclosed();
                    }

                    if (y > 0 && !visited.Contains((x, y - 1))
                        && (currentChar == '.' && matrix[x, y - 1] == '.'
                        || IsAnyOf(currentChar, '|', 'L', 'J') && IsAnyOf(matrix[x, y - 1], '|', 'F', '7')))
                    {
                        if (!currentIsNotEnclosed && !isEnclosed[x, y - 1])
                        {
                            NotEnclosed();
                        }

                        newQueue.Enqueue((x, y - 1));
                    }

                    //South
                    if (!currentIsNotEnclosed && y < matrix.RowCount - 1 && !isEnclosed[x, y + 1])
                    {
                        NotEnclosed();
                    }

                    if (y < matrix.RowCount - 1 && !visited.Contains((x, y + 1))
                        && (currentChar == '.' && matrix[x, y + 1] == '.'
                        || IsAnyOf(currentChar, '|', 'F', '7') && IsAnyOf(matrix[x, y + 1], '|', 'L', 'J')))
                    {
                        if (!currentIsNotEnclosed && !isEnclosed[x, y + 1])
                        {
                            NotEnclosed();
                        }

                        newQueue.Enqueue((x, y + 1));
                    }

                    if (x > 0 && !currentIsNotEnclosed)
                    {
                        var trying = (x - 1, y);
                        while (trying.Item1 >= 0 && y < matrix.RowCount - 1)
                        {
                            var top = matrix[trying.Item1, y];
                            var bottom = matrix[trying.Item1, y + 1];
                            AddReached(trying.Item1, y, x, y);
                            AddReached(trying.Item1, y + 1, x, y);
                            if (!isEnclosed[trying.Item1, y] || !isEnclosed[trying.Item1, y + 1])
                            {
                                NotEnclosed();
                                break;
                            }

                            if (trying.Item1 == 0)
                            {
                                break;
                            }

                            if (CanMoveBetweenHorizontal(top, bottom))
                            {
                                trying.Item1 = trying.Item1 - 1;
                            }
                            else
                            {
                                break;
                            }
                        }

                        trying = (x - 1, y);
                        while (trying.Item1 >= 0 && y > 0)
                        {
                            var top = matrix[trying.Item1, y - 1];
                            var bottom = matrix[trying.Item1, y];
                            AddReached(trying.Item1, y - 1, x, y);
                            AddReached(trying.Item1, y, x, y);
                            if (!isEnclosed[trying.Item1, y - 1] || !isEnclosed[trying.Item1, y])
                            {
                                NotEnclosed();
                                break;
                            }

                            if (trying.Item1 == 0)
                            {
                                break;
                            }

                            if (CanMoveBetweenHorizontal(top, bottom))
                            {
                                trying.Item1 = trying.Item1 - 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (x < matrix.ColumnCount - 1 && !currentIsNotEnclosed)
                    {
                        var trying = (x + 1, y);
                        while (trying.Item1 < matrix.ColumnCount && y < matrix.RowCount - 1)
                        {
                            var top = matrix[trying.Item1, y];
                            var bottom = matrix[trying.Item1, y + 1];
                            AddReached(trying.Item1, y, x, y);
                            AddReached(trying.Item1, y + 1, x, y);
                            if (!isEnclosed[trying.Item1, y] || !isEnclosed[trying.Item1, y + 1])
                            {
                                NotEnclosed();
                                break;
                            }

                            if (trying.Item1 == 0)
                            {
                                break;
                            }

                            if (CanMoveBetweenHorizontal(top, bottom))
                            {
                                trying.Item1 = trying.Item1 + 1;
                            }
                            else
                            {
                                break;
                            }
                        }

                        trying = (x + 1, y);
                        while (trying.Item1 < matrix.ColumnCount && x > 0)
                        {
                            var top = matrix[trying.Item1, y - 1];
                            var bottom = matrix[trying.Item1, y];
                            AddReached(trying.Item1, y - 1, x, y);
                            AddReached(trying.Item1, y, x, y);
                            if (!isEnclosed[trying.Item1, y - 1] || !isEnclosed[trying.Item1, y])
                            {
                                NotEnclosed();
                                break;
                            }

                            if (trying.Item1 == 0)
                            {
                                break;
                            }

                            if (CanMoveBetweenHorizontal(top, bottom))
                            {
                                trying.Item1 = trying.Item1 + 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (y > 0 && !currentIsNotEnclosed)
                    {
                        var trying = (x, y - 1);
                        while (trying.Item2 >= 0 && x < matrix.ColumnCount - 1)
                        {
                            var left = matrix[x, trying.Item2];
                            var right = matrix[x + 1, trying.Item2];
                            AddReached(x, trying.Item2, x, y);
                            AddReached(x + 1, trying.Item2, x, y);
                            if (!isEnclosed[x, trying.Item2] || !isEnclosed[x + 1, trying.Item2])
                            {
                                NotEnclosed();
                                break;
                            }

                            if (trying.Item2 == matrix.RowCount - 1)
                            {
                                break;
                            }

                            if (CanMoveBetweenVertical(left, right))
                            {
                                trying.Item2 = trying.Item2 - 1;
                            }
                            else
                            {
                                break;
                            }
                        }

                        trying = (x, y - 1);
                        while (trying.Item2 >= 0 && x > 0)
                        {
                            var left = matrix[x - 1, trying.Item2];
                            var right = matrix[x, trying.Item2];
                            AddReached(x - 1, trying.Item2, x, y);
                            AddReached(x, trying.Item2, x, y);
                            if (!isEnclosed[x - 1, trying.Item2] || !isEnclosed[x, trying.Item2])
                            {
                                NotEnclosed();
                                break;
                            }

                            if (trying.Item2 == matrix.RowCount - 1)
                            {
                                break;
                            }

                            if (CanMoveBetweenVertical(left, right))
                            {
                                trying.Item2 = trying.Item2 - 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (y < matrix.RowCount - 1 && !currentIsNotEnclosed)
                    {
                        var trying = (x, y + 1);
                        while (trying.Item2 < matrix.RowCount && x < matrix.ColumnCount - 1)
                        {
                            var left = matrix[x, trying.Item2];
                            var right = matrix[x + 1, trying.Item2];
                            AddReached(x, trying.Item2, x, y);
                            AddReached(x + 1, trying.Item2, x, y);
                            if (!isEnclosed[x, trying.Item2] || !isEnclosed[x + 1, trying.Item2])
                            {
                                NotEnclosed();
                                break;
                            }

                            if (trying.Item2 == matrix.RowCount - 1)
                            {
                                break;
                            }

                            if (CanMoveBetweenVertical(left, right))
                            {
                                trying.Item2 = trying.Item2 + 1;
                            }
                            else
                            {
                                break;
                            }
                        }

                        trying = (x, y + 1);
                        while (trying.Item2 < matrix.RowCount && x > 0)
                        {
                            var left = matrix[x - 1, trying.Item2];
                            var right = matrix[x, trying.Item2];
                            AddReached(x - 1, trying.Item2, x, y);
                            AddReached(x, trying.Item2, x, y);
                            if (!isEnclosed[x - 1, trying.Item2] || !isEnclosed[x, trying.Item2])
                            {
                                NotEnclosed();
                                break;
                            }

                            if (trying.Item2 == matrix.RowCount - 1)
                            {
                                break;
                            }

                            if (CanMoveBetweenVertical(left, right))
                            {
                                trying.Item2 = trying.Item2 + 1;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    void NotEnclosed()
                    {
                        foreach (var f in visited)
                        {
                            isEnclosed[f.x, f.y] = false;
                        }

                        if (reachedNodes.TryGetValue((x, y), out var reached))
                        {
                            foreach (var ff in reached)
                            {
                                isEnclosed[ff.fromX, ff.fromY] = false;
                            }
                        }

                        visited.Clear();
                        currentIsNotEnclosed = true;
                    }
                }
                
                void AddReached(int XX, int YY, int fromX, int fromY)
                {
                    var key = (XX, YY);
                    var value = (fromX, fromY);
                    if (!reachedNodes.ContainsKey(key))
                    {
                        reachedNodes.Add(key, new List<(int fromX, int fromY)>() { value });
                    } else
                    {
                        reachedNodes[key].Add(value);
                    }
                }

                bool CanMoveBetweenVertical(char left, char right) => IsAnyOf(left, '|', '7', 'J') && IsAnyOf(right, '|', 'F', 'L');
                bool CanMoveBetweenHorizontal(char top, char bottom) => IsAnyOf(top, '-', 'J', 'L') && IsAnyOf(bottom, '-', 'F', '7');
            }

            this.Print(isEnclosed.ToString(x => x ? "1" : "0", ""));
            var counter = 0;
            for (var x = 0; x < matrix.ColumnCount; x++)
            {
                for (var y = 0; y < matrix.RowCount; y++)
                {
                    if (isEnclosed[x, y] && mainLoop[x, y] == '.')
                    {
                        counter++;
                        mainLoop[x, y] = 'I';
                    }
                }
            }

            this.Print(mainLoop.ToString(x => x.ToString(), ""));
            this.PrintResult(counter);
        }

        private static bool IsAnyOf(char c, params char[] chars)
        {
            return chars.Any(x => x == c);
        }
    }
}
