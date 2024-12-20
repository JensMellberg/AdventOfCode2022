using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem14 : PatternProblem<Robot>
    {
        protected override string Pattern => "p=¤X¤,¤Y¤ v=¤VelocityX¤,¤VelocityY¤";

        public override void Solve(IEnumerable<Robot> testInput)
        {
            var inputList = testInput.ToList();
            var part1 = this.QuadrantScore(testInput.Select(x => x.PositionAfterXSeconds(100)));
            var part2 = this.StepThroughPart2(inputList);
            this.PrintResult(part1);
            this.PrintResult(part2);
        }

        public long StepThroughPart2(IList<Robot> testInput)
        {
            for (var i = 1; i < int.MaxValue; i++)
            {
                var positions = testInput.Select(x => x.PositionAfterXSeconds(i)).ToList();
                if (positions.Distinct().Count() > positions.Count - 2)
                {
                    if (!this.supressPrints)
                    {
                        var fieldMatrix = Matrix.InitWithStartValue(Robot.AreaHeight, Robot.AreaWidth, '.');
                        foreach (var position in positions)
                        {
                            fieldMatrix[(int)position.x, (int)position.y] = 'X';
                        }

                        this.Print(fieldMatrix.ToString(c => c.ToString(), ""));
                    }
                    
                    return i;
                }
            }

            return -1;
        }

        private long QuadrantScore(IEnumerable<(long x, long y)> positions)
        {
            var quad1 = 0;
            var quad2 = 0;
            var quad3 = 0;
            var quad4 = 0;
            foreach (var pos in positions)
            {
                if (pos.x < Robot.AreaWidth / 2 && pos.y < Robot.AreaHeight / 2)
                {
                    quad1++;
                }
                else if (pos.x > Robot.AreaWidth / 2 && pos.y < Robot.AreaHeight / 2)
                {
                    quad2++;
                }
                else if (pos.x < Robot.AreaWidth / 2 && pos.y > Robot.AreaHeight / 2)
                {
                    quad3++;
                }
                else if (pos.x > Robot.AreaWidth / 2 && pos.y > Robot.AreaHeight / 2)
                {
                    quad4++;
                }
            }

            return quad1 * quad2 * quad3 * quad4;
        }
    }

    public class Robot
    {
        public const int AreaWidth = 101;

        public const int AreaHeight = 103;

        public int X { get; set; }

        public int Y { get; set; }

        public int VelocityX { get; set; }

        public int VelocityY { get; set; }

        public (long x, long y) PositionAfterXSeconds(long seconds)
        {
            return ((this.X + VelocityX * seconds + AreaWidth * seconds) % AreaWidth, (this.Y + VelocityY * seconds + AreaHeight + AreaHeight * seconds) % AreaHeight);
        }
    }
}
