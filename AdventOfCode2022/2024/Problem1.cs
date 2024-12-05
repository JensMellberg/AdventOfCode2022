using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem1 : PatternProblem<(int first, int second)>
    {
        protected override string Pattern => "¤first¤   ¤second¤";

        public override void Solve(IEnumerable<(int first, int second)> testData)
        {
            var list1 = new List<int>();
            var list2 = new List<int>();
            var list2AppearanceCounts = new int[100000];
            foreach (var l in testData)
            {
                list1.Add(l.first);
                list2.Add(l.second);
                list2AppearanceCounts[l.second]++;
            }

            list1.Sort();
            list2.Sort();
            var totalDist = 0;
            var similarityScore = 0;
            for (var i = 0; i < list1.Count; i++)
            {
                totalDist += Math.Abs(list1[i] - list2[i]);
                similarityScore += list1[i] * list2AppearanceCounts[list1[i]];
            }

            this.PrintResult(totalDist);
            this.PrintResult(similarityScore);
        }
    }
}
