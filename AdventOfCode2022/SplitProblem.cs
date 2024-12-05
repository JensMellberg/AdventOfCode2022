using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public abstract class SplitProblem<T1, T2> : StringProblem
        where T1 : new()
        where T2 : new()
    {
        protected virtual string SeparatorLine => string.Empty;

        protected virtual string FirstPattern => null;

        protected virtual string SecondPattern => null;

        private PatternParser FirstPatternParser;

        private PatternParser SecondPatternParser;

        protected override EmptyStringBehavior EmptyStringBehavior => EmptyStringBehavior.Keep;

        public override void Solve(IEnumerable<string> testData)
        {
            if (FirstPattern != null)
            {
                this.FirstPatternParser = new PatternParser(FirstPattern);
            }

            if (SecondPattern != null)
            {
                this.SecondPatternParser = new PatternParser(SecondPattern);
            }

            var firstList = new List<T1>();
            var secondList = new List<T2>();
            var hasFoundSeparator = false;
            foreach (var line in testData)
            {
                if (line.Equals(this.SeparatorLine))
                {
                    hasFoundSeparator = true;
                }
                else if (!hasFoundSeparator)
                {
                    var item = this.Convert<T1>(line, FirstPatternParser);
                    firstList.Add(item);
                }
                else
                {
                    var item = this.Convert<T2>(line, SecondPatternParser);
                    secondList.Add(item);
                }
            }

            this.Solve(firstList, secondList);
        }

        protected abstract void Solve(IEnumerable<T1> testData1, IEnumerable<T2> testData2);

        private T Convert<T>(string input, PatternParser parser) where T : new()
        {
            if (parser != null)
            {
                return parser.ParseObject<T>(input);
            }

            if (typeof(Parsable).IsAssignableFrom(typeof(T))) {
                var returnObject = new T();
                (returnObject as Parsable).ParseFromLine(input);
                return returnObject;
            }

            throw new Exception("A split problem can only use classes with patterns or that extends Parsable.");
        }
    }
}
