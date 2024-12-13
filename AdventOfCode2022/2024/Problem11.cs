using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem11 : ObjectProblem<StoneList>
    {
        public static Dictionary<(long startValue, int blinksLeft), long> Results = new Dictionary<(long startValue, int blinksLeft), long>(); 
        public override void Solve(IEnumerable<StoneList> testData)
        {
            var stones = testData.Single();
            var stonesCopy = new StoneList
            {
                Values = stones.Stones.Select(x => new Stone { Value = x.Value }).ToList()
            };

            var first = stones.Blink(25);
            this.PrintResult(first);
            var second = stonesCopy.Blink(75);
            this.PrintResult(second);
        }
    }

    public class StoneList : ListParsable<Stone>
    {
        protected override string Separator => " ";
        public List<Stone> Stones => this.Values;

        public long Blink(int count) => this.Stones.Sum(x => x.Blink(count));

        protected override object ConvertValue(string value)
        {
            return new Stone { Value = long.Parse(value) };
        }
    }

    public class Stone
    {
        public long Value { get; set; }

        public long Blink(int count)
        {
            if (count == 0)
            {
                return 1;
            }

            var startKey = (this.Value, count);
            if (Problem11.Results.TryGetValue(startKey, out var result))
            {
                return result;
            }

            var valueString = this.Value.ToString();
            if (this.Value == 0)
            {
                this.Value = 1;
                result = this.Blink(count - 1);
            }

            else if (valueString.Length % 2 == 0)
            {
                var child1 = new Stone { Value = long.Parse(valueString.Substring(0, valueString.Length / 2)) };
                var child2 = new Stone { Value = long.Parse(valueString.Substring(valueString.Length / 2, valueString.Length / 2)) };
                result = new[] { child1, child2 }.Sum(x => x.Blink(count - 1));
            }
            else
            {
                this.Value *= 2024;
                result = this.Blink(count - 1);
            }

            Problem11.Results.Add(startKey, result);
            return result;
        }
    }
}
