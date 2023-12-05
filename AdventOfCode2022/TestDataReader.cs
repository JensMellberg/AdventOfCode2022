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
		private static readonly HttpClient client;

		private static HttpClient Client => client ?? CreateClient();

		public static string GetTestData(string day, bool isTest)
		{
			var fileName = $"{Program.CurrentYearNumber}/" + (isTest ? $"Test{day}.txt" : $"Input{day}.txt");
			string testData;
			if (!File.Exists(fileName))
			{
				testData = Client.GetStringAsync($"https://adventofcode.com/{Program.CurrentYearNumber}/day/{day}/input").Result;
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
			var handler = new HttpClientHandler
			{
				CookieContainer = new CookieContainer()
			};

			HttpClient client = new HttpClient(handler);
			var sessionKey = "53616c7465645f5ffbd475000cc30e798a0e4aef0766e5bdb992c2f8e970179a2f17048a2e395e5d0716c53017fcefd60269fccaa84f9e7d4405840c8f438773";
			handler.CookieContainer.Add(uri, new Cookie("session", sessionKey));
			return client;
		}
    }
}
