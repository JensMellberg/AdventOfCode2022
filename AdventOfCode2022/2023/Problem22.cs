using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem22 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public override void Solve(IEnumerable<string> testData)
        {
            var id = 0;
            var bricks = testData.Select(x => x.Split('~')).Select(x => new Brick(Point3D.FromString(x[0]), Point3D.FromString(x[1]), ++id)).ToList();
            var coordinates = new int[500, 500, 500];
            foreach (var brick in bricks.OrderBy(x => Math.Min(x.Start.Z, x.End.Z)).ToList())
            {
                var min = Math.Min(brick.Start.Z, brick.End.Z);
                var contained = brick.GetContainedPoints();
                var offset = 1;
                var underCoords = contained.Select(x => coordinates[x.X, x.Y, x.Z - offset]).ToList();
                while (min - offset > 0 && underCoords.All(x => x == 0))
                {
                    offset++;
                    underCoords = contained.Select(x => coordinates[x.X, x.Y, x.Z - offset]).ToList();
                }

                if (min - offset != 0)
                {
                    var distinct = underCoords.Where(x => x != 0).Distinct();
                    brick.SupportedBy.AddRange(distinct);
                    foreach (var s in distinct)
                    {
                        bricks[s - 1].Supports.Add(brick.Id);
                    }

                }

                contained.ForEach(x => coordinates[x.X, x.Y, x.Z - offset + 1] = brick.Id);
            }

            var total = 0;
            foreach (var b in bricks)
            {
                if (b.Supports.All(x => bricks[x - 1].SupportedBy.Count > 1))
                {
                    total++;
                }
            }

            this.PrintResult(total);

            var max = 0;
            foreach (var f in bricks)
            {
                var fallen = new HashSet<int>();
                bricks.ForEach(x => x.HasFallen = false);
                max += f.FallCount(bricks, fallen) - 1;
            }

            this.PrintResult(max);
        }

        private class Brick
        {
            public Point3D Start { get; set; }

            public Point3D End { get; set; }

            public int Id { get; set; }

            public Brick(Point3D start, Point3D end, int id) 
            {
                this.Start = start;
                this.End = end;
                this.Id = id;
            }

            public List<int> Supports = new List<int>();

            public List<int> SupportedBy = new List<int>();

            public bool HasFallen = false;

            public int FallCount(List<Brick> bricks, HashSet<int> fallen)
            {
                if (this.HasFallen)
                {
                    return 0;
                }

                var total = 0;
                foreach (var s in this.Supports)
                {
                    var brick = bricks[s - 1];
                    if (brick.SupportedBy.Where(x => !fallen.Contains(x) || x == this.Id).Count() == 1) {
                        fallen.Add(brick.Id);
                        total += brick.FallCount(bricks, fallen);
                    }
                }

                this.HasFallen = true;
                return total + 1;
            }

            public List<Point3D> GetContainedPoints()
            {
                var result = new List<Point3D>();
                var start = Start.Copy();
                while (!start.Equals(this.End))
                {
                    result.Add(start);
                    var copy = start.Copy();
                    copy.X += this.End.X.CompareTo(this.Start.X);
                    copy.Y += this.End.Y.CompareTo(this.Start.Y);
                    copy.Z += this.End.Z.CompareTo(this.Start.Z);
                    start = copy;
                }

                result.Add(this.End.Copy());
                return result;
            }
        }
    }
}
