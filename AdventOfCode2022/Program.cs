using System;
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
                else
				{
                    if (!SolveProblem(command))
					{
                        Console.WriteLine($"Could not find problem: {command}");
					}
				}
            }
		}

        static bool SolveProblem(string problemNumber)
        {
            var classString = "AdventOfCode2022.Problem" + problemNumber;
            var type = Type.GetType(classString);
            if (type == null)
            {
                return false;
            }

            Console.WriteLine("Running solution for problem " + problemNumber);
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            var solve = type.GetMethod("Solve");
            var parseData = type.GetMethod("ParseData");

            var instance = Activator.CreateInstance(type);
            var input = parseData.Invoke(instance, new object[] { TestDataReader.GetTestData(problemNumber) });
            solve.Invoke(instance, new object[] { input });
            stopWatch.Stop();
            Console.WriteLine($"total time elapsed: {stopWatch.Elapsed}");
            return true;
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
