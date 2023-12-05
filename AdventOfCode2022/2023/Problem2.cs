using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem2 : ObjectProblem<CubeGame>
    {
        public IDictionary<string, int> AllowAmountPerColor = new Dictionary<string, int> { { "red", 12 }, { "green", 13 }, { "blue", 14} };
        public override void Solve(IEnumerable<CubeGame> testData)
        {
            var testDataList = testData.ToList();
            this.PrintResult(testDataList.Select(x => x.IsAllowed(this.AllowAmountPerColor) ? x.GameId : 0).Sum());
            this.PrintResult(testDataList.Select(x => x.GetPowerSet()).Sum());
        }
    }

    public class CubeGame : Parsable
    {
        public int GameId { get; set; }

        public IList<CubeGameDraft> Drafts { get; set; } = new List<CubeGameDraft>();

        public override void ParseFromLine(string line)
        {
            var parser = new TokenParser(line);
            parser.Skip();
            this.GameId = int.Parse(parser.PopUntil(':').Last());

            while (!parser.IsFinished)
            {
                var draft = parser.ReadUntil(';');
                this.Drafts.Add(new CubeGameDraft(draft));
            }
        }

        public bool IsAllowed(IDictionary<string, int> allowAmountPerColor)
        {
            foreach (var draft in this.Drafts)
            {
                foreach (var color in allowAmountPerColor.Keys)
                {
                    if (draft.GetAmount(color) > allowAmountPerColor[color])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int GetPowerSet()
        {
            var results = new Dictionary<string, int> { { "red", 0 }, { "green", 0 }, { "blue", 0 } };
            foreach (var draft in this.Drafts)
            {
                foreach (var color in results.Keys.ToList())
                {
                    results[color] = Math.Max(results[color], draft.GetAmount(color));
                }
            }

            return results.Values.Aggregate((a, b) => a * b);
        }
    }

    public class CubeGameDraft
    {
        public IDictionary<string, int> AmountPerColor { get; set; } = new Dictionary<string, int>();
        public CubeGameDraft(string line)
        {
            var parser = new TokenParser(line);
            while (!parser.IsFinished)
            {
                var entry = parser.PopUntil(',');
                this.AmountPerColor.Add(entry[1], int.Parse(entry[0]));
            }
        }

        public int GetAmount(string color)
        {
            if (this.AmountPerColor.TryGetValue(color, out var amount))
            {
                return amount;
            }

            return 0;
        }
    }
}
