using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem9 : ObjectProblem<SensorHistory>
    {
        public override void Solve(IEnumerable<SensorHistory> testData)
        {
            this.PrintResult(testData.Sum(x => x.LastValue()));
            this.PrintResult(testData.Sum(x => x.FirstValue()));
        }
     }

    public class SensorHistory : Parsable
    {
        public long[] Sequence { get; set; }

        public bool IsAllZeros { get; set; }
        public override void ParseFromLine(string line)
        {
            this.Sequence = line.Split(' ').Select(long.Parse).ToArray();
        }

        public SensorHistory()
        {

        }

        public SensorHistory(long[] sequence)
        {
            Sequence = sequence;
        }

        public long Sum() => this.Sequence.Sum() + this.LastValue();

        public long LastValue()
        {
            if (this.IsAllZeros)
            {
                return 0;
            }

            return this.ProduceDiff().LastValue() + this.Sequence.Last();
        }

        public long FirstValue()
        {
            if (this.IsAllZeros)
            {
                return 0;
            }

            return this.Sequence.First() - this.ProduceDiff().FirstValue();
        }

        public SensorHistory ProduceDiff()
        {
            var values = new List<long>();
            var hasNonZero = false;
            for (var i = 1; i < this.Sequence.Length; i++)
            {
                var newValue = this.Sequence[i] - this.Sequence[i - 1];
                if (newValue != 0)
                {
                    hasNonZero = true;
                }

                values.Add(newValue);
            }

            var returner = new SensorHistory(values.ToArray());
            returner.IsAllZeros = !hasNonZero;
            return returner;
        }
    }
}
