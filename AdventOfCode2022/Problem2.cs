using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem2 : ObjectProblem<RoundResult>
	{
		public override void Solve(IEnumerable<RoundResult> testData)
		{
			this.PrintResult(testData.Sum(x => x.PlayerScore));
			this.PrintResult(testData.Sum(x => x.SmartScore()));
		}
	}

	public class RoundResult : Parsable
	{
		protected int elfChoice;
		protected int playerChoice;

		protected static int[,] resultMatrix = { { 3, 0, 6 }, { 6, 3, 0 }, { 0, 6, 3 } };

		public override void ParseFromLine(string line)
		{
			var elfChoice = line[0];
			var playerChoice = line[2];
			this.elfChoice = (int)elfChoice - 64;
			this.playerChoice = (int)playerChoice - 87;
		}

		public int PlayerScore => this.playerChoice + resultMatrix[this.playerChoice - 1, this.elfChoice - 1];

		public int SmartScore()
		{
			var desiredScore = this.playerChoice switch
			{
				1 => 0,
				2 => 3,
				3 => 6,
				_ => throw this.Exception,
			};

			for (var i = 0; i < 3; i++)
			{
				if (resultMatrix[i, this.elfChoice - 1] == desiredScore)
				{
					this.playerChoice = i + 1;
					return this.PlayerScore;
				}
			}

			throw this.Exception;
		}
	}
}
