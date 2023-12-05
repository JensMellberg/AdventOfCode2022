using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem10 : StringProblem
	{
		public override void Solve(IEnumerable<string> testData)
		{
			var memory = new Memory();
			var instructions = testData.Select(FromLine);
			instructions.ForEach(x => x.Perform(memory));
			this.PrintResult(memory.SignalStrengths.Sum());
			this.Print(memory.Pixels.ToString(x => x ? "#" : ".", ""));

			Instruction FromLine(string line)
			{
				var tokens = line.Split(' ');
				if (tokens[0] == "noop")
				{
					return new Noop();
				}

				return new AddX(int.Parse(tokens[1]));
			}
		}


		private class Memory
		{
			private int cycle;
			public int registerX { get; set; }
			public List<int> SignalStrengths { get; }

			public Matrix<bool> Pixels;
			public Memory()
			{
				this.cycle = 0;
				this.registerX = 1;
				this.SignalStrengths = new List<int>();
				this.Pixels = Matrix.InitWithStartValue(6, 40, false);
			}

			public void IncreaseCycle()
			{
				this.cycle++;
				if ((this.cycle - 20) % 40 == 0)
				{
					this.SignalStrengths.Add(this.cycle * this.registerX);
				}

				var pixelX = (this.cycle - 1) % 40;
				var pixelY = (this.cycle - 1) / 40;
				if (pixelX >= this.registerX - 1 && pixelX <= this.registerX + 1)
				{
					Pixels[pixelX, pixelY] = true;
				}
			}
		}

		private interface Instruction
		{
			public void Perform(Memory memory);
		}

		private class Noop : Instruction
		{

			public void Perform(Memory memory)
			{
				memory.IncreaseCycle();
			}
		}

		private class AddX : Instruction
		{
			private int value;
			public AddX(int value)
			{
				this.value = value;
			}

			public void Perform(Memory memory)
			{
				memory.IncreaseCycle();
				
				memory.IncreaseCycle();
				memory.registerX += value;
			}
		}
	}
}
