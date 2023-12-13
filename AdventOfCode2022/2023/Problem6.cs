using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem6 : StringProblem
    {
        public override void Solve(IEnumerable<string> testData)
        {
            var lines = testData.ToArray();
            var timeParser = new TokenParser(lines.First());
            var distanceparser = new TokenParser(lines[1]);
            timeParser.Skip();
            distanceparser.Skip();
            var races = new List<Race>();
            while (!timeParser.IsFinished)
            {
                var time = int.Parse(timeParser.Pop());
                var distance = int.Parse(distanceparser.Pop());
                races.Add(new Race
                {
                    Time = time,
                    Distance = distance
                });
            }

            this.Calculate(races);
            var concatedTime = string.Join("", races.Select(x => x.Time.ToString()));
            var concatedDist = string.Join("", races.Select(x => x.Distance.ToString()));
            var singleRace = new Race
            {
                Time = long.Parse(concatedTime),
                Distance = long.Parse(concatedDist)
            };

            this.Calculate(new List<Race>() { singleRace });
        }

        private void Calculate(List<Race> races)
        {
            long result = 1;
            foreach (var race in races)
            {
                long firstWin = 0;
                long lastWin = 0;
                for (long i = 0; i < race.Time; i++)
                {
                    if (race.GetDistance(i) > race.Distance)
                    {
                        firstWin = i;
                        break;
                    }
                }

                for (long i = race.Time - 1; i >= 0; i--)
                {
                    if (race.GetDistance(i) > race.Distance)
                    {
                        lastWin = i;
                        break;
                    }
                }

                result *= (lastWin - firstWin + 1);
            }

            this.PrintResult(result);
        }

        private class Race
        {
            public long Time { get; set; }

            public long Distance { get; set; }

            public long GetDistance(long buttonHold)
            {
                var speed = buttonHold;
                var remainingTime = Time - buttonHold;
                return remainingTime * speed;
            }
        }
     }
}
