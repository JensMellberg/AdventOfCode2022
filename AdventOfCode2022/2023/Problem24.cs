using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem24 : ObjectProblem<HailStone>
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        private const long MinIntersect = 200000000000000;

        private const long MaxIntersect = 400000000000000;

        public override void Solve(IEnumerable<HailStone> testData)
        {
            var list = testData.ToList();
            var total = 0;
            for (var f = 0; f < list.Count; f++)
            {
                for (var f2 = f + 1; f2 < list.Count; f2++)
                {
                    var intersection = list[f].Intersection2D(list[f2], true);
                    if (intersection.HasValue)
                    {
                        var (x, y) = intersection.Value;
                        if (x >= MinIntersect && x <= MaxIntersect
                            && y >= MinIntersect && y <= MaxIntersect)
                        {
                            total++;
                        }
                    }
                    
                }
            }

            this.PrintResult(total);
        }
    }

    public class HailStone : Parsable
    {
        public Point3D StartPoint { get; set; }

        public Point3D VelocityVector { get; set; }

        public override void ParseFromLine(string line)
        {
            var tokens = line.Split('@');
            this.StartPoint = Point3D.FromString(tokens[0].Replace(" ", ""));
            this.VelocityVector = Point3D.FromString(tokens[1].Replace(" ", ""));
            base.ParseFromLine(line);
        }

        public (long x, long y)? Intersection2D(HailStone other, bool recursive)
        {
            var a = ((double)other.StartPoint.X - this.StartPoint.X) / this.VelocityVector.X;
            var b = ((double)other.StartPoint.Y - this.StartPoint.Y) / this.VelocityVector.Y;

            var c = (double)other.VelocityVector.Y / this.VelocityVector.Y - (double)other.VelocityVector.X / this.VelocityVector.X;

            var time = (a - b) / c;

            // Intersected in the past
            if (time < 0 || recursive && !other.Intersection2D(this, false).HasValue)
            {
                return null;
            }

            var x = other.StartPoint.X + time * other.VelocityVector.X;
            var y = other.StartPoint.Y + time * other.VelocityVector.Y;
            
            return ((long)Math.Floor(x), (long)Math.Ceiling(y));
        }
    }
}
