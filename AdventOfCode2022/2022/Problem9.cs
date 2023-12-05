using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem9 : ObjectProblem<Instruction>
	{
		public override void Solve(IEnumerable<Instruction> testData)
		{
			this.SolveOnce(testData, 2);
			this.SolveOnce(testData, 10);
		}

		private void SolveOnce(IEnumerable<Instruction> testData, int nodes)
		{
			var visited = Matrix.InitWithStartValue(400, 400, false);
			var snake = new Snake((x, y) => visited[x, y] = true, nodes);
			foreach (var x in testData)
			{
				x.Perform(snake);
				//this.Print(visited.ToString(x => x ? "#" : ".", ""));
			}
			//testData.ForEach(x => x.Perform(snake));
			//this.Print(visited.ToString(x => x ? "#" : ".", ""));
			this.PrintResult(visited.AllValues().Sum(x => x ? 1 : 0));
		}
	}

	public class SnakeNode
	{
		public Point position;

		private Action<int, int> onPointVisited;

		public SnakeNode Child { get; set; }
		public SnakeNode(Point p, Action<int, int> onPointVisited)
		{
			this.position = p;
			this.onPointVisited = onPointVisited;
		}

		public void Move(int x, int y, int steps)
		{
			for (var i = 0; i < steps; i++)
			{
				this.MoveTo(this.position.X + x, this.position.Y + y);
			}
		}

		public void MoveTo(int x, int y)
		{
			this.position.X = x;
			this.position.Y = y;
			if (this.Child == null)
			{
				this.onPointVisited(this.position.X, this.position.Y);
				return;
			}

			if (Child.position.XDistance(position, true) > 1)
			{
				var isBehind = Child.position.X < this.position.X;
				Child.MoveTo(this.position.X + (isBehind ? -1 : 1), this.position.Y);
			}
			else if (Child.position.YDistance(position, true) > 1)
			{
				var isBehind = Child.position.Y < this.position.Y;
				Child.MoveTo(this.position.X, this.position.Y + (isBehind ? -1 : 1));
			}
		}
	}

	public class Snake
	{
		Action<int, int> OnPointVisited;
		public Snake(Action<int, int> onPointVisited, int nodes)
		{
			var startPoint = new Point(300, 300);
			this.OnPointVisited = onPointVisited;
			this.OnPointVisited(startPoint.X, startPoint.Y);
			this.nodes = new SnakeNode[nodes];
			SnakeNode lastNode = null;
			for (var i = 0; i < nodes; i++)
			{
				var node = new SnakeNode(startPoint.Copy(), onPointVisited);
				this.nodes[i] = node;
				if (lastNode != null)
				{
					lastNode.Child = node;
				}

				lastNode = node;
			}
		}

		private SnakeNode[] nodes;

		public void Move(int x, int y, int steps)
		{
			this.nodes[0].Move(x, y, steps);
		}
	}

	public class Instruction : Parsable
	{
		public char Direction { get; set; }

		public int Steps { get; set; }
		public override void ParseFromLine(string line)
		{
			var tokens = line.Split(' ');
			this.Direction = tokens[0][0];
			this.Steps = int.Parse(tokens[1]);
		}

		public void Perform(Snake snake)
		{
			var deltaX = 0;
			var deltaY = 0;
			switch (this.Direction)
			{
				case 'R': deltaX = 1; break;
				case 'U': deltaY = -1; break;
				case 'L': deltaX = -1; break;
				case 'D': deltaY = 1; break;
			}

			snake.Move(deltaX, deltaY, this.Steps);
		}
	}
}
