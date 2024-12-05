using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Net.Http.Headers;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem5 : SplitProblem<OrderRule, ProducePages>
    {
        protected override string FirstPattern => "¤First¤|¤Second¤";
        protected override void Solve(IEnumerable<OrderRule> rules, IEnumerable<ProducePages> pages)
        {
            this.PrintResult(pages.Sum(x => rules.All(rule => x.SatisfiesRule(rule)) ? x.MiddlePage : 0));
        }
    }

    public class OrderRule
    {
        public int First { get; set; }

        public int Second { get; set; }
    }

    public class ProducePages : ListParsable<int>
    {
        Dictionary<int, int> PageIndexes { get; set; }

        public bool SatisfiesRule(OrderRule rule)
        {
            if (!this.PageIndexes.ContainsKey(rule.First) || !this.PageIndexes.ContainsKey(rule.Second))
            {
                return true;
            }

            return this.PageIndexes[rule.First] < this.PageIndexes[rule.Second];
        }

        public int MiddlePage { get; private set; }

        public override void ParseFromLine(string line)
        {
            base.ParseFromLine(line);
            var pages = this.Values;
            this.PageIndexes = new Dictionary<int, int>();
            var middleIndex = pages.Count / 2;
            for (var i = 0; i < pages.Count; i++)
            {
                this.PageIndexes.Add(pages[i], i);
                if (i == middleIndex)
                {
                    this.MiddlePage = pages[i];
                }
            }
        }
    }
}
