using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AdventOfCode2022
{
	public class ParsableUtils
	{
		public static T CreateFromLine<T>(string line)
			where T : Parsable, new()
		{
			var instance = new T();
			instance.ParseFromLine(line);
			return instance;
		}
    }
}
