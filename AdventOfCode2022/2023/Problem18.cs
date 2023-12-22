using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem18 : ObjectProblem<DigInstruction>
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public override void Solve(IEnumerable<DigInstruction> testData)
        {
            var lines = new List<DigLine>();
            var point = new Point(0, 0);
            foreach (var instr in testData)
            {
                var oldPoint = point.Copy();
                var delta = instr.Direction.GetDelta();
                point.X += delta.x * instr.Steps;
                point.Y += delta.y * instr.Steps;
               /* Point start;
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
                }*/

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
            Console.ReadLine();
            while (yLines.Any())
            {
                var min = yLines.FindMin(x => x.StartPoint.Y);
                var width = min.Distance();
                var prevDist = min.PreviousLine.Distance();
                var nextDist = min.NextLine.Distance();
                long height = 0;
                //yLines.Remove(min);

                if (prevDist < nextDist)
                {
                    min.NextLine.StartPoint.Y += (int)prevDist;
                    height = prevDist;
                    min.StartPoint.Y += (int)height;
                    min.EndPoint.Y += (int)height;
                    //var newWidth = min.PreviousLine.EndPoint.XDistance(min.NextLine.StartPoint, true);
                    min.StartPoint.X = min.PreviousLine.EndPoint.X;
                    min.EndPoint.X = min.NextLine.StartPoint.X;
                    totalArea += width * height;
                }
                else if (nextDist < prevDist)
                {
                    min.PreviousLine.EndPoint.Y += (int)nextDist;
                    height = nextDist;
                    min.StartPoint.Y += (int)height;
                    min.EndPoint.Y += (int)height;
                    //var newWidth = min.PreviousLine.EndPoint.XDistance(min.NextLine.StartPoint, true);
                    min.StartPoint.X = min.PreviousLine.EndPoint.X;
                    min.EndPoint.X = min.NextLine.StartPoint.X;
                    totalArea += width * height;
                }
                else
                {
                    if (yLines.Count == 2)
                    {
                        totalArea += width * min.NextLine.Distance();
                        break;
                    }

                    min.StartPoint.Y += (int)prevDist;
                    min.EndPoint.Y += (int)prevDist;
                    min.PreviousLine.PreviousLine.NextLine = min;
                    min.NextLine.NextLine.PreviousLine = min;

                    //
                    var temp = min.NextLine.NextLine;
                    min.PreviousLine.PreviousLine.EndPoint = min.NextLine.NextLine.EndPoint;
                    min.PreviousLine.PreviousLine.NextLine = min.NextLine.NextLine.NextLine;
                    min.NextLine.NextLine.NextLine.PreviousLine = min.PreviousLine.PreviousLine;
                    yLines.Remove(min);
                    yLines.Remove(temp);
                }

                Printer();
                Console.ReadLine();
            }

            void Printer()
            {
                var matrix = Matrix.InitWithStartValue(10, 7, '.');
                var (xx, yy) = (0, 0);
                matrix[0, 0] = '#';
                var firstLine = lines[0];
                var currentLine = firstLine;
                var isFirstLoop = true;
                while (currentLine != firstLine || isFirstLoop)
                {
                    var delta = currentLine.Direction.GetDelta();
                    for (var f = 0; f < currentLine.Distance(); f++)
                    {
                        xx += delta.x;
                        yy += delta.y;
                        matrix[xx, yy] = '#';
                    }

                    isFirstLoop = false;
                    currentLine = currentLine.NextLine;
                }

                this.Print(matrix.ToString(x => x.ToString(), ""));
            }
        }

        private class DigLine
        {
            public Point StartPoint { get; set; }

            public Point EndPoint { get; set; }

            public Direction Direction { get; set; }

            public DigLine PreviousLine { get; set; }

            public DigLine NextLine { get; set; }

            public long Distance() => this.StartPoint.ManhattanDistance(this.EndPoint);

            public Point XIntersection(long y)
            {
                if (StartPoint.Y > y || EndPoint.Y < y)
                {
                    return null;
                }

                return null;
            }
        }
    }

    public class DigInstruction : Parsable
    {
        public Direction Direction { get; set; }

        public int Steps { get; set; }

        public string HexColor { get; set; }

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
            this.HexColor = parser.Pop().Substring(1, 7);
            base.ParseFromLine(line);
        }
    }
}
