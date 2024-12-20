using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem3 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;

        public override void Solve(IEnumerable<string> testData)
        {
            var matrix = Matrix.FromTestInput<char>(testData);
            var isPart = Matrix.InitWithStartValue(matrix.RowCount, matrix.ColumnCount, false);
            var gears = Matrix.InitWithStartValue(matrix.RowCount, matrix.ColumnCount, 0);
            var currentGearId = 1;

            for (var x = 0; x < matrix.ColumnCount; x++)
            {
                for (var y = 0; y < matrix.RowCount; y++)
                {
                    var value = matrix[x, y];
                    if (ParsableUtils.IsNumber(value) || value == '.')
                    {
                        continue;
                    }

                    var isGear = value == '*';
                    var gearId = isGear ? currentGearId++ : 0;
                    if (isGear)
                    {
                        gears[x, y] = -gearId;
                    }
                    foreach ((int aX, int aY) in matrix.GetAdjacentCoordinatesDiagonally(x, y))
                    {
                        isPart[aX, aY] = true;
                        gears[aX, aY] = gearId;
                    }
                }
            }

            IList<int> numbers = new List<int>();
            IDictionary<int, int> CountByGearId = new Dictionary<int, int>();
            IDictionary<int, int> NumbersByGearId = new Dictionary<int, int>();
            for (var y = 0; y < matrix.RowCount; y++)
            {
                string number = string.Empty;
                var currentIsPart = false;
                var gearId = 0;
                for (var x = 0; x < matrix.ColumnCount; x++)
                {
                    var value = matrix[x, y];
                    if (ParsableUtils.IsNumber(value))
                    {
                        number += value;
                        currentIsPart |= isPart[x, y];
                        gearId = Math.Max(gearId, gears[x, y]);
                    } 
                    else
                    {
                        if (!string.IsNullOrEmpty(number) && currentIsPart) {
                            numbers.Add(int.Parse(number));
                            if (gearId != 0)
                            {
                                if (CountByGearId.ContainsKey(gearId))
                                {
                                    CountByGearId[gearId]++;
                                }
                                else
                                {
                                    CountByGearId.Add(gearId, 1);
                                }

                                if (NumbersByGearId.ContainsKey(gearId))
                                {
                                    NumbersByGearId[gearId] *= int.Parse(number);
                                }
                                else
                                {
                                    NumbersByGearId.Add(gearId, int.Parse(number));
                                }
                            }
                        }

                        number = string.Empty;
                        currentIsPart = false;
                        gearId = 0;
                    }
                }

                if (!string.IsNullOrEmpty(number) && currentIsPart)
                {
                    numbers.Add(int.Parse(number));
                    if (gearId != 0)
                    {
                        if (CountByGearId.ContainsKey(gearId))
                        {
                            CountByGearId[gearId]++;
                        }
                        else
                        {
                            CountByGearId.Add(gearId, 1);
                        }

                        if (NumbersByGearId.ContainsKey(gearId))
                        {
                            NumbersByGearId[gearId] *= int.Parse(number);
                        }
                        else
                        {
                            NumbersByGearId.Add(gearId, int.Parse(number));
                        }
                    }
                }
            }

            this.PrintResult(numbers.Sum());
            this.PrintResult(CountByGearId.Keys.Where(x => CountByGearId[x] == 2).Sum(x => NumbersByGearId[x]));
        }
     }
}
