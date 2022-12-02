using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem2 : Problem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var results = testData.Where(x => !string.IsNullOrEmpty(x)).Select(x => new RoundResult(x));
			this.PrintResult(results.Sum(x => x.PlayerScore));
		}

		private class RoundResult
		{
			private int elfChoice;
			private int playerChoice;

			public RoundResult(string line)
			{
				var elfChoice = line[0];
				var playerChoice = line[2];
				this.elfChoice = (int)elfChoice - 64;
				this.playerChoice = (int)playerChoice - 87;
			}

			public int PlayerScore => this.playerChoice + Math.Max(0, this.Score());

			public int Score()
			{
				if (this.elfChoice == this.playerChoice)
				{
					return 3;
				}

				int lowestChoice = Math.Min(this.elfChoice, this.playerChoice);
				int highestChoice = Math.Max(this.elfChoice, this.playerChoice);
				int multiplier = this.elfChoice < this.playerChoice ? -1 : 1;
				if (lowestChoice == 1)
				{
					return (highestChoice == 2 ? 0 : 6) * multiplier;
				}

				return 6 * multiplier;
			}
		}
	}

}
