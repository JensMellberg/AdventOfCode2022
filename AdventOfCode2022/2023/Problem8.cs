using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem8 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;
        public static IDictionary<(string, long), (long, string)> StepsFromIndex = new Dictionary<(string, long), (long, string)>();
        public static HashSet<(string, long)> reserved = new HashSet<(string, long)>();
        public static int TotalAdded = 0;
        public static int NullSteps = 0;

        public override void Solve(IEnumerable<string> testData)
        {
            var lineParser = new PatternParser("¤location¤ = (¤left¤, ¤right¤)");
            var testArray = testData.ToArray();
            var testParser = new TokenParser(testArray);
            var instructionString = testParser.Pop();
            var instructions = new Instructions(instructionString);
            var nodebyLocation = new Dictionary<string, Node>();

            while (!testParser.IsFinished)
            {
                var (location, left, right) = lineParser.ParseObject<(string location, string left, string right)>(testParser.Pop());
                nodebyLocation.Add(location, new Node(left, right));
            }

            var current = "AAA";
            var steps = 0;
            while (current != "ZZZ")
            {
                var instruction = instructions.GetInstruction();
                instructions.Next();
                current = nodebyLocation[current].GetFromInstruction(instruction);
                steps++;
            }

            this.PrintResult(steps);

            var currents = nodebyLocation.Keys.Where(x => x[2] == 'A').Select(x => new Ghost(x)).ToList();
            var goal = nodebyLocation.Keys.Where(x => x[2] != 'Z').Count() * instructionString.Length;
            steps = 0;
            var nbrOnZ = 0;

            // Just assume we filled the dictionary after 100000 steps that didn't add anything.
            while (NullSteps < 100000)
            {
                var instruction = instructions.GetInstruction();
                for (var i = 0; i < currents.Count; i++)
                {
                    var crnt = currents[i];
                    var wasOnZ = crnt.Current[2] == 'Z';
                    var next = nodebyLocation[crnt.Current].GetFromInstruction(instruction);
                    currents[i].UpdateDict(next, (instructions.Index + 1) % instructionString.Length);
                    var isOnZ = currents[i].Current[2] == 'Z';
                    if (wasOnZ && !isOnZ)
                    {
                        nbrOnZ--;
                    }
                    else if (!wasOnZ && isOnZ)
                    {
                        nbrOnZ++;
                    }
                }

                instructions.Next();
                steps++;
            }

            var newCurrents = currents.Select(x => (x.Start, (long)0, (long)0)).ToList();
            var currentIndex = 0;
            steps = 0;
            var stepsThatEndsOnZ = new Dictionary<long, int>();
            while (steps < 1)
            {
               steps++;
               for (var i = 0; i < newCurrents.Count; i++)
               {
                    var crnt = newCurrents[i];
                    var newPlace = StepsFromIndex[(crnt.Start, crnt.Item2)];
                    crnt.Item3 = crnt.Item3 + newPlace.Item1;
                    if (!stepsThatEndsOnZ.ContainsKey(crnt.Item3))
                    {
                        stepsThatEndsOnZ.Add(crnt.Item3, 1);
                    }
                    else
                    {
                        stepsThatEndsOnZ[crnt.Item3]++;
                        if (stepsThatEndsOnZ[crnt.Item3] == newCurrents.Count)
                        {
                            this.PrintResult(crnt.Item3);
                            return;
                        }
                    }

                    crnt.Start = newPlace.Item2;
                    crnt.Item2 = (crnt.Item2 + newPlace.Item1) % instructionString.Length;
                    newCurrents[i] = crnt;
                }
            }

            this.PrintResult(ParsableUtils.LowestCommonMultiple(newCurrents.Select(x => x.Item3)));
        }

        private class Instructions
        {
            private int pointer = 0;

            private string line;
            public Instructions(string line) {
                this.line = line;
            }

            public char GetInstruction() => line[pointer];

            public void Next()
            {
                pointer++;
                if (pointer == line.Length)
                {
                    pointer = 0;
                }
            }

            public int Index => this.pointer;
        }

        private class Ghost
        {
            public string Start { get; set; }
            public string Current { get; set; }
            public Ghost(string current)
            {
                this.Current = current;
                this.Start = current;
                if (!reserved.Contains((current, 0)))
                {
                    reserved.Add((current, 0));
                    reservedLocal.Add((current, 0), 0);

                }
            }

            public IDictionary<(string, long), long> reservedLocal = new Dictionary<(string, long), long>();

            private int StepsSinceLast = 0;

            public void UpdateDict(string current, long index)
            {
                if (current == "11Z")
                {
                    var a = 5;
                }
                StepsSinceLast++;
                this.Current = current;
                if (current[2] == 'Z' && reservedLocal.Count > 0)
                {
                    foreach (var c in reservedLocal)
                    {
                        Problem8.StepsFromIndex.Add(c.Key, (StepsSinceLast - c.Value, current));
                        Problem8.TotalAdded++;
                    }

                    StepsSinceLast = 0;
                    reservedLocal.Clear();
                    if (!reserved.Contains((current, index)))
                    {
                        reserved.Add((current, index));
                        reservedLocal.Add((current, index), 0);
                    }

                    NullSteps = 0;
                    return;
                }

                if (!reserved.Contains((current, index)))
                {
                    reserved.Add((current, index));
                    reservedLocal.Add((current, index), StepsSinceLast);
                    NullSteps = 0;
                } 
                else
                {
                    NullSteps++;
                }
            }
        }

        private class Node
        {
            public string Left { get; set; }

            public string Right { get; set; }

            public Node(string left, string right)
            {
                this.Left = left;
                this.Right = right;
            }

            public string GetFromInstruction(char instruction) => instruction == 'L' ? Left : Right;
        }
    }
}
