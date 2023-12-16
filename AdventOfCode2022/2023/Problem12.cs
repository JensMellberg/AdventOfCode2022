using System;
using System.Collections.Generic;
using System.Linq;
using AdventOfCode2022.TwentyTwo;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem12 : ObjectProblem<SpringRow>
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public static IDictionary<(string, string), long> memory = new Dictionary<(string, string), long>();
        public override void Solve(IEnumerable<SpringRow> testData)
        {
            testData.ForEach(x => this.Print(x.originalLine + "  :" + x.Combinations()));
            this.PrintResult(testData.Sum(x => x.Combinations()));
            var newData = testData.Select(x => x.Unfold());

            long total = 0;
            foreach (var data in newData)
            {
                memory.Clear();
                var combs = data.Combinations();
                total += combs;
            }

            this.PrintResult(total);
        }
    }

    public class SpringRow : Parsable
    {
        public SpringRow() { }

        public SpringRow(string row, int[] configs, int currentCount, bool isFinished)
        {
            this.row = row;
            this.configs = configs;
            this.currentCount = currentCount;
            this.isFinished = isFinished;
            this.initialFinished = isFinished;
        }

        string row;
        int[] configs;
        int currentCount = 0;
        bool isFinished = false;
        bool initialFinished = false;

        public SpringRow Unfold()
        {
            var newRow = this.row + "?" + this.row + "?" + this.row + "?" + this.row + "?" + this.row;
            var newConfig = this.configs.Concat(this.configs).Concat(this.configs).Concat(this.configs).Concat(this.configs).ToArray();
            return new SpringRow(newRow, newConfig, 0, false);
        }

        public override void ParseFromLine(string line)
        {
            var tokens = line.Split(' ');
            this.row = tokens[0];
            this.configs = tokens[1].Split(',').Select(int.Parse).ToArray();
            base.ParseFromLine(line);
        }

        private string Stringify() => string.Join(",", this.configs) + "#" + this.currentCount + "|" + this.initialFinished;

        public long Combinations()
        {
            var config = configs.Length == 0 ? -1 : configs[0];

            if (this.row.Length == 0)
            {
                if (isFinished && currentCount > 0)
                {
                    return 0;
                }

                if (!isFinished && currentCount == config && configs.Length == 1)
                {
                    this.isFinished = true;
                }

                if (currentCount > 0 && currentCount < config)
                {
                    return 0;
                }

                var result = this.isFinished ? 1 : 0;
                return result;
            }

            var configsLeft = this.Stringify();

            if (Problem12.memory.TryGetValue((this.row, configsLeft), out var value5))
            {
                return value5;
            }

            var current = this.row[0];
            if (current == '#')
            {
                if (isFinished || currentCount >= config)
                {
                    return 0;
                }

                var result = new SpringRow(row[1..], configs, currentCount + 1, this.isFinished).Combinations();
                StoreState(result);
                return result;
            }
            else if (current == '?')
            {
                var firstRow = HashTagRow();
                var secondRow = DotRow();
                var result = firstRow.Combinations() + secondRow.Combinations();
                return result;

                SpringRow HashTagRow() => new SpringRow("#" + row[1..], configs, this.currentCount, this.isFinished);
                SpringRow DotRow() => new SpringRow("." + row[1..], configs, this.currentCount, this.isFinished);
            }
            else
            {
                if (isFinished && currentCount > 0)
                {
                    return 0;
                }

                var newConfigs = configs;
                var newCount = currentCount;

                if (currentCount == config)
                {
                    newConfigs = configs[1..];
                    newCount = 0;
                    if (newConfigs.Length == 0)
                    {
                        this.isFinished = true;
                    }
                }

                if (currentCount > 0 && currentCount < config)
                {
                    return 0;
                }

                var result = new SpringRow(row[1..], newConfigs, newCount, this.isFinished).Combinations();
                StoreState(result);
                return result;
            }
        }

        private void StoreState(long storedValue)
        {
            var configsLeftstring = this.Stringify();
            TryAdd(row, configsLeftstring, storedValue);
        }

        private void TryAdd(string rowy, string configString, long valuee)
        {
            if (!Problem12.memory.ContainsKey((rowy, configString)) && !string.IsNullOrEmpty(rowy))
            {
                Problem12.memory.Add((rowy, configString), valuee);
            }
        }
    }
}
