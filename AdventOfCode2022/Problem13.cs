using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022
{
	public class Problem13 : ObjectProblem<PacketList>
	{
		public override void Solve(IEnumerable<PacketList> testData)
		{
			var i = 0;
			var input = testData.ToList();
			var pairs = new List<PacketPair>();
			while (i < input.Count)
			{
				pairs.Add(new PacketPair(input[i], input[i + 1]));
				i += 2;
			}

			this.PrintResult(pairs.Select((x, i) => (x, i)).Sum(x => x.x.Compare == 1 ? x.i + 1 : 0));
			var _ = 0;
			var __ = 0;
			var firstDivider = PacketList.ParseNext("[[2]]", ref _);
			var secondDivider = PacketList.ParseNext("[[6]]", ref __);
			var allPackets = input.Concat(new[] { firstDivider, secondDivider }).ToList();
			allPackets.Sort((a, b) => a.Compare(b) * -1);
			this.PrintResult((allPackets.IndexOf(firstDivider) + 1) * (allPackets.IndexOf(secondDivider) + 1));
		}
	}

	public interface PacketData
	{
		public int Compare(PacketData other);

		public bool IsList { get; }

		public string Print { get; }
	}

	public class PacketInteger : PacketData
	{
		public int Value { get; set; }

		public bool IsList => false;

		public int Compare(PacketData other)
		{
			if (other.IsList)
			{
				return new PacketList(new List<PacketData>(new[] { this })).Compare(other);
			}

			var value = other as PacketInteger;
			if (this.Value < value.Value)
			{
				return 1;
			} 
			else if (this.Value == value.Value)
			{
				return 0;
			}

			return -1;
		}

		public string Print => this.Value.ToString();
	}

	public class PacketList : Parsable, PacketData
	{
		public IList<PacketData> Values = new List<PacketData>();

		public bool IsList => true;

		public PacketList(IList<PacketData> values)
		{
			this.Values = values;
		}

		public PacketList() { }

		public int Compare(PacketData other)
		{
			if (other is PacketList list)
			{
				for (var i = 0; i < this.Values.Count; i++)
				{
					if (i >= list.Values.Count)
					{
						return -1;
					}

					var result = this.Values[i].Compare(list.Values[i]);
					if (result != 0)
					{
						return result;
					}
				}

				
				return list.Values.Count == this.Values.Count ? 0 : 1;
			}

			return this.Compare(new PacketList(new List<PacketData>(new[] { other })));
		}

		public string Print => '[' + string.Join(',', this.Values.Select(x => x.Print)) + ']';

		public override void ParseFromLine(string line)
		{
			var index = 0;
			this.Values = ((PacketList)ParseNext(line, ref index)).Values;
			base.ParseFromLine(line);
		}

		public static PacketData ParseNext(string line, ref int index)
		{
			if (line[index] == '[')
			{
				var values = new List<PacketData>();
				index++;
				while (line[index] != ']')
				{
					values.Add(ParseNext(line, ref index));
					if (line[index] == ',')
					{
						index++;
					}
				}

				index++;
				return new PacketList(values);
			}
			else if (IsNumber(line[index]))
			{
				int startIndex = index;
				while (IsNumber(line[index]))
				{
					index++;
				}

				var endIndex = index;
				if (line[index] == ',')
				{
					index++;
				}

				return new PacketInteger
				{
					Value = int.Parse(line.Substring(startIndex, endIndex - startIndex))
				};
			} 
			else
			{
				throw new Exception($"Unexpected token at line {line}. Expected '[' or number but got {line[index]}");
			}

			bool IsNumber(char c) => c >= '0' && c <= '9';
		}
	}

	public class PacketPair
	{
		private PacketData first;
		private PacketData second;
		public PacketPair(PacketData first, PacketData second)
		{
			this.first = first;
			this.second = second;
		}

		public int Compare => first.Compare(second);

		public string Print => first.Print + '\n' + second.Print;
	}
}
