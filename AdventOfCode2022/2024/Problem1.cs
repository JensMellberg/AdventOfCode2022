using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem1 : StringProblem
    {
        public override void Solve(IEnumerable<string> testData)
        {
            var list1 = new List<int>();
            var list2 = new List<int>();
            var list2AppearanceCounts = new int[100000];
            foreach (var l in testData)
            {
                var tokens = l.Split(' ');
                var list2Entry = int.Parse(tokens[tokens.Length - 1]);
                list1.Add(int.Parse(tokens[0]));
                list2.Add(list2Entry);
                list2AppearanceCounts[list2Entry]++;
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
