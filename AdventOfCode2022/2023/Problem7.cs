using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem7 : ObjectProblem<CardHand>
    {
        public override void Solve(IEnumerable<CardHand> testData)
        {
            var orderedData = testData.ToList();
            orderedData.Sort();
            this.PrintResult(orderedData.Select((x, i) => (i + 1) * x.Bid).Sum());
            orderedData.ForEach(x =>
            {
                x.Rank = x.JokerRank;
                x.JokerRank = HandRank.Inactive;
            });
            orderedData.Sort();
            this.PrintResult(orderedData.Select((x, i) => (i + 1) * x.Bid).Sum());
        }
     }

    public class CardHand : Parsable, IComparable<CardHand>
    {
        public long Bid { get; set; }

        public HandRank Rank { get; set; }

        public HandRank JokerRank { get; set; }

        private string Hand { get; set; }

        public int CompareTo(CardHand other)
        {
            if (this.Rank > other.Rank)
            {
                return 1;
            }
            else if (this.Rank < other.Rank)
            {
                return -1;
            }

            for (var i = 0; i < this.Hand.Length; i++)
            {
                var strength = CardStrength(this.Hand[i]);
                var otherStrength = CardStrength(other.Hand[i]);
                if (strength > otherStrength)
                {
                    return 1;
                }
                else if (strength < otherStrength)
                {
                    return -1;
                }
            }

            return 0;
        }

        public override void ParseFromLine(string line)
        {
            var parser = new TokenParser(line);
            var hand = parser.Pop();
            this.Bid = int.Parse(parser.Pop());
            this.SetHand(hand);
            this.SetJokerHand(hand);
            base.ParseFromLine(line);
        }

        private int CardStrength(char card)
        {
            var JScore = this.JokerRank == HandRank.Inactive ? 1 : 11;
            return card switch
            {
                'A' => 14,
                'K' => 13,
                'Q' => 12,
                'J' => JScore,
                'T' => 10,
                _ => card - '0',
            };
        }

        private HandRank GetRank(string hand)
        {
            var hasTriple = false;
            var hasPair = false;
            while (hand.Length > 1)
            {
                var crnt = hand[0];
                var remaining = hand[1..];
                var count = remaining.Count(x => x == crnt);
                if (count == 4)
                {
                   return HandRank.FiveOfAKind;
                }
                else if (count == 3)
                {
                    return HandRank.FourOfAKind;
                }
                else if (count == 2)
                {
                    hasTriple = true;
                }
                else if (count == 1)
                {
                    if (hasPair)
                    {
                        return HandRank.TwoPair;
                    }

                    hasPair = true;
                }

                if (hasPair && hasTriple)
                {
                    return HandRank.FullHouse;
                }

                hand = remaining.Replace(crnt.ToString(), "");
            }

            if (hasTriple)
            {
                return HandRank.ThreeOfAKind;
            }
            else if (hasPair)
            {
                return HandRank.Pair;
            }
            else
            {
                return HandRank.None;
            }
        }

        private void SetHand(string hand)
        {
            this.Hand = hand;
            this.Rank = this.GetRank(hand);
        }

        private void SetJokerHand(string hand)
        {
            var jokerIndexes = new List<int>();
            for (var i = 0; i < hand.Length; i++)
            {
                if (hand[i] == 'J')
                {
                    jokerIndexes.Add(i);
                }
            }

            if (jokerIndexes.Count == 5 || jokerIndexes.Count == 4)
            {
                this.JokerRank = HandRank.FiveOfAKind;
                return;
            }
            else if (jokerIndexes.Count == 0)
            {
                this.JokerRank = this.GetRank(hand);
                return;
            }

            var distinctCards = hand.Select(x => x).Where(x => x != 'J').Distinct().ToList();
            var bestRank = HandRank.None;
            for (var i = 0; i < distinctCards.Count; i++)
            {
                var newHand = hand;
                newHand = hand.ReplaceAtIndex(jokerIndexes[0], distinctCards[i]);
                if (jokerIndexes.Count > 1)
                {
                    for (var i2 = 0; i2 < distinctCards.Count; i2++)
                    {
                        newHand = newHand.ReplaceAtIndex(jokerIndexes[1], distinctCards[i2]);
                        if (jokerIndexes.Count > 2)
                        {
                            for (var i3 = 0; i3 < distinctCards.Count; i3++)
                            {
                                newHand = newHand.ReplaceAtIndex(jokerIndexes[2], distinctCards[i3]);
                                var newRank = this.GetRank(newHand);
                                if (newRank > bestRank)
                                {
                                    bestRank = newRank;
                                }
                            }
                        }
                        else
                        {
                            var newRank = this.GetRank(newHand);
                            if (newRank > bestRank)
                            {
                                bestRank = newRank;
                            }
                        }
                    }
                }
                else
                {
                    var newRank = this.GetRank(newHand);
                    if (newRank > bestRank)
                    {
                        bestRank = newRank;
                    }
                }
            }

            this.JokerRank = bestRank;
        }
    }

    /*private List<string> CreateCombinations(int startIndex, string pair, string[] initialArray)
    {
        var combinations = new List<string>();
        for (int i = startIndex; i < initialArray.Length; i++)
        {
            var value = $"{pair}{initialArray[i]}";
            combinations.Add(value);
            combinations.AddRange(CreateCombinations(i + 1, value, initialArray));
        }

        return combinations;
    }*/

    public enum HandRank
    {
        FiveOfAKind = 6,
        FourOfAKind = 5,
        FullHouse = 4,
        ThreeOfAKind = 3,
        TwoPair = 2,
        Pair = 1,
        None = 0,
        Inactive = -1
    }
}
