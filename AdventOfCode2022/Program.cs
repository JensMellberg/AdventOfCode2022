using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AdventOfCode2022
{
	class Program
	{
        public static string CurrentYear => CurrentYearNumber switch
        {
            "2022" => "TwentyTwo",
            "2023" => "TwentyThree",
            "2024" => "TwentyFour",
            _ => null,
        };

        public static string CurrentYearNumber = "2024";

        static void Main(string[] args)
		{
            const string WelcomeText = "Type the specific day you want to solve the problem for or an empty line for the latest problem. Or \"RunAll\"!";
            Console.WriteLine(WelcomeText);
            while (true)
			{
                var command = Console.ReadLine();
                var tokens = command.Split(" ");
                if (string.IsNullOrEmpty(command))
                {
                    SolveLastProblem();
                } 
                else if (command.Equals("RunAll", StringComparison.OrdinalIgnoreCase))
				{
                    BenchMark();
				}
                else if (tokens[0].Equals("-year", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Year set to {tokens[1]}");
                    CurrentYearNumber = tokens[1];
                }
                else if (tokens[0].Equals("-folder", StringComparison.OrdinalIgnoreCase))
                {
                    Process.Start("explorer.exe", Environment.CurrentDirectory + "\\" + CurrentYearNumber);
                }
                else if (tokens[0].Equals("-gettest", StringComparison.OrdinalIgnoreCase))
                {
                    var day = tokens[1];
                    TestDataReader.RetrieveTestData(day);
                    Console.Clear();
                    Console.WriteLine(WelcomeText);
                }
                else
				{
                    int? testIndex = null;
                    if (tokens.Length > 1 && tokens[1].Equals("-test", StringComparison.OrdinalIgnoreCase))
                    {
                        testIndex = tokens.Length > 2 ? int.Parse(tokens[2]) : 0;
                    }

                    if (!SolveProblem(tokens[0], testIndex))
					{
                        Console.WriteLine($"Could not find problem: {command}");
					}
				}
            }
		}

        static void BenchMark()
		{
            for (var i = 1; i < 26; i++)
			{
                if (!GetProblemClass(i.ToString(), out var type, out var instance))
				{
                    continue;
				}

                var method = type.GetMethod("SetBenchmarkRun");
                var resultMethod = type.GetMethod("GetResults");
               
                method.Invoke(instance, null);
                var elapsed = RunProblem(instance, type, i.ToString(), null);
                List<string> results = resultMethod.Invoke(instance, null) as List<string>;
                var firstResult = results.Count > 0 ? results[0] : "";
                var secondResult = results.Count > 1 ? results[1] : "";
                Console.WriteLine($"Problem {i}: Part 1: {firstResult} | Part2: {secondResult} | Time: {elapsed}");
            }
		}

        static bool GetProblemClass(string problemNumber, out Type problem, out object instance)
		{
            var classString = $"AdventOfCode2022.{CurrentYear}.Problem" + problemNumber;
            problem = Type.GetType(classString);
            instance = problem == null ? null : Activator.CreateInstance(problem);
            return problem != null;
        }

        static bool SolveProblem(string problemNumber, int? testIndex = null)
        {
            if (!GetProblemClass(problemNumber, out var type, out var instance))
			{
                return false;
			} 

            Console.WriteLine("Running solution for problem " + problemNumber);
            var elapsed = RunProblem(instance, type, problemNumber, testIndex);
            Console.WriteLine($"total time elapsed: {elapsed}");
            return true;
        }

        static TimeSpan RunProblem(object instance, Type type, string problemNumber, int? testIndex)
        {
            return RunProblem(instance, type, TestDataReader.GetTestData(problemNumber, testIndex));
        }

        static TimeSpan RunProblem(object instance, Type type, string testData)
		{
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var solve = type.GetMethod("Solve");
            var parseData = type.GetMethod("ParseData");
            var input = parseData.Invoke(instance, new object[] { testData });
            solve.Invoke(instance, new object[] { input });
            stopWatch.Stop();
            return stopWatch.Elapsed;
        }

        static void SolveLastProblem()
        {
            var currentGuess = 25;
            while (!SolveProblem(currentGuess.ToString()))
            {
                currentGuess--;
            }
        }
    }
}
