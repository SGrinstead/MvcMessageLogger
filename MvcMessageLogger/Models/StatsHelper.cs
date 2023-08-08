using MvcMessageLogger.DataAccess;
using System.Collections.Generic;

namespace MvcMessageLogger.Models
{
	public class StatsHelper
	{
		public static KeyValuePair<int, int> HourWithMostMessages(MvcMessageLoggerContext context)
		{
			var hours = context.Messages.GroupBy(message => message.CreatedAt.ToLocalTime().Hour);
			int hourWithMost = 0;
			int messageCount = 0;
			foreach (var hour in hours)
			{
				if (hour.Count() > messageCount)
				{
					hourWithMost = hour.Key;
					messageCount = hour.Count();
				}
			}
			return new KeyValuePair<int, int> ( hourWithMost, messageCount );
		}

		public static KeyValuePair<string, int> MostCommonWord(MvcMessageLoggerContext context)
		{
			string allWords = AllWords(context);
			var split = allWords.Split(",");
			var wordCount = new Dictionary<string, int>();

			foreach (string word in split)
			{
				string lowerWord = word.ToLower();
				if (string.IsNullOrEmpty(lowerWord))
				{
					continue;
				}
				if (!wordCount.ContainsKey(lowerWord))
				{
					wordCount.Add(lowerWord, 0);
				}
				wordCount[lowerWord]++;
			}
			var maxPair = wordCount.First(word => word.Value == wordCount.Max(word => word.Value));

			return maxPair;
		}

		private static string AllWords(MvcMessageLoggerContext context)
		{
			string allWords = "";
			var characterList = new List<string> { " ", "!", "' ", "?", ";", ":", ".", "/" };
			foreach (var message in context.Messages)
			{
				allWords += message.Content + " ";
			}
			foreach (string character in characterList)
			{
				if (allWords.Contains(character))
				{
					allWords = allWords.Replace(character, ",");
				}
			}
			return allWords;
		}
	}
}
