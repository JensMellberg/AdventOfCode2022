using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem19 : SplitProblem<Towels, Towel>
    {
        private Dictionary<char, List<string>> towelsByFirstLetter;
        private Dictionary<string, long> PossibleTowels = new Dictionary<string, long>();

        protected override void Solve(IEnumerable<Towels> availableTowels, IEnumerable<Towel> towels)
        {
            this.towelsByFirstLetter = new Dictionary<char, List<string>>();
            foreach (var t in availableTowels.Single().Values)
            {
                if (!towelsByFirstLetter.TryGetValue(t[0], out var list))
                {
                    list = new List<string>();
                    towelsByFirstLetter[t[0]] = list;
                }

                list.Add(t);
            }

            this.PrintResult(towels.Where(x => this.WaysToMakeTowel(x.Pattern) > 0).Count());
            this.PrintResult(towels.Sum(x => this.WaysToMakeTowel(x.Pattern)));
        }

        private long WaysToMakeTowel(string towel)
        {
            if (PossibleTowels.ContainsKey(towel))
            {
                return PossibleTowels[towel];
            }

            long ways = 0;
            foreach (var t in towelsByFirstLetter[towel[0]])
            {
                if (towel == t)
                {
                    ways++;
                }
                else if (towel.StartsWith(t))
                {
                    ways += this.WaysToMakeTowel(towel.Substring(t.Length));
                }
            }

            PossibleTowels.Add(towel, ways);
            return ways;
        }
    }

    public class Towels : ListParsable<string>
    {
        protected override string Separator => ", ";
    }

    public class Towel : Parsable
    {
        public string Pattern { get; set; }

        public override void ParseFromLine(string line)
        {
            this.Pattern = line;
            base.ParseFromLine(line);
        }
    }
}
