using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem5 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;
        public override void Solve(IEnumerable<string> testData)
        {
            var arrayData = testData.ToArray();
            var dataParser = new TokenParser(arrayData);
            var seedsParser = new TokenParser(dataParser.Pop());
            seedsParser.Skip();
            var seeds = seedsParser.PopUntil(';').Select(x => new SeedRange { Start = long.Parse(x), Length = 1 }).ToList();
            this.Calculate(seeds, dataParser);

            var secondDataParser = new TokenParser(arrayData);
            var seedsParser2 = new TokenParser(secondDataParser.Pop());
            seedsParser2.Skip();
            var seeds2 = new List<SeedRange>();
            while (!seedsParser2.IsFinished)
            {
                var first = long.Parse(seedsParser2.Pop());
                var length = long.Parse(seedsParser2.Pop());
                seeds2.Add(new SeedRange { Start = first, Length = length });
            }

            this.Calculate(seeds2, secondDataParser);
        }

        private void Calculate(List<SeedRange> seeds, TokenParser dataParser)
        {
            var maps = new SeedsMap[7];
            for (var i = 0; i < 7; i++)
            {
                maps[i] = new SeedsMap();
            }

            var pointer = -1;
            while (!dataParser.IsFinished)
            {
                var line = dataParser.Pop();
                if (!ParsableUtils.IsNumber(line[0]))
                {
                    pointer++;
                }
                else
                {
                    var mapParser = new TokenParser(line);
                    var range = new MapRange();
                    range.TargetStart = long.Parse(mapParser.Pop());
                    range.SourceStart = long.Parse(mapParser.Pop());
                    range.Length = long.Parse(mapParser.Pop());
                    maps[pointer].Ranges.Add(range);
                }
            }

            long lowest = long.MaxValue;
            foreach (var seed in seeds)
            {
                var value = new List<SeedRange>
                {
                    seed
                };

                foreach (var map in maps)
                {
                    var previousValues = new List<SeedRange>(value);
                    value.Clear();
                    foreach (var s in previousValues)
                    {
                        value.AddRange(map.MapNumber(s));
                    }
                    
                }

                lowest = Math.Min(lowest, value.Min(x => x.Start));
            }

            this.PrintResult(lowest);
        }

        private class SeedRange
        {
            public long Start { get; set; }

            public long Length { get; set; }
        }

        private class SeedsMap
        {
            public IList<MapRange> Ranges { get; set; } = new List<MapRange>();

            public IList<SeedRange> MapNumber(SeedRange number)
            {
                var resultRanges = new List<SeedRange>();
                var unfinishedRanges = new List<SeedRange>() { number };
                foreach (var range in this.Ranges)
                {
                    var current = new List<SeedRange>(unfinishedRanges);
                    unfinishedRanges.Clear();
                    foreach (var xx in current)
                    {
                        range.MapNumber(xx, unfinishedRanges, resultRanges);
                    }
                }

                resultRanges.AddRange(unfinishedRanges);
                return resultRanges;
            }
        }

        private class MapRange
        {
            public long SourceStart { get; set; }

            public long TargetStart { get; set; }

            public long Length { get; set; }

            public void MapNumber(SeedRange number, List<SeedRange> unfinishedRanges, List<SeedRange> resultRanges)
            {
                var ranges = new List<SeedRange>();
                var end = number.Start + number.Length;
                var thisEnd = this.SourceStart + this.Length;
                //Starts before
                if (number.Start < this.SourceStart)
                {
                    //Ends before
                    if (end < this.SourceStart)
                    {
                        unfinishedRanges.Add(number);
                        return;
                    }
                    else
                    {
                        //Add before range
                        unfinishedRanges.Add(new SeedRange { Start = number.Start, Length = this.SourceStart - number.Start });
                    }
                    

                    //Ends after
                    if (end > thisEnd)
                    {
                        //Add full range
                        resultRanges.Add(new SeedRange { Start = this.Map(this.SourceStart), Length = this.Length });
                        //Add after range
                        unfinishedRanges.Add(new SeedRange { Start = thisEnd, Length = end - thisEnd });
                    }
                    else if (end > this.SourceStart)
                    {
                        //Add intersection
                        resultRanges.Add(new SeedRange { Start = this.Map(this.SourceStart), Length = end - this.SourceStart - 1 });
                    }
                }
                
                //Starts in middle
                if (number.Start >= this.SourceStart && number.Start < this.SourceStart + this.Length)
                {
                    //Ends after
                    if (end > thisEnd)
                    {
                        resultRanges.Add(new SeedRange { Start = this.Map(number.Start), Length = this.SourceStart + this.Length - number.Start });
                        //Add after range
                        unfinishedRanges.Add(new SeedRange { Start = thisEnd, Length = end - thisEnd });
                    }
                    else
                    {
                        //Fully contained
                        resultRanges.Add(new SeedRange { Start = this.Map(number.Start), Length = number.Length });
                    }
                }

                if (number.Start > thisEnd)
                {
                    unfinishedRanges.Add(number);
                }

                if (ranges.Any(x => x.Start == 0))
                {
                    var a = 5;
                }
            }

            private long Map(long number)
            {
                return number - this.SourceStart + this.TargetStart;
            }
        }
    }
}
