using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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

		public static bool IsNumber(char s) => s >= '0' && s <= '9';

        public static long LowestCommonMultiple(IEnumerable<long> numbers)
        {
            return numbers.Aggregate((S, val) => S * val / GreatestCommonDivider(S, val));
        }

        public static long GreatestCommonDivider(long n1, long n2)
        {
            if (n2 == 0)
            {
                return n1;
            }
            else
            {
                return GreatestCommonDivider(n2, n1 % n2);
            }
        }
    }
}
