using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;

namespace AdventOfCode2022.TwentyFour
{
    public class Problem21 : StringProblem
    {
        public static Dictionary<char, (int x, int y)> keyPadDict = new Dictionary<char, (int x, int y)>()
        {
            {'^', (1, 0) },
            {'A', (2, 0) },
            {'<', (0, 1) },
            {'v', (1, 1) },
            {'>', (2, 1) }
        };

        public static Dictionary<char, (int x, int y)> numberPadDict = new Dictionary<char, (int x, int y)>()
        {
            {'7', (0, 0) },
            {'8', (1, 0) },
            {'9', (2, 0) },
            {'4', (0, 1) },
            {'5', (1, 1) },
            {'6', (2, 1) },
            {'1', (0, 2) },
            {'2', (1, 2) },
            {'3', (2, 2) },
            {'0', (1, 3) },
            {'A', (2, 3) },
        };

        public override void Solve(IEnumerable<string> testInput)
        {

            var numberPad = this.CreateNumberPad(2);
            var numberPad2 = this.CreateNumberPad(2);

            long result = 0;
            long result2 = 0;
            foreach (var code in testInput)
            {
               // var presses1 = numberPad.GetPresses(code);
                var presses2 = numberPad2.GetPresses(code);
                var numericCode = int.Parse(code[..3]);
               // result += presses1.Length * numericCode;
                result2 += presses2.Length * numericCode;
                numberPad.Reset();
                numberPad2.Reset();
            }

            this.PrintResult(result);
            this.PrintResult(result2);
        }

        private KeyPad CreateNumberPad(int robotCount)
        {
            KeyPad previousRobot = null;
            for (var i = 0; i < robotCount; i++)
            {
                var robot = new KeyPad(keyPadDict, previousRobot);
                robot.Root = i == 0;
                previousRobot = robot;
            }

            return new KeyPad(numberPadDict, previousRobot);
        }

        private class KeyPad
        {
            private Dictionary<char, (int x, int y)> keypad;

            public bool Root { get; set; }

            private HashSet<(int x, int y)> allowedPoses;

            private KeyPad wrapped;

            private (int x, int y) pointerPos;

            public KeyPad(Dictionary<char, (int x, int y)> keypad, KeyPad wrapped)
            {
                this.keypad = keypad;
                this.wrapped = wrapped;
                this.allowedPoses = keypad.Values.ToHashSet();
                this.pointerPos = keypad['A'];
            }

            public string GetPresses(string result)
            {
                var presses = "";
                if (this.Root)
                {
                    var a = 5;
                }

                foreach (var c in result)
                {
                    presses += PressesToPoint(this.keypad[c]);
                }

                if (this.wrapped != null)
                {
                    return this.wrapped.GetPresses(presses);
                }
                else
                {
                    return presses;
                }

                string PressesToPoint((int x, int y) pos)
                {
                    var (deltaX, deltaY) = (pos.x - pointerPos.x, pos.y - pointerPos.y);
                    var horizontalDirs = Enumerable.Repeat(deltaX > 0 ? Direction.Right : Direction.Left, Math.Abs(deltaX)).ToList();
                    var verticalDirs = Enumerable.Repeat(deltaY > 0 ? Direction.Down : Direction.Up, Math.Abs(deltaY)).ToList();
                    var resultDirs = new List<Direction>();
                    var preferVert = true;

                    while (horizontalDirs.Any() || verticalDirs.Any())
                    {
                        if (horizontalDirs.Any() && verticalDirs.Any())
                        {
                            var furthestPoint = keyPadDict['<'];
                            var vertPoint = keyPadDict[DirectionToChar(verticalDirs[0])];
                            var horizPoint = keyPadDict[DirectionToChar(horizontalDirs[0])];
                            preferVert = Math.Abs(vertPoint.x - furthestPoint.x) + Math.Abs(vertPoint.y - furthestPoint.y)
                                < Math.Abs(horizPoint.x - furthestPoint.x) + Math.Abs(horizPoint.y - furthestPoint.y);
                        }

                        if (preferVert)
                        {
                            AddVert();
                        }

                        if (horizontalDirs.Any())
                        {
                            var horizontalDelta = horizontalDirs[0].GetDelta();
                            if (allowedPoses.Contains((pointerPos.x + horizontalDelta.x * horizontalDirs.Count, pointerPos.y)))
                            {
                                pointerPos.x += horizontalDelta.x * horizontalDirs.Count;
                                resultDirs.AddRange(horizontalDirs);
                                horizontalDirs.Clear();
                            }
                        }

                        if (!preferVert)
                        {
                            AddVert();
                        }

                        void AddVert()
                        {
                            if (verticalDirs.Any())
                            {
                                var verticalDelta = verticalDirs[0].GetDelta();
                                if (allowedPoses.Contains((pointerPos.x, pointerPos.y + verticalDelta.y * verticalDirs.Count)))
                                {
                                    pointerPos.y += verticalDelta.y * verticalDirs.Count;
                                    resultDirs.AddRange(verticalDirs);
                                    verticalDirs.Clear();
                                }
                            }
                        }
                    }

                    return string.Join("", resultDirs.Select(x => DirectionToChar(x)).Concat(new[] { 'A' }));
                }
            }

            public void Reset()
            {
                this.pointerPos = keypad['A'];
                this.wrapped?.Reset();
            }

            private static char DirectionToChar(Direction d) => d switch
            {
                Direction.Left => '<',
                Direction.Right => '>',
                Direction.Down => 'v',
                Direction.Up => '^',
                _ => throw new Exception(),
            };
        }
    }
}
