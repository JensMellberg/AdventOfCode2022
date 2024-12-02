using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode2022
{
	public class Point3D
    {
		public long X { get; set; }

		public long Y { get; set; }

		public long Z { get; set; }

		public Point3D(long x, long y, long z)
		{
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public Point3D Copy() => new Point3D(this.X, this.Y, this.Z);

		public long XDistance(Point3D other, bool absolute)
		{
			var distance = other.X - this.X;
			return absolute ? Math.Abs(distance) : distance;
		}

		public long YDistance(Point3D other, bool absolute)
		{
			var distance = other.Y - this.Y;
			return absolute ? Math.Abs(distance) : distance;
		}

        public long ZDistance(Point3D other, bool absolute)
        {
            var distance = other.Z - this.Z;
            return absolute ? Math.Abs(distance) : distance;
        }

        public long ManhattanDistance(Point3D other) => this.XDistance(other, true) + this.YDistance(other, true) + this.ZDistance(other, true);

        public override bool Equals(object obj)
        {
            if (obj is Point3D pObj)
			{
				return this.X == pObj.X && this.Y == pObj.Y && this.Z == pObj.Z;
            }

			return false;
        }

        public override string ToString() => X + "," + Y + "," + Z;

		public static Point3D FromString(string line)
		{
			var tokens = line.Split(',');
			return new Point3D(long.Parse(tokens[0]), long.Parse(tokens[1]), long.Parse(tokens[2]));
		}
	}
}
