using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem19 : StringProblem
    {
        private class Range
        {
            public int Start { get; set; }
            
            public int End { get; set; }

            public long Total => End - Start + 1;
        }

        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        protected override EmptyStringBehavior EmptyStringBehavior => EmptyStringBehavior.Keep;

        public static Dictionary<string, List<int>> BreakPoints = new Dictionary<string, List<int>>();

        private long AcceptedCount(RangeInput rangeInput, WorkFlow current, Dictionary<string, WorkFlow> workflows)
        {
            long total = 0;
            foreach (var instr in current.Instructions)
            {
                var inValue = instr.Condition.Value;
                if (inValue != null)
                {
                    var relevantRanges = rangeInput.Ranges[inValue];
                    var (meets, leftovers) = instr.Condition.Meets(relevantRanges);
                    var newInput = rangeInput.Copy();
                    newInput.Ranges[inValue] = meets;
                    rangeInput.Ranges[inValue] = leftovers;
                    total += AcceptedCount(newInput, instr, workflows);
                }
                else
                {
                    total += AcceptedCount(rangeInput, instr, workflows);
                    break;
                }
            }

            return total;
        }

        private long AcceptedCount(RangeInput rangeInput, WorkFlowInstruction current, Dictionary<string, WorkFlow> workflows)
        {
            if (rangeInput.Ranges.Values.Any(x => !x.Any()))
            {
                return 0;
            }

            if (current.Result.Accept)
            {
                long total = 1;
                rangeInput.Ranges.Values.ForEach(x => total *= x.Sum(x => x.Total));
                return total;
            }
            else if (current.Result.Reject)
            {
                return 0;
            }

            return AcceptedCount(rangeInput, workflows[current.Result.GoTo], workflows);
        }

        public override void Solve(IEnumerable<string> testData)
        {
            BreakPoints.Clear();
            BreakPoints.Add("x", new List<int>());
            BreakPoints.Add("m", new List<int>());
            BreakPoints.Add("a", new List<int>());
            BreakPoints.Add("s", new List<int>());
            (var workflows, var inputs) = this.Parse(testData);
            long total = 0;
            foreach (var i in inputs)
            {
                if (this.IsAccepted(workflows, i))
                {
                    total += i.Total;
                }
            }

            this.PrintResult(total);

            //** Part 2 */

            BreakPoints.Keys.ForEach(x => BreakPoints[x].Add(1));
            BreakPoints.Values.ForEach(x => x.Sort());

            var input = new Input();
            var ranges = new Dictionary<string, IList<Range>>();
            foreach (var type in BreakPoints.Keys)
            {
                var list = BreakPoints[type];
                var rangeList = new List<Range>();
                ranges.Add(type, rangeList);
                for (var i = 0; i < list.Count; i++)
                {
                    if (i < list.Count - 1)
                    {
                        rangeList.Add(new Range { Start = list[i], End = list[i + 1] - 1 });
                    }
                    else
                    {
                        rangeList.Add(new Range { Start = list[i], End = 4000 });
                    }
                }
            }

            var rangeInput = new RangeInput { Ranges = ranges };
            this.PrintResult(this.AcceptedCount(rangeInput, workflows["in"], workflows));
        }

        private bool IsAccepted(Dictionary<string, WorkFlow> workflows, Input input)
        {
            var current = "in";
            while (true)
            {
                var result = workflows[current].GetResult(input);
                if (result.Accept)
                {
                    return true;
                }
                else if (result.Reject)
                {
                    break;
                }

                current = result.GoTo;
            }

            return false;
        }

        private (Dictionary<string, WorkFlow> workflows, List<Input> inputs) Parse(IEnumerable<string> testData)
        {
            var workFlows = new Dictionary<string, WorkFlow>();
            var input = new List<Input>();
            var outerParser = new TokenParser(testData.ToArray());
            var flow = outerParser.Pop();
            while (flow != string.Empty)
            {
                var tokens = flow.Split('{');
                var name = tokens[0];
                var workFlow = new WorkFlow(tokens[1][..^1]);
                workFlows.Add(name, workFlow);
                flow = outerParser.Pop();
            }

            var line = outerParser.Pop();
            while (!outerParser.IsFinished && !string.IsNullOrEmpty(line))
            {
                input.Add(new Input(line[1..^1]));
                line = outerParser.Pop();
            }

            return (workFlows, input);
        }
      
        private class WorkFlow
        {
            public List<WorkFlowInstruction> Instructions { get; set; } = new List<WorkFlowInstruction>();

            public WorkFlowResult GetResult(Input input)
            {
                foreach (var i in this.Instructions)
                {
                    if (i.Condition.Meets(input))
                    {
                        return i.Result;
                    }
                }

                return null;
            }

            public WorkFlow(string line)
            {
                var parser = new TokenParser(line, ',');
                while (!parser.IsFinished)
                {
                    var next = parser.Pop();
                    var tokens = next.Split(':');
                    Condition condition;
                    if (tokens.Length > 1)
                    {
                        var value = tokens[0][0].ToString();
                        var op = tokens[0][1];
                        var comparer = int.Parse(tokens[0][2..]);
                        condition = new StandardCondition(value, op, comparer);
                    }
                    else
                    {
                        condition = new TrueCondition();
                    }

                    var goTo = tokens.Last();
                    WorkFlowResult result = goTo switch
                    {
                        "A" => new WorkFlowResult { Accept = true },
                        "R" => new WorkFlowResult { Reject = true },
                        _ => new WorkFlowResult { GoTo = goTo },
                    };
                    
                    this.Instructions.Add(new WorkFlowInstruction { Condition = condition, Result = result });
                }
            }
        }

        private class WorkFlowInstruction
        {
            public Condition Condition;

            public WorkFlowResult Result { get; set; }
        }

        private interface Condition
        {
            bool Meets(Input input);

            (IList<Range> meets, IList<Range> leftovers) Meets(IList<Range> ranges);

            string Value { get; }
        }

        private class TrueCondition : Condition
        {
            public bool Meets(Input input) => true;

            public (IList<Range> meets, IList<Range> leftovers) Meets(IList<Range> ranges) => (ranges, new List<Range>());

            public string Value => null;
        }

        private class StandardCondition : Condition
        {
            public string value;
            private int comparer;
            private char op;

            public string Value => this.value;
            public StandardCondition(string value, char op, int comparer)
            {
                this.value = value;
                this.comparer = comparer;
                this.op = op;
                if (op == '<')
                {
                    BreakPoints[value].Add(comparer);
                }
                else
                {
                    BreakPoints[value].Add(comparer + 1);
                }
            }

            public bool Meets(Input input) => op == '<' ? input.GetValue(this.value) < this.comparer : input.GetValue(this.value) > this.comparer;

            public (IList<Range> meets, IList<Range> leftovers) Meets(IList<Range> ranges)
            {
                var meets = new List<Range>();
                var leftOvers = new List<Range>();
                if (op == '<')
                {
                    foreach (var r in ranges)
                    {
                        if (r.Start < this.comparer)
                        {
                            meets.Add(r);
                        }
                        else
                        {
                            leftOvers.Add(r);
                        }
                    }
                }
                else
                {
                    foreach (var r in ranges)
                    {
                        if (r.End > this.comparer)
                        {
                            meets.Add(r);
                        }
                        else
                        {
                            leftOvers.Add(r);
                        }
                    }
                }

                return (meets, leftOvers);
            }
        }

        private class Input
        {
            private Dictionary<string, int> values = new Dictionary<string, int>();
            public int GetValue(string value) => values[value];

            public long Total => values.Values.Sum();

            public void Set(string value, int valueValue)
            {
                this.values[value] = valueValue;
            }

            public Input() { }

            public Input(string line)
            {
                var parser = new TokenParser(line, ',');
                while (!parser.IsFinished)
                {
                    var tokens = parser.Pop().Split('=');
                    this.values.Add(tokens[0], int.Parse(tokens[1]));
                }
            }
        }

        private class RangeInput
        {
            public Dictionary<string, IList<Range>> Ranges = new Dictionary<string, IList<Range>>();

            public RangeInput Copy()
            {
                var newRanges = Ranges.ToDictionary(x => x.Key, x => x.Value);
                return new RangeInput { Ranges = newRanges };
            }
        }

        private class WorkFlowResult
        {
            public string GoTo { get; set; }

            public bool Accept { get; set; }

            public bool Reject { get; set; }
        }
    }
}
