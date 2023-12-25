using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem18 : ObjectProblem<DigInstruction>
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public override void Solve(IEnumerable<DigInstruction> testData)
        {
            this.SolveProblem(testData);
            this.SolveProblem(testData.Select(x => x.ToHexVersion()));
        }

        private void SolveProblem(IEnumerable<DigInstruction> data)
        {
            var lines = new List<DigLine>();
            var point = new Point(0, 0);
            foreach (var instr in data)
            {
                var oldPoint = point.Copy();
                var delta = instr.Direction.GetDelta();
                point.X += delta.x * instr.Steps;
                point.Y += delta.y * instr.Steps;
                 Point start;
                 Point end;
                 if (instr.Direction.IsHorizontal())
                 {
                     if (oldPoint.X < point.X)
                     {
                         start = oldPoint;
                         end = point;
                     }
                     else
                     {
                         start = point;
                         end = oldPoint;
                     }
                 }
                 else
                 {
                     if (oldPoint.Y < point.Y)
                     {
                         start = oldPoint;
                         end = point;
                     }
                     else
                     {
                         start = point;
                         end = oldPoint;
                     }
                 }

                var line = new DigLine { StartPoint = oldPoint.Copy(), EndPoint = point.Copy(), Direction = instr.Direction };

                if (lines.Any())
                {
                    var previousLine = lines.Last();
                    previousLine.NextLine = line;
                    line.PreviousLine = previousLine;
                }

                lines.Add(line);
            }

            lines.Last().NextLine = lines.First();
            lines.First().PreviousLine = lines.Last();
            // var xSortedLines = lines.OrderBy(x => x.StartPoint.X);
            var yLines = lines.Where(x => x.Direction.IsHorizontal()).ToList();
            long totalArea = 0;
            Printer();

            while (yLines.Any())
            {
                var sorted = yLines.OrderBy(x => x.StartPoint.Y).ToList();
                //var min = yLines.FindMin(x => x.StartPoint.Y);
                var min = sorted[0];

                var width = min.Distance();
                var prevDist = min.PreviousLine.Distance();
                var nextDist = min.NextLine.Distance();
                long height = Math.Min(prevDist, nextDist);
                var inbetweenLines = new List<DigLine>();

                foreach (var line in sorted.Skip(1))
                {
                    if (line.StartPoint.Y > min.StartPoint.Y + height)
                    {
                        break;
                    }

                    if (!(line == min.PreviousLine.PreviousLine || line == min.NextLine.NextLine) && (line.StartPoint.IsBetweenX(min.StartPoint, min.EndPoint) || line.StartPoint.IsBetweenX(min.StartPoint, min.EndPoint)))
                    {
                        inbetweenLines.Add(line);
                        height = Math.Min(line.StartPoint.Y - min.StartPoint.Y, height);
                    }
                }

                min.StartPoint.Y += (int)height;
                min.EndPoint.Y += (int)height;
                
                //yLines.Remove(min);

                if (inbetweenLines.Count == 0)
                {
                    if (prevDist < nextDist)
                    {
                        DigLine target;
                        if (min.PreviousLine.PreviousLine.Direction == min.Direction.Reverse())
                        {
                            target = min.PreviousLine.PreviousLine.PreviousLine;
                            totalArea += min.PreviousLine.PreviousLine.Distance();
                            this.Print(min.PreviousLine.PreviousLine.Distance().ToString());
                        }
                        else
                        {
                            target = min.PreviousLine.PreviousLine;
                        }

                        min.ConnectStart(target);
                        min.NextLine.StartPoint.Y += (int)height;
                    }
                    else if (nextDist < prevDist)
                    {
                        DigLine target;
                        if (min.NextLine.NextLine.Direction == min.Direction.Reverse())
                        {
                            target = min.NextLine.NextLine.NextLine;
                            totalArea += min.NextLine.NextLine.Distance();
                            this.Print(min.NextLine.NextLine.Distance().ToString());
                        }
                        else
                        {
                            target = min.NextLine.NextLine;
                        }

                        min.ConnectEnd(target);
                        min.PreviousLine.EndPoint.Y += (int)nextDist;
                    }
                    else
                    {
                        if (yLines.Count == 2)
                        {
                            totalArea += (width + 1) * (min.NextLine.Distance() + 1);
                            this.PrintResult(totalArea);
                            return;
                            break;
                        }

                        if (min.PreviousLine.PreviousLine.Direction == min.Direction.Reverse())
                        {
                            totalArea += min.PreviousLine.PreviousLine.Distance();
                            this.Print(min.PreviousLine.PreviousLine.Distance().ToString());
                        }

                        if (min.NextLine.NextLine.Direction == min.Direction.Reverse())
                        {
                            totalArea += min.NextLine.NextLine.Distance();
                            this.Print(min.NextLine.NextLine.Distance().ToString());
                        }

                        min.ConnectStart(min.PreviousLine.PreviousLine.PreviousLine);
                        min.ConnectEnd(min.NextLine.NextLine.NextLine);


                        /*min.PreviousLine.PreviousLine.NextLine = min;
                        min.NextLine.NextLine.PreviousLine = min;

                        //
                        var temp = min.NextLine.NextLine;
                        min.PreviousLine.PreviousLine.EndPoint = min.NextLine.NextLine.EndPoint;
                        min.PreviousLine.PreviousLine.NextLine = min.NextLine.NextLine.NextLine;
                        min.NextLine.NextLine.NextLine.PreviousLine = min.PreviousLine.PreviousLine;

                        yLines.Remove(min);
                        min = temp.NextLine;
                        yLines.Remove(temp);*/
                    }
                }
                else
                {
                    var relevantLines = inbetweenLines.Where(x => x.StartPoint.Y == inbetweenLines[0].StartPoint.Y).OrderBy(x => x.StartPoint.X).ToList();
                    var firstY = relevantLines[0].StartPoint.Y;
                    if (firstY < prevDist && firstY < nextDist)
                    {
                        min.PreviousLine.EndPoint.Y += (int)height;
                        min.NextLine.StartPoint.Y += (int)height;
                    }

                    var end = min.NextLine;
                    var startLine = min.PreviousLine;
                    foreach (var l in relevantLines)
                    {
                        var newLine = new DigLine();
                        newLine.ConnectStart(startLine);
                        newLine.ConnectEnd(l.NextLine);
                        startLine = l;
                    }
                }

                totalArea += (width + 1) * (height);
                yLines = this.RemoveOrphans(min).Where(x => x.Direction.IsHorizontal()).ToList();
                Printer();
                this.Print(((width + 1) * (height)).ToString());
            }

            void Printer()
            {
                return;
                var matrix = Matrix.InitWithStartValue(10, 7, '.');
                
                var firstLine = yLines.FindMin(x => (long)x.StartPoint.Y);
                var (xx, yy) = (firstLine.StartPoint.X, firstLine.StartPoint.Y);
                matrix[xx, yy] = firstLine.Direction switch
                {
                    Direction.Left => '<',
                    Direction.Right => '>',
                    Direction.Up => '^',
                    Direction.Down => 'v',
                };

                var currentLine = firstLine;
                var isFirstLoop = true;
                while (currentLine != firstLine || isFirstLoop)
                {
                    var delta = currentLine.Direction.GetDelta();
                    for (var f = 0; f < currentLine.Distance(); f++)
                    {
                        xx += delta.x;
                        yy += delta.y;
                        if (matrix[xx, yy] == '.') {
                            matrix[xx, yy] = currentLine.Id.ToString()[0];
                        };
                    }

                    isFirstLoop = false;
                    currentLine = currentLine.NextLine;
                }

                this.Print(matrix.ToString(x => x.ToString(), ""));
                Console.ReadLine();
            }
        }

        private IEnumerable<DigLine> RemoveOrphans(DigLine start)
        {
            var startId = start.Id;
            var isFirst = true;
            var current = start;
            while (current.Id != startId || isFirst)
            {
                isFirst = false;
                yield return current;
                current = current.NextLine;
            }
        }

        private class DigLine
        {
            public Point StartPoint { get; set; }

            public Point EndPoint { get; set; }

            public Direction Direction { get; set; }

            public DigLine PreviousLine { get; set; }

            public DigLine NextLine { get; set; }

            public static int CurrentId = 0;

            public int Id { get; set; }

            public long Distance() => this.StartPoint.ManhattanDistance(this.EndPoint);

            public DigLine()
            {
                this.Id = CurrentId++;
            }

            public Point XIntersection(long y)
            {
                if (StartPoint.Y > y || EndPoint.Y < y)
                {
                    return null;
                }

                return null;
            }

            public void ConnectStart(DigLine other)
            {
                this.PreviousLine = other;
                this.StartPoint = other.EndPoint.Copy();
                other.NextLine = this;
            }

            public void ConnectEnd(DigLine other)
            {
                this.NextLine = other;
                other.PreviousLine = this;
                this.EndPoint = other.StartPoint.Copy();
            }
        }
    }

    public class DigInstruction : Parsable
    {
        public Direction Direction { get; set; }

        public int Steps { get; set; }

        public string HexColor { get; set; }

        public DigInstruction ToHexVersion()
        {
            var stepsHex = HexColor.Substring(0, 5);
            var dir = HexColor[5] switch
            {
                '0' => Direction.Right,
                '1' => Direction.Down,
                '2' => Direction.Left,
                '3' => Direction.Up,
                _ => throw this.Exception,
            };

            return new DigInstruction
            {
                Steps = Convert.ToInt32(stepsHex, 16),
                Direction = dir
            };
        }

        public override void ParseFromLine(string line)
        {
            var parser = new TokenParser(line);
            var dirString = parser.Pop();
            this.Direction = dirString switch
            {
                "R" => Direction.Right,
                "L" => Direction.Left,
                "U" => Direction.Up,
                "D" => Direction.Down,
                _ => throw this.Exception,
            };

            this.Steps = int.Parse(parser.Pop());
            this.HexColor = parser.Pop().Substring(2, 6);
            base.ParseFromLine(line);
        }
    }
}
