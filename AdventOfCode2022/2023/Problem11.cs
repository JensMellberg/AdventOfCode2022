using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem12 : ObjectProblem<SpringRow>
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;
        public override void Solve(IEnumerable<SpringRow> testData)
        {
            //testData.ForEach(x => this.Print(x.originalLine + "  :" + x.Combinations()));
            // var x = testData.Last();
            //this.Print(x.originalLine + "  :" + x.Combinations());
            this.PrintResult(testData.Sum(x => x.Combinations()));
            var newData = testData.Select(x => x.Unfold());
            // this.PrintResult(newData.Sum(x => x.Combinations()));
            long total = 0;
            foreach (var data in newData)
            {
                var combs = data.Combinations();
                this.Print(data.originalLine + "  :" + combs);
                total += combs;
            }

            this.PrintResult(total);
        }
    }

    public class SpringRow : Parsable
    {
        public SpringRow() { }

        public SpringRow(string row, int[] configs, int currentCount)
        {
            this.row = row;
            this.configs = configs;
            this.currentCount = currentCount;
        }

        string row;
        int[] configs;
        int rowPointer = 0;
        int configPointer = 0;
        int currentCount = 0;
        bool isFinished = false;

        public SpringRow Unfold()
        {
            var newRow = this.row + "?" + this.row + "?" + this.row + "?" + this.row + "?" + this.row;
            var newConfig = this.configs.Concat(this.configs).Concat(this.configs).Concat(this.configs).Concat(this.configs).ToArray();
            return new SpringRow(newRow, newConfig, 0);
        }
        public override void ParseFromLine(string line)
        {
            var tokens = line.Split(' ');
            this.row = tokens[0];
            this.configs = tokens[1].Split(',').Select(int.Parse).ToArray();
            base.ParseFromLine(line);
        }

        public long Combinations()
        {
            while (rowPointer < row.Length)
            {
                if (row[rowPointer] == '#')
                {
                    currentCount++;
                    if (isFinished || currentCount > configs[configPointer])
                    {
                        return 0;
                    }
                }
                else if (row[rowPointer] == '?')
                {
                    if (isFinished)
                    {
                        rowPointer++;
                        continue;
                    }

                    if (currentCount == configs[configPointer])
                    {
                        configPointer++;
                        currentCount = 0;
                        if (configPointer == configs.Length)
                        {
                            this.isFinished = true;
                        }
                    }
                    else if (currentCount > 0 && currentCount < configs[configPointer])
                    {
                        currentCount++;
                    }
                    else
                    {
                        var newRow = "#" + row[(rowPointer + 1)..];
                        var newRow2 = "." + row[(rowPointer + 1)..];
                        var newConfigs = configs[configPointer..];
                        var firstRow = new SpringRow(newRow, newConfigs, this.currentCount);
                        var secondRow = new SpringRow(newRow2, newConfigs, this.currentCount);
                        return firstRow.Combinations() + secondRow.Combinations();
                    }
                }
                else
                {
                    if (isFinished && currentCount > 0)
                    {
                        return 0;
                    }

                    if (!isFinished && currentCount == configs[configPointer])
                    {
                        configPointer++;
                        currentCount = 0;
                        if (configPointer == configs.Length)
                        {
                            this.isFinished = true;
                        }
                    }

                    if (!isFinished && currentCount > 0 && currentCount < configs[configPointer])
                    {
                        return 0;
                    }
                }

                rowPointer++;
            }

            if (isFinished && currentCount > 0 || isFinished && configPointer < configs.Length - 1)
            {
                return 0;
            }

            if (!isFinished && configs[configPointer] == currentCount && configPointer == configs.Length - 1)
            {
                isFinished = true;
            }

            if (isFinished)
            {
             //   Console.WriteLine(fullLine);
            }

            return isFinished ? 1 : 0;
        }
    }
}
