using System;
using System.Diagnostics;

namespace AdventOfCode2022
{
	class Program
	{
		static void Main(string[] args)
		{
            while (true)
			{
                var command = Console.ReadLine();
                if (string.IsNullOrEmpty(command))
                {
                    SolveLastProblem();
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
            var problem = (Problem)Activator.CreateInstance(type);
            var input = TestDataReader.GetTestData(problemNumber);
            problem.Solve(input);
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
