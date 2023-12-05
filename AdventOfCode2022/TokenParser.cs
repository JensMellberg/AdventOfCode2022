using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AdventOfCode2022
{
	public class TokenParser
	{
		private string[] tokens;

		private int pointer = 0;
		public TokenParser(string line, char splitChar)
		{
			this.tokens = line.Split(splitChar, StringSplitOptions.RemoveEmptyEntries);
		}

		public TokenParser(string line) : this(line, ' ')
		{
		}

		public TokenParser(string[] tokens)
		{
			this.tokens=tokens;
		}

		public bool IsFinished => this.pointer >= this.tokens.Length;

		public void Skip()
		{
			this.Skip(1);
		}

		public void Skip(int steps)
		{
			this.pointer += steps;
		}

		public string Pop()
		{
            this.pointer++;
            return this.tokens[this.pointer - 1];
		}

		public string Peek() => this.tokens[this.pointer];

		public string[] PopUntil(char endChar) => this.PopUntil(endChar.ToString());

        public string[] PopUntil(string endString)
		{
			IList<string> result = new List<string>();
			while (this.pointer < this.tokens.Length && !this.tokens[this.pointer].Contains(endString, StringComparison.InvariantCultureIgnoreCase))
			{
                result.Add(this.Pop());
            }

			if (this.pointer < this.tokens.Length)
			{
				if (this.Peek().Length > endString.Length)
				{
					var index = this.Peek().IndexOf(endString, StringComparison.InvariantCultureIgnoreCase);
					var beforeToken = this.Peek()[..index];
					var afterToken = index < endString.Length - 1
						? this.Peek()[(index + 1)..]
						: string.Empty;
					if (beforeToken == string.Empty)
					{
						this.tokens[this.pointer] = afterToken;
					}
					else if (afterToken == string.Empty)
					{
						result.Add(beforeToken);
						this.pointer++;
                    }
					else
					{
                        result.Add(beforeToken);
                        this.tokens[this.pointer] = afterToken;
                    }
                }
				else
				{
					this.Pop();
				}
			}

			return result.ToArray();
		}

		public string ReadUntil(string endString) => string.Join(' ', this.PopUntil(endString));

		public string ReadUntil(char endChar) => this.ReadUntil(endChar.ToString());
    }
}
