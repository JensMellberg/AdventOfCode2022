using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AdventOfCode2022
{
	public class PatternParser
	{
		// Maybe implement in future.
		private List<Token> tokens = new List<Token>();
		private const char Separator = '¤';
		public PatternParser(string pattern)
		{
			var index = 0;
			while (index < pattern.Length)
			{
				if (pattern[index] == Separator)
				{
					index++;
					string value = "";
					while (pattern[index] != Separator)
					{
						value += pattern[index];
						index++;
					}

					tokens.Add(new Variable { Value = value });
					index++;
				}
				else
				{
                    string value = "";
                    while (pattern[index] != Separator && index < pattern.Length)
                    {
                        value += pattern[index];
                        index++;
                    }

					tokens.Add(new Constant { Value = value });
                }
			}
		}

		public T ParseObject<T>(string line)
		{
			var currentIndex = 0;
			var current = this.tokens[0];
			Token nextConstant = null;
			var stringIndex = 0;
			while (currentIndex < this.tokens.Count)
			{
				if (current is Variable)
				{
					if (currentIndex < this.tokens.Count - 1)
					{
						nextConstant = this.tokens[currentIndex + 1];
					} 
					else
					{
                        // Create object
                        continue;
                    }

					var value = ""; //TODO
				}
			}

			return default(T);
		}

		private class Variable : Token
		{
			public string Value { get; set; }
		}

		private class Constant : Token
		{
            public string Value { get; set; }
        }

		private interface Token
		{
            string Value { get; set; }
        }
    }
}
