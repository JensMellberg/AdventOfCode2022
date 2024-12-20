using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem13 : PatternProblem<(long x, long y)>
    {
        private const int ACost = 3;
        private const int BCost = 1;
        private const long CostAdjustment = 10000000000000;
        protected override string Pattern => "[Button A|Button B|Prize]: X[+|=]¤x¤, Y[+|=]¤y¤";

        public override void Solve(IEnumerable<(long x, long y)> testInput)
        {
            var testList = testInput.ToList();
            long totalCost = 0;
            long totalCost2 = 0;
            for (var i = 0; i < testList.Count; i+=3)
            {
                var a = testList[i];
                var b = testList[i + 1];
                var price = testList[i + 2];

                var cost1 = CalculateTokenCost(a, b, price);
                totalCost += cost1.HasValue ? cost1.Value : 0;

                var cost2 = CalculateTokenCost(a, b, (price.x + CostAdjustment, price.y + CostAdjustment));
                totalCost2 += cost2.HasValue ? cost2.Value : 0;
            }

            this.PrintResult(totalCost);
            this.PrintResult(totalCost2);
        }

        private static long? CalculateTokenCost((long x, long y) aButton, (long x, long y) bButton, (long x, long y) price)
        {
            var bPresses = (aButton.y * price.x - aButton.x * price.y) / (bButton.x * aButton.y - aButton.x * bButton.y);
            var aPresses = (price.x - bButton.x * bPresses) / aButton.x;
            if (aPresses * aButton.x + bPresses * bButton.x == price.x
                       && aPresses * aButton.y + bPresses * bButton.y == price.y)
            {
                return ACost * aPresses + BCost * bPresses;
            }

            return null;
        }
    }
}
