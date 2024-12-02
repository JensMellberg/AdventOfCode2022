using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem2 : ObjectProblem<ReportData>
    {
        public override void Solve(IEnumerable<ReportData> testData)
        {
            var safeCount = testData.Count(x => x.IsSafe());
            var safeCountWithRemovals = testData.Count(x => x.Clones().Any(c => c.IsSafe()));
            this.PrintResult(safeCount);
            this.PrintResult(safeCountWithRemovals);
        }    
    }

    public class ReportData : Parsable
    {
        public int[] values;
        public bool IsSafe()
        {
            const int minDiff = 1;
            const int maxDiff = 3;
            bool? previousDirection = null;
            int? previousValue = null;

            for (var i = 0; i < this.values.Length; i++)
            {
                var val = this.values[i];
                if (previousValue.HasValue)
                {
                    var newDirection = val > previousValue.Value;
                    if (newDirection != (previousDirection ?? newDirection))
                    {
                        return false;
                    }

                    var diff = Math.Abs(val - previousValue.Value);
                    if (diff < minDiff || diff > maxDiff)
                    {
                        return false;
                    }

                    previousValue = val;
                    previousDirection = newDirection;
                }
                else
                {
                    previousValue = val;
                }
            }

            return true;
        }

        public IEnumerable<ReportData> Clones()
        {
            for (var i = 0; i < this.values.Length; i++)
            {
                var clonedArray = this.values.Take(i).Concat(this.values.Skip(i + 1)).ToArray();
                yield return new ReportData { values = clonedArray };
            }
        }

        public override void ParseFromLine(string line)
        {
            this.values = line.Split(' ').Select(int.Parse).ToArray();
            base.ParseFromLine(line);
        }
    }
}
