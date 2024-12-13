using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem7 : ObjectProblem<Equation>
    {
        public override void Solve(IEnumerable<Equation> testData)
        {
            this.PrintResult(testData.Where(x => x.IsSolvable()).Sum(x => x.result));
            this.PrintResult(testData.Where(x => x.IsSolvableWithConcat()).Sum(x => x.result));
        }    
    }

    public class Equation : Parsable
    {
        public long result;
        private long[] values;
        public bool IsSolvable() => this.IsSolvableWith(1, 0, '*', false) || this.IsSolvableWith(0, 0, '+', false);

        public bool IsSolvableWithConcat() => this.IsSolvableWith(1, 0, '*', true) 
            || this.IsSolvableWith(0, 0, '+', true)
            || this.IsSolvableWith(0, 0, '|', true);

        public bool IsSolvableWith(long previousValue, int index, char op, bool allowConcat)
        {
            if (previousValue > result)
            {
                return false;
            }

            long newValue;
            switch (op)
            {
                case '*': newValue = previousValue * values[index]; break;
                case '+': newValue = previousValue + values[index]; break;
                case '|': newValue = long.Parse(previousValue.ToString() + values[index].ToString()); break;
                default: throw this.Exception;
            }

            if (index == values.Length - 1)
            {
                return newValue == this.result;
            }
            else
            {
                return this.IsSolvableWith(newValue, index + 1, '*', allowConcat)
                    || this.IsSolvableWith(newValue, index + 1, '+', allowConcat)
                    || allowConcat && this.IsSolvableWith(newValue, index + 1, '|', true);
            }
        }
        public override void ParseFromLine(string line)
        {
            var parser = new TokenParser(line, ' ');
            var result = parser.Pop();
            this.result = long.Parse(result.Substring(0, result.Length - 1));
            values = parser.PopUntil('X').Select(long.Parse).ToArray();
            base.ParseFromLine(line);
        }
    }
}
