using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem20 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public static long PressesUntilRx = 0;

        public static long Presses = 0;

        public override void Solve(IEnumerable<string> testData)
        {
            var modules = testData.Select(x => Module.FromLine(x)).ToDictionary(x => x.Name, x => x);
            foreach (var mS in modules.Values.SelectMany(x => x.TargetStrings).Distinct().ToArray())
            {
                if (!modules.ContainsKey(mS))
                {
                    modules[mS] = new EmptyModule(mS);
                }
            }

            foreach (var m in modules.Values)
            {
                m.Targets = m.TargetStrings.Select(x => modules[x]).ToList();
                m.Targets.ForEach(x => x.Inputs.Add(m));
            }

            modules.Values.ForEach(x => x.Init());

            var broadcaster = modules["broadcaster"];
            var queue = new Queue<(Module from, List<Module> targets, bool signal)>();
            long lowCount = 0;
            long highCount = 0;

            while (PressesUntilRx == 0)
            {
                Presses++;
                if (Presses == 1001)
                {
                    this.PrintResult(highCount * lowCount);
                }

                queue.Enqueue((null, new[] { broadcaster }.ToList(), false));

                while (queue.Any())
                {
                    var (from, targets, signal) = queue.Dequeue();
                    var count = targets.Count;
                    if (signal)
                    {
                        highCount += count;
                    }
                    else
                    {
                        lowCount += count;
                    }

                    foreach (var m in targets)
                    {
                        m.Receive(from, signal, queue);
                    }
                }
            }

            
            this.PrintResult(PressesUntilRx);
        }

        private abstract class Module
        {
            public List<Module> Targets { get; set; }

            public List<Module> Inputs { get; set; } = new List<Module>();

            public List<string> TargetStrings { get; set; }

            public string Name { get; set; }

            public virtual void Init() { }

            public abstract void Receive(Module from, bool signal, Queue<(Module from, List<Module> targets, bool signal)> sendQueue);


            public static Module FromLine(string line)
            {
                var parser = new TokenParser(line);
                string name;
                var frst = parser.Pop();
                parser.Pop();
                var targetStrings = new List<string>();
                while (!parser.IsFinished)
                {
                    targetStrings.Add(parser.PopUntil(',')[0]);
                }
               
                if (frst[0] == '%')
                {
                    name = frst[1..];
                    return new FlipFlop
                    {
                        Name = name,
                        TargetStrings = targetStrings
                    };
                }
                else if (frst[0] == '&')
                {
                    name = frst[1..];

                    // Hacky but too much work to find it programmatically
                    if (name == "jz")
                    {
                        return new ImportantConjunction
                        {
                            Name = name,
                            TargetStrings = targetStrings
                        };
                    }

                    return new Conjunction
                    {
                        Name = name,
                        TargetStrings = targetStrings
                    };
                }
                else
                {
                    return new Broadcaster
                    {
                        TargetStrings = targetStrings
                    };
                }
            }
        }

        private class FlipFlop : Module
        {
            private bool currentState = false;

            public override void Receive(Module from, bool signal, Queue<(Module from, List<Module> targets, bool signal)> sendQueue)
            {
                if (!signal)
                {
                    this.currentState = !this.currentState;
                    sendQueue.Enqueue((this, this.Targets, this.currentState));
                }
            }
        }

        private class Conjunction : Module
        {
            private Dictionary<string, bool> memory { get; set; }

            public override void Init()
            {
                memory = this.Inputs.ToDictionary(x => x.Name, x => false);
            }

            public override void Receive(Module from, bool signal, Queue<(Module from, List<Module> targets, bool signal)> sendQueue)
            {
                this.memory[from.Name] = signal;
                var sendPulse = memory.Values.Any(x => !x);
                sendQueue.Enqueue((this, this.Targets, sendPulse));
            }
        }

        private class ImportantConjunction : Conjunction
        {
            private Dictionary<string, List<long>> highSignalHits { get; set; }

            private Dictionary<string, long> patterns { get; set; }

            public override void Init()
            {
                this.highSignalHits = this.Inputs.ToDictionary(x => x.Name, x => new List<long>());
                this.patterns = this.Inputs.ToDictionary(x => x.Name, x => (long)0);
                base.Init();
            }

            public override void Receive(Module from, bool signal, Queue<(Module from, List<Module> targets, bool signal)> sendQueue)
            {
                base.Receive(from, signal, sendQueue);
                if (signal)
                {
                    var previousHits = this.highSignalHits[from.Name];
                    previousHits.Add(Problem20.Presses);
                   
                    if (previousHits.Count > 2 && (previousHits[^1] - previousHits[^2]) == (previousHits[^2] - previousHits[^3]))
                    {
                        this.patterns[from.Name] = previousHits[^1] - previousHits[^2];
                        if (this.patterns.Values.All(x => x != 0))
                        {
                            this.Finished();
                        }
                    }
                }
            }

            private void Finished()
            {
                // A bit naive to assume presses until first high is the same as every consecutive high. But it works!
                var lcm = ParsableUtils.LowestCommonMultiple(this.patterns.Values);
                Problem20.PressesUntilRx = lcm;
            }
        }

        private class Broadcaster : Module
        {
            public Broadcaster()
            {
                this.Name = "broadcaster";
            }

            public override void Receive(Module from, bool signal, Queue<(Module from, List<Module> targets, bool signal)> sendQueue)
            {
                sendQueue.Enqueue((this, this.Targets, signal));
            }
        }

        private class EmptyModule : Module
        {
            public EmptyModule(string name)
            {
                this.Name = name;
                this.TargetStrings = new List<string>();
            }

            public override void Receive(Module from, bool signal, Queue<(Module from, List<Module> targets, bool signal)> sendQueue)
            {
                // Worth a try!
                if (!signal)
                {
                    Problem20.PressesUntilRx = Problem20.Presses;
                }
            }
        }
    }
}
