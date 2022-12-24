using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode2022
{
	class Program
	{
		static void Main(string[] args)
		{
            Console.WriteLine("Type the specific day you want to solve the problem for or an empty line for the latest problem.");
            while (true)
			{
                var command = Console.ReadLine();
                if (string.IsNullOrEmpty(command))
                {
                    SolveLastProblem();
                } 
                else if (command.Equals("RunAll", StringComparison.OrdinalIgnoreCase))
				{
                    BenchMark();
				}
                else
				{
                    var tokens = command.Split(" ");

                    if (!SolveProblem(tokens[0], tokens.Length > 1 && tokens[1].Equals("-test", StringComparison.OrdinalIgnoreCase)))
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
                var elapsed = RunProblem(instance, type, i.ToString(), false);
                List<string> results = resultMethod.Invoke(instance, null) as List<string>;
                var firstResult = results.Count > 0 ? results[0] : "";
                var secondResult = results.Count > 1 ? results[1] : "";
                Console.WriteLine($"Problem {i}: Part 1: {firstResult} | Part2: {secondResult} | Time: {elapsed}");
            }
		}

        static bool GetProblemClass(string problemNumber, out Type problem, out object instance)
		{
            var classString = "AdventOfCode2022.Problem" + problemNumber;
            problem = Type.GetType(classString);
            instance = problem == null ? null : Activator.CreateInstance(problem);
            return problem != null;
        }

        static bool SolveProblem(string problemNumber, bool isTest = false)
        {
            if (!GetProblemClass(problemNumber, out var type, out var instance))
			{
                return false;
			} 

            Console.WriteLine("Running solution for problem " + problemNumber);
            var elapsed = RunProblem(instance, type, problemNumber, isTest);
            Console.WriteLine($"total time elapsed: {elapsed}");
            return true;
        }

        static TimeSpan RunProblem(object instance, Type type, string problemNumber, bool isTest)
        {
            return RunProblem(instance, type, TestDataReader.GetTestData(problemNumber, isTest));
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
