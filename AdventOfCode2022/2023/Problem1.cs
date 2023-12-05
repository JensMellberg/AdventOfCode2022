using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem1 : StringProblem
    {
        private string[] Numbers = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
        public override void Solve(IEnumerable<string> testData)
        {
            this.PrintResult(testData.Select(x => this.GetCalibrationValue(x)).Sum());
            this.PrintResult(testData.Select(x => this.GetRealCalibrationValue(x)).Sum());
        }

        private int GetCalibrationValue(string line)
        {
            string first = null;
            string second = null;
            foreach (var c in line)
            {
                if (c >= '0' && c <= '9')
                {
                    if (first == null)
                    {
                        first = c.ToString();
                    }

                    second = c.ToString();
                }
            }

            return int.Parse(first + second);
        }

        private int GetRealCalibrationValue(string line)
        {
            string first = null;
            string second = null;
            string current = string.Empty;
            var i = 0;
            while (i < line.Length)
            {
                var c = line[i];
                if (c >= '0' && c <= '9')
                {
                    SetValue(c.ToString());
                } 
                else
                {
                    current = c.ToString();
                    var newI = i;
                    while (this.Numbers.Any(x => x.StartsWith(current)) && newI < line.Length - 1)
                    {
                        newI++;
                        var c2 = line[newI];
                        current += c2;
                        if (this.Numbers.Contains(current))
                        {
                            var cNbr = Array.IndexOf(this.Numbers, current);
                            current = string.Empty;
                            SetValue(cNbr.ToString());
                        }
                    }
                }

                i++;
            }

            return int.Parse(first + second);

            void SetValue(string value)
            {
                if (first == null)
                {
                    first = value;
                }

                second = value;
            }
        }
    }
}
