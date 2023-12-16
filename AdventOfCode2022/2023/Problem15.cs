using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem15 : StringProblem
    {
        public override void Solve(IEnumerable<string> testData)
        {
            var sequence = testData.Single().Split(',');
            this.PrintResult(sequence.Select(x => this.Hash(x)).Sum());
            var boxes = new List<Lens>[256];
            for (var i = 0; i < 256; i++)
            {
                boxes[i] = new List<Lens>();
            }

            foreach (var step in sequence)
            {
                var tokens = step.Split("=");
                if (tokens.Length < 2)
                {
                    var label = step[..^1];
                    var box = Hash(label);
                    var index = BoxLabelIndex(box, label);
                    if (index != -1)
                    {
                        boxes[box].RemoveAt(index);
                    }
                } 
                else
                {
                    var focalLength = int.Parse(tokens[1]);
                    var label = tokens[0];
                    var box = Hash(label);
                    var index = BoxLabelIndex(box, label);
                    if (index != -1)
                    {
                        boxes[box][index].FocalLength = focalLength;
                    }
                    else
                    {
                        boxes[box].Add(new Lens { FocalLength = focalLength, Label = label });
                    }
                }
            }

            var total = 0;
            for (var i = 0; i < 256; i++)
            {
                for (var x = 0; x < boxes[i].Count; x++)
                {
                    total += (i + 1) * (x + 1) * boxes[i][x].FocalLength;
                }
            }

            this.PrintResult(total);

            int BoxLabelIndex(int box, string label)
            {
                return boxes[box].FindIndex(x =>  x.Label == label);
            }
        }

        private int Hash(string s)
        {
            var total = 0;
            foreach (var c in s)
            {
                total = (total + c) * 17 % 256;
            }

            return total;
        }

        private class Lens
        {
            public string Label { get; set; }

            public int FocalLength { get; set; }
        }
    }
}
