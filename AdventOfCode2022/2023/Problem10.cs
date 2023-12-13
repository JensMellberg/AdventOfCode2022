using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem10 : StringProblem
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
            if (matrix.RowCount > 112)
            {
                outsideQueue.Enqueue((70, matrix.RowCount - 1));
                outsideQueue.Enqueue((96, matrix.RowCount - 1));
                outsideQueue.Enqueue((112, matrix.RowCount - 1));
            }
           
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

            //this.Print(mainLoop.ToString(x => x.ToString(), ""));
            var isEnclosed = Matrix.FromTestInput<bool>(testData, _ => true);
            var result = 0;
            for (var again = 0; again < 2; again++)
            {
                var bigQueue = new Queue<(int x, int y)>();

                var visitedOuter = new HashSet<(int x, int y)>();
                var reachedNodes = new Dictionary<(int x, int y), List<(int fromX, int fromY)>>();
                
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

                var inbetweenQueue = new Queue<(int x, int y, int x2, int y2, string dir, int xOrigin, int yOrigin)>();

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
                            inbetweenQueue.Enqueue((x - 1, y, x - 1, y + 1, "left", x, y));
                            inbetweenQueue.Enqueue((x - 1, y - 1, x - 1, y, "left", x, y));
                        }

                        if (x < matrix.ColumnCount - 1 && !currentIsNotEnclosed)
                        {
                            inbetweenQueue.Enqueue((x + 1, y, x + 1, y + 1, "right", x, y));
                            inbetweenQueue.Enqueue((x + 1, y - 1, x + 1, y, "right", x, y));
                        }

                        if (y > 0 && !currentIsNotEnclosed)
                        {
                            inbetweenQueue.Enqueue((x - 1, y - 1, x, y - 1, "up", x, y));
                            inbetweenQueue.Enqueue((x, y - 1, x + 1, y - 1, "up", x, y));
                        }

                        if (y < matrix.RowCount - 1 && !currentIsNotEnclosed)
                        {
                            inbetweenQueue.Enqueue((x - 1, y + 1, x, y + 1, "down", x, y));
                            inbetweenQueue.Enqueue((x, y + 1, x + 1, y + 1, "down", x, y));
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
                }

                reachedNodes.Clear();
                var copy = new Queue<(int x, int y, int x2, int y2, string dir, int xOrigin, int yOrigin)>(inbetweenQueue);
                var visitedBetween = new HashSet<(int x, int y, int x2, int y2, string dir, int xOrigin, int yOrigin)>();
                var deadEnds = new HashSet<(int x, int y, int x2, int y2, string dir)>();
                for (var i = 0; i < 2; i++)
                {
                    while (inbetweenQueue.Any())
                    {
                        var (x, y, x2, y2, dir, xOrigin, yOrigin) = inbetweenQueue.Dequeue();
                        if (!matrix.IsInBounds(x, y) || !matrix.IsInBounds(x2, y2))
                        {
                            continue;
                        }

                        if (!isEnclosed[xOrigin, yOrigin])
                        {
                            continue;
                        }
                        if (visitedBetween.Contains((x, y, x2, y2, dir, xOrigin, yOrigin)) || deadEnds.Contains((x, y, x2, y2, dir)))
                        {
                            continue;
                        }

                        visitedBetween.Add((x, y, x2, y2, dir, xOrigin, yOrigin));
                        

                        var first = matrix[x, y];
                        var scnd = matrix[x2, y2];
                        var newFirst = (x, y);
                        var newScnd = (x2, y2);

                        if ((dir == "left" || dir == "right") && !CanMoveBetweenHorizontal(first, scnd))
                        {
                            deadEnds.Add((x, y, x2, y2, dir));
                            continue;
                        }
                        if ((dir == "up" || dir == "down") && !CanMoveBetweenVertical(first, scnd))
                        {
                            deadEnds.Add((x, y, x2, y2, dir));
                            continue;
                        }

                        if (dir == "up")
                        {
                            inbetweenQueue.Enqueue((newScnd.x2, newScnd.y2 - 1, newScnd.x2, newScnd.y2, "right", xOrigin, yOrigin));
                            inbetweenQueue.Enqueue((newFirst.x, newFirst.y - 1, newFirst.x, newFirst.y, "left", xOrigin, yOrigin));
                            newFirst.y -= 1;
                            newScnd.y2 -= 1;
                        }
                        else if (dir == "down")
                        {
                            inbetweenQueue.Enqueue((newScnd.x2, newScnd.y2, newScnd.x2, newScnd.y2 + 1, "right", xOrigin, yOrigin));
                            inbetweenQueue.Enqueue((newFirst.x, newFirst.y, newFirst.x, newFirst.y + 1, "left", xOrigin, yOrigin));
                            newFirst.y += 1;
                            newScnd.y2 += 1;
                        }
                        else if (dir == "left")
                        {
                            inbetweenQueue.Enqueue((newFirst.x - 1, newFirst.y, newFirst.x, newFirst.y, "up", xOrigin, yOrigin));
                            inbetweenQueue.Enqueue((newScnd.x2 - 1, newScnd.y2, newScnd.x2, newScnd.y2, "down", xOrigin, yOrigin));
                            newFirst.x -= 1;
                            newScnd.x2 -= 1;
                        }
                        else if (dir == "right")
                        {
                            inbetweenQueue.Enqueue((newFirst.x, newFirst.y, newFirst.x + 1, newFirst.y, "up", xOrigin, yOrigin));
                            inbetweenQueue.Enqueue((newScnd.x2, newScnd.y2, newScnd.x2 + 1, newScnd.y2, "down", xOrigin, yOrigin));
                            newFirst.x += 1;
                            newScnd.x2 += 1;
                        }

                        if (!matrix.IsInBounds(newFirst.x, newFirst.y) || !matrix.IsInBounds(newScnd.x2, newScnd.y2))
                        {
                            continue;
                        }

                        if (!isEnclosed[newFirst.x, newFirst.y] || !isEnclosed[newScnd.x2, newScnd.y2])
                        {
                            isEnclosed[xOrigin, yOrigin] = false;
                            if (reachedNodes.TryGetValue((xOrigin, yOrigin), out var reached))
                            {
                                foreach (var ff in reached)
                                {
                                    isEnclosed[ff.fromX, ff.fromY] = false;
                                }
                            }

                            continue;
                        }

                        if (matrix[newFirst.x, newFirst.y] == '.')
                        {
                            AddReached(newFirst.x, newFirst.y, xOrigin, yOrigin);
                        }

                        if (matrix[newScnd.x2, newScnd.y2] == '.')
                        {
                            AddReached(newScnd.x2, newScnd.y2, xOrigin, yOrigin);
                        }

                        inbetweenQueue.Enqueue((newFirst.x, newFirst.y, newScnd.x2, newScnd.y2, dir, xOrigin, yOrigin));
                    }

                    inbetweenQueue = copy;
                }

                //this.Print(isEnclosed.ToString(x => x ? "1" : "0", ""));
                var counter = 0;
                for (var x = 0; x < matrix.ColumnCount; x++)
                {
                    for (var y = 0; y < matrix.RowCount; y++)
                    {
                        if (isEnclosed[x, y] && mainLoop[x, y] == '.')
                        {
                            counter++;
                        }
                    }
                }

                
                if (counter == result)
                {
                    for (var x = 0; x < matrix.ColumnCount; x++)
                    {
                        for (var y = 0; y < matrix.RowCount; y++)
                        {
                            if (isEnclosed[x, y] && mainLoop[x, y] == '.')
                            {
                                mainLoop[x, y] = 'I';
                            }
                        }
                    }

                    //this.Print(mainLoop.ToString(x => x.ToString(), ""));
                    this.PrintResult(counter);
                    break;
                }

                again--;
                result = counter;

                bool CanMoveBetweenVertical(char left, char right) => !IsAnyOf(left, '.', 'O', 'S') && !IsAnyOf(left, '.', 'O', 'S') && !(IsAnyOf(left, '-', 'L', 'F') && IsAnyOf(right, '-', 'J', '7'));
                bool CanMoveBetweenHorizontal(char top, char bottom) => !IsAnyOf(top, '.', 'O', 'S') && !IsAnyOf(bottom, '.', 'O', 'S') && !(IsAnyOf(top, '|', 'F', '7') && IsAnyOf(bottom, '|', 'J', 'L'));

                void AddReached(int XX, int YY, int fromX, int fromY)
                {
                    var key = (XX, YY);
                    var value = (fromX, fromY);
                    if (!reachedNodes.ContainsKey(key))
                    {
                        reachedNodes.Add(key, new List<(int fromX, int fromY)>() { value });
                    }
                    else
                    {
                        reachedNodes[key].Add(value);
                    }
                }
            }
            
        }

        private static bool IsAnyOf(char c, params char[] chars)
        {
            return chars.Any(x => x == c);
        }
    }
}
