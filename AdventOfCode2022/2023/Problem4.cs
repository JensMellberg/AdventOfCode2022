using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem4 : ObjectProblem<LotteryCard>
    {
        public override void Solve(IEnumerable<LotteryCard> testData)
        {
            var list = testData.ToList();
            var total = 0;
            this.PrintResult(list.Sum(x => x.Score()));
            for (var i = 0; i < list.Count; i++)
            {
                var matches = list[i].Matches;
                for (var x = i + 1; x <= i + matches; x++)
                {
                    list[x].Instances += list[i].Instances;
                }

                total += list[i].Instances;
            }

            this.PrintResult(total);
        }
     }

    public class LotteryCard : Parsable
    {
        int CardId { get; set; }

        IEnumerable<int> WinningNumbers;

        IEnumerable<int> Numbers;

        public int Instances = 1;

        public int Matches => Numbers.Where(x => this.WinningNumbers.Contains(x)).Count();

        public int Score()
        {
            var winCount = Matches;
            return winCount == 0 ? 0 : (int)Math.Pow(2, winCount - 1);
        }

        public override void ParseFromLine(string line)
        {
            var parser = new TokenParser(line);
            parser.Skip();
            var firstPart = parser.Pop();
            this.CardId = int.Parse(firstPart.Substring(0, firstPart.Length - 1));
            this.WinningNumbers = parser.PopUntil('|').Select(int.Parse);
            this.Numbers = parser.PopUntil('|').Select(int.Parse);
            base.ParseFromLine(line);
        }
    }
}
