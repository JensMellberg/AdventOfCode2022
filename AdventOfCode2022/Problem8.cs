using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem8 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var matrix = Matrix.FromTestInput<int>(testData);
			var visibleTrees = Matrix.InitWithStartValue(matrix.RowCount, matrix.ColumnCount, false);
			var ScenicScore = Matrix.InitWithStartValue(matrix.RowCount, matrix.ColumnCount, 0);
			for (int x = 0; x < matrix.RowCount; x++)
			{
				var highestTree = -1;
				for (int i = 0; i < matrix.ColumnCount; i++)
				{
					if (matrix[i, x] > highestTree)
					{
						visibleTrees[i, x] = true;
					}

					highestTree = Math.Max(highestTree, matrix[i, x]);
				}
			}

			for (int x = 0; x < matrix.RowCount; x++)
			{
				var highestTree = -1;
				for (int i = matrix.ColumnCount - 1; i >= 0; i--)
				{
					if (matrix[i, x] > highestTree)
					{
						visibleTrees[i, x] = true;
					}

					highestTree = Math.Max(highestTree, matrix[i, x]);
				}
			}

			for (int x = 0; x < matrix.ColumnCount; x++)
			{
				var highestTree = -1;
				for (int i = 0; i < matrix.RowCount; i++)
				{
					if (matrix[x, i] > highestTree)
					{
						visibleTrees[x, i] = true;
					}

					highestTree = Math.Max(highestTree, matrix[x, i]);
				}
			}

			for (int x = 0; x < matrix.ColumnCount; x++)
			{
				var highestTree = -1;
				for (int i = matrix.RowCount - 1; i >= 0; i--)
				{
					if (matrix[x, i] > highestTree)
					{
						visibleTrees[x, i] = true;
					}

					highestTree = Math.Max(highestTree, matrix[x, i]);
				}
			}

			this.PrintResult(visibleTrees.AllValues().Sum(x => x ? 1 : 0));
			for (int x = 0; x < matrix.ColumnCount; x++)
			{
				for (int i = matrix.RowCount - 1; i >= 0; i--)
				{
					SetScenicScore(x, i);
				}
			}

			this.PrintResult(ScenicScore.AllValues().Max());

			void SetScenicScore(int x, int y)
			{
				var height = matrix[x, y];
				var xX = x;
				var leftScore = 0;
				while (xX > 0)
				{
					xX--;
					leftScore++;
					if (matrix[xX, y] < height)
					{
						
					} else
					{
						break;
					}
				}

				var rightScore = 0;
				xX = x;
				while (xX < matrix.ColumnCount - 1)
				{
					xX++;
					rightScore++;
					if (matrix[xX, y] < height)
					{
					}
					else
					{
						break;
					}
				}

				var downScore = 0;
				var yY = y; ;
				while (yY < matrix.ColumnCount - 1)
				{
					downScore++;
					yY++;
					if (matrix[x, yY] < height)
					{
						
					}
					else
					{
						break;
					}
				}

				var upScore = 0;
				yY = y; ;
				while (yY > 0)
				{
					upScore++;
					yY--;
					if (matrix[x, yY] < height)
					{
					
					}
					else
					{
						break;
					}
				}

				ScenicScore[x, y] = upScore * downScore * leftScore * rightScore;
			}
		}
	}
}
