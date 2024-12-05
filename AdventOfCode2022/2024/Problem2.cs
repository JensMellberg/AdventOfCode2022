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

    public class ReportData : ListParsable<int>
    {
        protected override string Separator => " ";
        public bool IsSafe()
        {
            const int minDiff = 1;
            const int maxDiff = 3;
            bool? previousDirection = null;
            int? previousValue = null;

            for (var i = 0; i < this.Values.Count; i++)
            {
                var val = this.Values[i];
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
            for (var i = 0; i < this.Values.Count; i++)
            {
                var clonedList = this.Values.Take(i).Concat(this.Values.Skip(i + 1)).ToList();
                yield return new ReportData { Values = clonedList };
            }
        }
    }
}
