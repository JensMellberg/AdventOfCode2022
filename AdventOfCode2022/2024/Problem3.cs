using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem3 : StringProblem
    {
        public override void Solve(IEnumerable<string> testData)
        {
            const string Mul = "mul(";
            const string Do = "do()";
            const string Dont = "don't()";
            var instruction = string.Join("", testData);
            var index = 0;
            var sum = 0;
            var conditionalSum = 0;
            var isEnabled = true;
            while (index < instruction.Length)
            {
                if (instruction[index] == Mul[0] && instruction.Substring(index, 4) == Mul)
                {
                    index += 4;
                    var number = ReadNumber();
                    if (number == "" || instruction[index] != ',')
                    {
                        continue;
                    }

                    index++;
                    var number2 = ReadNumber();
                    if (number == "" || instruction[index] != ')')
                    {
                        continue;
                    }

                    var product = int.Parse(number) * int.Parse(number2);
                    sum += product;
                    conditionalSum = isEnabled ? conditionalSum + product : conditionalSum;
                }
                else if (instruction[index] == Do[0] && instruction.Substring(index, 4) == Do)
                {
                    isEnabled = true;
                    index += 3;
                }
                else if (instruction[index] == Dont[0] && instruction.Substring(index, 7) == Dont)
                {
                    isEnabled = false;
                    index += 6;
                }

                index++;

                string ReadNumber()
                {
                    var number = "";
                    while (ParsableUtils.IsNumber(instruction[index]))
                    {
                        number += instruction[index];
                        index++;
                    }

                    return number;
                }
            }

            this.PrintResult(sum);
            this.PrintResult(conditionalSum);
        }
    }
}
