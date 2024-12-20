using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AdventOfCode2022
{
	public class PatternParser
	{
		private List<Token> tokens = new List<Token>();
		private const char Separator = '¤';
		public PatternParser(string pattern)
		{
			var index = 0;
            string variable = null;
            string constant = null;
            while (index < pattern.Length)
			{
				if (pattern[index] == Separator)
				{
					index++;
					variable = "";
                    while (pattern[index] != Separator)
					{
                        variable += pattern[index];
						index++;
					}

					index++;
					if (index == pattern.Length)
					{
                        tokens.Add(new Token { VariableName = variable, TrailingPattern = null });
                    }
                }
				else
				{
                    constant = "";
					var alternativesByIndex = new Dictionary<int, List<string>>();
                    while (index < pattern.Length && pattern[index] != Separator)
                    {
						if (pattern[index] == '[')
						{
							var alternatives = new List<string>();
							alternativesByIndex.Add(constant.Length, alternatives);
							var currentAlt = "";
							index++;
							while (pattern[index] != ']')
							{
                                if (pattern[index] == '|')
								{
									alternatives.Add(currentAlt);
									currentAlt = "";
								}
								else
								{
									currentAlt += pattern[index];
								}

                                index++;
							}

							alternatives.Add(currentAlt);
						}

                        constant += pattern[index];
                        index++;
                    }

                    tokens.Add(new Token { VariableName = variable, TrailingPattern = constant, AlternativesByIndex = alternativesByIndex });
                }
			}
		}

		public T ParseObject<T>(string line) where T : new()
		{
			var currentIndex = 0;
			var currentToken = this.tokens[0];
			var stringIndex = 0;
			var currentVariable = "";
			var currentConstant = "";
			var currentConstantIndex = 0;
			var result = new List<(string variableName, string variableValue)>();
			while (stringIndex < line.Length)
			{
				if (currentToken.TrailingPattern == null)
				{
					currentVariable = line.Substring(stringIndex);
					stringIndex = line.Length;
					AddResult();
                    break;
				}

				var character = line[stringIndex];
				var patternCharacter = currentToken.TrailingPattern[currentConstantIndex];
				if (currentToken.AlternativesByIndex.TryGetValue(currentConstantIndex, out var alternatives)) {
					var tempConstantIndex = currentConstantIndex;
					foreach (var alt in alternatives)
					{
						if (line.Substring(stringIndex, alt.Length) == alt)
						{
							stringIndex += alt.Length;
                            character = line[stringIndex];
                            currentConstantIndex++;
							currentConstant += "]";
							break;
						}
					}

					if (currentConstantIndex == currentToken.TrailingPattern.Length)
					{
                        AddResult();
						continue;
                    }
					else
					{
						patternCharacter = currentToken.TrailingPattern[currentConstantIndex];
                    }
				}
                
				if (character == patternCharacter || patternCharacter == '$')
				{
					currentConstant += character;
					currentConstantIndex++;
					if (currentConstant.Length == currentToken.TrailingPattern.Length)
					{
                        AddResult();
					}
				} 
				else
				{
					currentConstantIndex = 0;
					currentVariable += currentConstant + character;
					currentConstant = "";
				}

				stringIndex++;

				void AddResult()
				{
					if (currentToken.VariableName != null && currentToken.VariableName != "_")
					{
                        result.Add((currentToken.VariableName, currentVariable));
                    }
					
					currentVariable = "";
                    currentConstant = "";
                    currentConstantIndex = 0;
					currentIndex++;
					currentToken = currentIndex == this.tokens.Count ? null : this.tokens[currentIndex];
                }
			}

			var returnObject = new T();
			var counter = 1;
			foreach (var attribute in result)
			{
                var propertyInfo = returnObject.GetType().GetProperty(attribute.variableName);
				if (propertyInfo == null)
				{
					var tupleProperty = returnObject.GetType().GetField("Item" + counter);
					if (tupleProperty != null)
					{
						SetField(tupleProperty);
					}
					else
					{
                        throw new Exception($"No valid property on type {returnObject.GetType()}");
                    }
				}
				else
				{
					SetProperty();
				}

				counter++;

				void SetField(FieldInfo fi)
				{
                    var targetType = fi.FieldType;
					object tempObj = returnObject;
					fi.SetValue(tempObj, CastValue(targetType, attribute.variableValue));
					returnObject = (T)tempObj;
                }

				void SetProperty()
				{
                    var targetType = propertyInfo.PropertyType;
                    propertyInfo.SetValue(returnObject, CastValue(targetType, attribute.variableValue));
                }

                object CastValue(Type targetType, string value)
                {
                    if (typeof(Parsable).IsAssignableFrom(targetType))
                    {
                        var valueObject = (Parsable)Activator.CreateInstance(targetType);
                        valueObject.ParseFromLine(attribute.variableValue);
						return valueObject;
                    }

                    if (targetType == typeof(int))
                    {
						return int.Parse(value);
                    }
                    else if (targetType == typeof(long))
                    {
                        return long.Parse(value);
                    }

					return value;
                }
            }


			return returnObject;
        }

		private class Token
		{
            public string TrailingPattern { get; set; }

			public string VariableName { get; set; }

			public Dictionary<int, List<string>> AlternativesByIndex { get; set; }
        }
    }
}
