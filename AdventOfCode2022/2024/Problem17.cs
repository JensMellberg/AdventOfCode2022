using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem17 : SplitProblem<(string register, long value), (string temp, ListParsable<int> list)>
    {
        protected override string FirstPattern => "Register ¤register¤: ¤value¤";

        protected override string SecondPattern => "¤temp¤: ¤list¤";
        protected override void Solve(IEnumerable<(string register, long value)> registerInput, IEnumerable<(string temp, ListParsable<int> list)> instruction)
        {
            var registers = registerInput.ToDictionary(x => x.register, x => x.value);
            var instructions = instruction.Single().list.Values;
            this.PrintResult(this.GetOutput(registers, instructions));
           
            var result = BuildResult(instructions.Count - 1, 0);
            this.PrintResult(result);

            ulong? BuildResult(int index, ulong previous)
            {
                if (index == -1)
                {
                    return previous;
                }

                previous *= 8;
                for (uint x = 0; x < 8; x++)
                {
                    if (SimplifiedProgram(previous + x) == instructions[index])
                    {
                        var res = BuildResult(index - 1, previous + x);
                        if (res.HasValue)
                        {
                            return res;
                        }
                    }
                }

                return null;
            }
        }

        private uint SimplifiedProgram(ulong regA)
        {
            ulong regB;
            ulong regC;
            regB = regA % 8;
            regB = regB ^ 1;
            regC = (ulong)(regA / Math.Pow(2, regB));
            regA /= 8;
            regB = regB ^ 4;
            regB = regB ^ regC;
            return (uint)(regB % 8);
        }

        private string GetOutput(Dictionary<string, long> registers, IList<int> instructions)
        {
            var instructionPointer = 0;
            var outputs = new List<int>();
            while (instructionPointer < instructions.Count)
            {
                var value = instructions[instructionPointer + 1];
                switch (instructions[instructionPointer])
                {
                    case 0:
                        registers["A"] = (int)(registers["A"] / Math.Pow(2, GetComboValue(value)) % int.MaxValue);
                        break;
                    case 1:
                        registers["B"] = registers["B"] ^ value;
                        break;
                    case 2:
                        registers["B"] = GetComboValue(value) % 8;
                        break;
                    case 3:
                        if (registers["A"] != 0)
                        {
                            instructionPointer = value;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    case 4:
                        registers["B"] = registers["B"] ^ registers["C"];
                        break;
                    case 5:
                        outputs.Add((int)(GetComboValue(value) % 8));
                        break;
                    case 6:
                        registers["B"] = (int)(registers["A"] / Math.Pow(2, GetComboValue(value)) % int.MaxValue);
                        break;
                    case 7:
                        registers["C"] = (int)(registers["A"] / Math.Pow(2, GetComboValue(value)) % int.MaxValue);
                        break;
                    default: break;
                }

                instructionPointer += 2;
            }

            return string.Join(',', outputs);

            long GetComboValue(int value)
            {
                switch (value)
                {
                    case 4: return registers["A"];
                    case 5: return registers["B"];
                    case 6: return registers["C"];
                    default: return value;
                }
            }
        }
    }
}
