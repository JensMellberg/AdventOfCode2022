using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AdventOfCode2022
{
	public class TestRunner
	{
		public static void RunTests()
		{
			var runner = new TestRunner();
			var tests = typeof(TestRunner)
				.GetMethods()
				.Where(m => m.GetCustomAttributes(typeof(Test), false).Length > 0);

			foreach (var test in tests)
			{
				try
				{
					test.Invoke(runner, null);
				}
				catch (TargetInvocationException ex)
				{
					Console.WriteLine($"Error in test {test.Name}. {ex.InnerException.Message}");
				}
			}

			Console.WriteLine("Test run completed.");
        }

		[Test]
		public void BasicPattern()
		{
			var parser = new PatternParser("¤first¤   ¤second¤");
			var simple = parser.ParseObject<(int first, int second)>("55   66");
			AssertEquals((55, 66), simple);

            var complex = parser.ParseObject<(string first, string second)>("af#h:   11");
            AssertEquals(("af#h:", "11"), complex);
        }

        [Test]
        public void HandlesAlternatives()
        {
            var parser = new PatternParser("¤_¤: X[+|=]¤x¤, Y[+|=]¤y¤");
            var button = parser.ParseObject<(int x, int y)>("Button A: X+11, Y+12");
            AssertEquals((11, 12), button);

            var price = parser.ParseObject<(int x, int y)>("Prize: X=1115, Y=1215");
            AssertEquals((1115, 1215), price);
        }

        [Test]
        public void HandlesAlternativesOfDifferentLength()
        {
            var parser = new PatternParser("[public|private] int ¤Name¤");
            var name1 = parser.ParseObject<NameContainer>("public int Hej");
            AssertEquals("Hej", name1.Name);

            var name2 = parser.ParseObject<NameContainer>("private int Secret");
            AssertEquals("Secret", name2.Name);

            var parser2 = new PatternParser("[Button A|Button B|Prize]: X[+|=]¤x¤, Y[+|=]¤y¤");
            var button = parser2.ParseObject<(int x, int y)>("Button A: X+11, Y+12");
            AssertEquals((11, 12), button);
        }

		[Test]
		public void CanParseParsable()
		{
			var parser = new PatternParser("¤discard¤: ¤List¤");
			var line = "Program: 5,4,3";
			var result = parser.ParseObject<(string discard, ListParsable<int> List)>(line);
			AssertEquals("5,4,3", result.List.originalLine);
		}

        private void AssertEquals(object expected, object actual)
		{
			if (!expected.Equals(actual))
			{
				throw new TestException($"Expected value to be {expected} but was {actual}.");
			}
		}

        private class Test : Attribute
        {
        }

        private class TestException : Exception
        {
			public TestException(string message) : base(message) { }
        }

		private class NameContainer
		{
			public string Name { get; set; }
		}
    }
}
