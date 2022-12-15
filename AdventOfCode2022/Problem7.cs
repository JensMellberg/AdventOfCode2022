using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem7 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var commands = new Queue<string>(testData);
			Directory currentDirectory = null;
			Directory rootDirectory = null;
			while (commands.Any())
			{
				var command = commands.Dequeue();
				while (command.StartsWith("$ cd"))
				{
					var directoryName = command.Split(' ')[2];
					if (directoryName == "..")
					{
						currentDirectory = currentDirectory.parent;
					}
					else if (directoryName == "/")
					{
						rootDirectory = new Directory(directoryName, null);
						currentDirectory = rootDirectory;
					}
					else
					{
						currentDirectory = (Directory)currentDirectory.children.First(x => x.Name == directoryName);
					}

					command = commands.Dequeue();
				}

				if (command != "$ ls")
				{
					throw new Exception($"Expected $ ls but got {command}.");
				}

				while (commands.Any() && commands.Peek()[0] != '$')
				{
					var line = commands.Dequeue();
					var tokens = line.Split();
					ISystemNode file = null;
					if (tokens[0] == "dir")
					{
						file = new Directory(tokens[1], currentDirectory);
					} 
					else
					{
						file = new File(tokens[1], int.Parse(tokens[0]));
					}

					currentDirectory.children.Add(file);
				}
			}

			this.PrintResult(rootDirectory.Flatten().OfType<Directory>().Select(x => x.Size).Where(x => x <= 100000).Sum());
			var spaceNeeded = rootDirectory.Size - 40000000;
			this.PrintResult(rootDirectory.Flatten().OfType<Directory>().Select(x => x.Size).Where(x => x > spaceNeeded).OrderBy(x => x).First());
		}

		private class Directory : ISystemNode
		{
			private string name;

			public Directory parent { get; set; }
			public IList<ISystemNode> children { get; }

			private int size = -1;

			public Directory(string name, Directory parent)
			{
				this.name = name;
				this.children = new List<ISystemNode>();
				this.parent = parent;
			}

			public string Name => this.name;

			public int Size
			{
				get
				{
					if (this.size == -1)
					{
						this.size = this.children.Sum(x => x.Size);
					}

					return this.size;
				}
			}

			public IEnumerable<ISystemNode> Flatten() => children.SelectMany(x => x.Flatten()).Concat(new[] { this });
		}

		private class File : ISystemNode
		{
			private string name;
			private int size;
			public File(string name, int size)
			{
				this.name = name;
				this.size = size;
			}

			public int Size => this.size;

			public string Name => this.name;

			public IEnumerable<ISystemNode> Flatten() => new[] { this };

		}

		private interface ISystemNode
		{
			public int Size { get; }

			public string Name { get; }

			public IEnumerable<ISystemNode> Flatten();
		}
	}
}
