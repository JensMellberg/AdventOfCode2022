using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AdventOfCode2022
{
	public class TestDataReader
	{
		private static HttpClient client;

		private static HttpClient Client => client != null ? client : CreateClient();

		public static string GetTestData(string day)
		{
			var fileName = $"Input{day}.txt";
			string testData;
			if (!File.Exists(fileName))
			{
				testData = Client.GetStringAsync($"https://adventofcode.com/2022/day/{day}/input").Result;
				File.WriteAllText(fileName, testData);
			}
			else
			{
				testData = File.ReadAllText(fileName);
			}

			if (string.IsNullOrEmpty(testData))
			{
				throw new Exception($"Failed to retrieve test data for problem {day}");
			}

			return testData;
		}

		private static HttpClient CreateClient()
		{
			Uri uri = new Uri("https://adventofcode.com");
			var handler = new HttpClientHandler();
			handler.CookieContainer = new CookieContainer();

			HttpClient client = new HttpClient(handler);
			var sessionKey = "53616c7465645f5f0cc303b36763213e606642dba3188a05b953f5ce923ff6bab5d1ec897e5569030b8372f00dcfe8641246abd245b76eb9e206c945cfa73ed1";
			handler.CookieContainer.Add(uri, new Cookie("session", sessionKey)); // Adding a Cookie
			return client;
		}
    }
}
