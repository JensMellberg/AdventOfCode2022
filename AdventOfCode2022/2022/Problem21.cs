using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode2022.TwentyTwo
{
	public class Problem21 : ObjectProblem<Monkey>
	{
		public override void Solve(IEnumerable<Monkey> testData)
		{
			var input = testData.ToList();
			var monkeysByName = input.ToDictionary(x => x.Name, x => x);
			this.PrintResult(monkeysByName["root"].Expression.Value(monkeysByName));
			monkeysByName["humn"].Expression.IsX = true;
			this.PrintResult(((MonkeyExpression)(monkeysByName["root"].Expression)).Top(monkeysByName));
		}
	}

	public class Monkey : Parsable
	{
		public string Name { get; set; }

		public Expression Expression { get; set; }

		public override void ParseFromLine(string line)
		{
			var tokens = line.Replace(":", "").Split(" ");
			this.Name = tokens[0];
			this.Expression = Expression.Parse(tokens);
			base.ParseFromLine(line);
		}
	}

	public class Result
	{
		public bool VariableResult { get; set; }

		public long IntResult { get; set; }

		public Result ExpressionResult { get; set; }

		public bool IsX { get; set; }
	}

	public interface Expression
	{
		public long Value(Dictionary<string, Monkey> monkeysByName);

		public static Expression Parse(string[] tokens)
		{
			if (tokens.Length > 2)
			{
				return new MonkeyExpression(tokens[1], tokens[3], tokens[2]);
			}

			return new NumberExpression(int.Parse(tokens[1]));
		}

		public bool ContainsVariable(Dictionary<string, Monkey> monkeysByName);

		public long Evaluate(Dictionary<string, Monkey> monkeysByName, long equality);

		public bool IsX { get; set; }
	}

	public class NumberExpression : Expression
	{
		public NumberExpression(long value)
		{
			this.value = value;
		}

		private long value;

		public long Value(Dictionary<string, Monkey> monkeysByName) => this.value;

		public bool ContainsVariable(Dictionary<string, Monkey> monkeysByName) => IsX;

		public long Evaluate(Dictionary<string, Monkey> monkeysByName, long equality) => IsX ? equality : this.value;

		public bool IsX { get; set; }
	}

	public class MonkeyExpression : Expression
	{
		private string Operator;

		public string leftMonkeyName { get; set; }

		public string rightMonkeyName { get; set; }

		public bool IsX { get; set; }

		public MonkeyExpression(string leftMonkeyName, string rightMonkeyName, string Operator)
		{
			this.leftMonkeyName = leftMonkeyName;
			this.rightMonkeyName = rightMonkeyName;
			this.Operator = Operator;
		}

		public long Value(Dictionary<string, Monkey> monkeysByName)
		{
			var left = monkeysByName[leftMonkeyName].Expression.Value(monkeysByName);
			var right = monkeysByName[rightMonkeyName].Expression.Value(monkeysByName);
			return Operator switch
			{
				"+" => left + right,
				"-" => left - right,
				"*" => left * right,
				"/" => left / right,
				_ => throw new Exception($"Unexpected operator {Operator}")
			};
		}

		private bool? containsVariable;

		public bool ContainsVariable(Dictionary<string, Monkey> monkeysByName)
		{
			if (containsVariable.HasValue)
			{
				return containsVariable.Value;
			}

			return monkeysByName[leftMonkeyName].Expression.ContainsVariable(monkeysByName) ||
				monkeysByName[rightMonkeyName].Expression.ContainsVariable(monkeysByName);
		}

		public (Expression variableExpression, Expression constantExpression) GetExpressions(Dictionary<string, Monkey> monkeysByName)
		{
			var variableExpression = monkeysByName[leftMonkeyName].Expression.ContainsVariable(monkeysByName)
				? monkeysByName[leftMonkeyName].Expression
				: monkeysByName[rightMonkeyName].Expression;

			var constantExpression = monkeysByName[leftMonkeyName].Expression.ContainsVariable(monkeysByName)
				? monkeysByName[rightMonkeyName].Expression
				: monkeysByName[leftMonkeyName].Expression;

			return (variableExpression, constantExpression);
		} 

		public long Evaluate(Dictionary<string, Monkey> monkeysByName, long equality)
		{
			var (variableExpression, constantExpression) = this.GetExpressions(monkeysByName);
			var multiplier = monkeysByName[leftMonkeyName].Expression.ContainsVariable(monkeysByName)
				? 1
				: -1;
			long newEquality;
			var value = constantExpression.Value(monkeysByName);
			switch (this.Operator)
			{
				case "+": newEquality = equality - value; break;
				case "*": newEquality = equality / value; break;
				case "-": newEquality = (equality * multiplier) + value; break;
				case "/": newEquality = multiplier == 1
						? equality * value
						: value / equality; break;
				default: throw new Exception("hehehe");
			}

			return variableExpression.Evaluate(monkeysByName, newEquality);
		}

		public long Top(Dictionary<string, Monkey> monkeysByName)
		{
			var (variableExpression, constantExpression) = this.GetExpressions(monkeysByName);
			var equality = constantExpression.Value(monkeysByName);
			return variableExpression.Evaluate(monkeysByName, equality);
		}
	}
}
