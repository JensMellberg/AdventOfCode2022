using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode2022.TwentyThree
{
    public class Problem13 : StringProblem
    {
        protected override TabBehavior TabBehavior => TabBehavior.Reject;
        protected override EmptyStringBehavior EmptyStringBehavior => EmptyStringBehavior.Keep;
        
        public override void Solve(IEnumerable<string> testData)
        {
            var currentList = new List<string>();
            var matrices = new List<Matrix<char>>();
            foreach (var data in testData)
            {
                if (string.IsNullOrEmpty(data))
                {
                    matrices.Add(Matrix.FromTestInput<char>(currentList));
                    currentList = new List<string>();
                }
                else
                {
                    currentList.Add(data);
                }
            }

            var resultByIndex = new Dictionary<int, (long score, int index, string type)>();
            long sum = 0;
            for (var i = 0; i < matrices.Count; i++)
            {
                var score = PatternScore(matrices[i]);
                resultByIndex.Add(i, score);
                sum += score.score;
            }

            this.PrintResult(sum);
            sum = 0;
            for (var i = 0; i < matrices.Count; i++)
            {
                var entry = resultByIndex[i];
                var score = PatternScore2(matrices[i], entry.index, entry.type);
                sum += score;
            }

            this.PrintResult(sum);
        }

        private (long score, int index, string type) PatternScore(Matrix<char> matrix)
        {
            var cols = matrix.GetColumns().ToArray();
            var rows = matrix.GetRows().ToArray();
            var rowScore = FindMirrorIndex(rows);
            if (rowScore > 0)
            {
                return (rowScore * 100, rowScore - 1, "row");
            }

            var colScore = FindMirrorIndex(cols);
            return (colScore, colScore - 1, "col");

            int FindMirrorIndex(IEnumerable<char>[] entries)
            {
                for (var i = 0; i < entries.Length - 1; i++)
                {
                    var first = entries[i].ToArray();
                    var second = entries[i + 1].ToArray();
                    var frstI = i;
                    var scndI = i + 1;
                    while (EqualDiff(first, second) == 0)
                    {
                        frstI--;
                        scndI++;
                        if (frstI < 0 || scndI >= entries.Length)
                        {
                            return i + 1;
                        }

                        first = entries[frstI].ToArray();
                        second = entries[scndI].ToArray();
                    }
                }

                return 0;
            }

            int EqualDiff(char[] a, char[] b)
            {
                var oneDiff = false;
                for (var pos = 0; pos < a.Length; pos++)
                {
                    if (a[pos] != b[pos])
                    {
                        if (oneDiff)
                        {
                            return 2;
                        }
                        else
                        {
                            oneDiff = true;
                        }
                    }
                }

                return oneDiff ? 1 : 0;
            }
        }

        private long PatternScore2(Matrix<char> matrix, int previousIndex, string previousType)
        {
            var cols = matrix.GetColumns().ToArray();
            var rows = matrix.GetRows().ToArray();
            var rowScore = FindMirrorIndex(rows, previousType == "row" ? previousIndex : -1) * 100;
            return rowScore > 0 ? rowScore : FindMirrorIndex(cols, previousType == "col" ? previousIndex : -1);

            int FindMirrorIndex(IEnumerable<char>[] entries, int skipIndex)
            {
                for (var i = 0; i < entries.Length - 1; i++)
                {
                    if (i == skipIndex)
                    {
                        continue;
                    }

                    var hasSmudged = false;
                    var first = entries[i].ToArray();
                    var second = entries[i + 1].ToArray();
                    var frstI = i;
                    var scndI = i + 1;
                    var diff = EqualDiff(first, second);
                    while (diff < 2)
                    {
                        if (diff == 1 && hasSmudged)
                        {
                            break;
                        } 
                        else if (diff == 1)
                        {
                            hasSmudged = true;
                        }

                        frstI--;
                        scndI++;
                        if (frstI < 0 || scndI >= entries.Length)
                        {
                            return i + 1;
                        }

                        first = entries[frstI].ToArray();
                        second = entries[scndI].ToArray();
                        diff = EqualDiff(first, second);
                    }
                }

                return 0;
            }

            int EqualDiff(char[] a, char[] b)
            {
                var oneDiff = false;
                for (var pos = 0; pos < a.Length; pos++)
                {
                    if (a[pos] != b[pos])
                    {
                        if (oneDiff)
                        {
                            return 2;
                        }
                        else
                        {
                            oneDiff = true;
                        }
                    }
                }

                return oneDiff ? 1 : 0;
            }
        }
    }
}
