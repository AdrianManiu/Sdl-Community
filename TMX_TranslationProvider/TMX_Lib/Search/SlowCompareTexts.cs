﻿using System;
using System.Collections.Generic;
using System.Linq;
using TMX_Lib.Utils;

namespace TMX_Lib.Search
{
	// compares two texts, returns a score 0-1
	// this is a slow and detailed compare, to be run only after the fast searches have shown the probability for "sameness"
	public class SlowCompareTexts
	{
		private static Dictionary<string, int> StrToDictioary(string a)
		{
			var dic = new Dictionary<string, int>();
			foreach (var word in a.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries))
			{
				if (dic.ContainsKey(word))
					dic[word]++;
				else 
					dic.Add(word, 1);
			}

			return dic;
		}

		private class CharAndIndex
		{
			protected bool Equals(CharAndIndex other)
			{
				return Ch == other.Ch && CharIndex == other.CharIndex && WordIndex == other.WordIndex;
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != this.GetType()) return false;
				return Equals((CharAndIndex)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hashCode = Ch.GetHashCode();
					hashCode = (hashCode * 397) ^ CharIndex;
					hashCode = (hashCode * 397) ^ WordIndex;
					return hashCode;
				}
			}

			public char Ch;
			public int CharIndex = 0;
			public int WordIndex = 0;
		}
		private static List<CharAndIndex> WordToChars(string word)
		{
			var list = new List<CharAndIndex>();
			var idx = 0;
			foreach (var ch in word)
			{
				var exists = list.LastOrDefault(o => o.Ch == ch);
				if (exists != null)
					list.Add(new CharAndIndex { Ch = ch, CharIndex = exists.CharIndex + 1, WordIndex = idx});
				else
					list.Add(new CharAndIndex { Ch = ch, WordIndex = idx});
				++idx;
			}

			return list;
		}

		private static (string noPunct, string punct) RemovePunctuation(string a)
		{
			var punct = "";
			var noPunct = new string( a.Where(ch =>
			{
				if (Char.IsPunctuation(ch))
				{
					punct += ch;
					return false;
				}

				return true;
			}).ToArray());
			return (noPunct, punct);
		}

		// returns a word penalty, based on the order of the words - if they don't match
		private static double OrderPenalty(string a, string b)
		{
			// simple for now:
			// - 3% for each order mismatch (word missing or extra word or different word)
			// - 12% if two consecutive mismatches ("this is an interesting idea" vs "this interesting idea")
			var aWords = a.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
			var bWords = b.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
			var aIdx = 0;
			var bIdx = 0;
			var penalty = 0d;
			while (aIdx < aWords.Length && bIdx < bWords.Length)
			{
				if (a[aIdx] == b[bIdx])
				{
					aIdx++;
					bIdx++;
					continue;
				}

				penalty += 0.03;
				if (aIdx + 1 >= aWords.Length || bIdx + 1 >= bWords.Length)
					// can't compare against next word
					break;

				if (a[aIdx + 1] == b[bIdx + 1])
				{
					// word difference
					aIdx++;
					bIdx++;
					continue;
				}

				if (a[aIdx + 1] == b[bIdx])
				{
					aIdx++;
					continue;
				}

				if (a[aIdx] == b[bIdx + 1])
				{
					bIdx++;
					continue;
				}

				// two consecutive differences
				penalty = 0.12;
				break;
			}

			return penalty;
		}

		// returns a 0-1 score (0-least , 1-best)
		public static double Compare(string a, string b)
		{
			var charCount = Math.Min(a.Length, b.Length);

			string aPunct, bPunct;
			(a, aPunct) = RemovePunctuation(a);
			(b, bPunct) = RemovePunctuation(b);

			var aWordsDictionary = StrToDictioary(a);
			var bWordsDictionary = StrToDictioary(b);

			var penalty = 0d;

			// these will contain only the words that are different in a and b
			var aDiff = a;
			var bDiff = b;

			// first, see what words are different
			var keys = aWordsDictionary.Keys.ToList();
			keys.AddRange(bWordsDictionary.Keys);
			keys = keys.Distinct().ToList();
			// ... the idea: first remove 'its', before 'it'
			foreach (var word in keys.OrderByDescending(w => w.Length))
			{
				var aWordCount = 0;
				var bWordCount = 0;
				if (aWordsDictionary.TryGetValue(word, out var count))
					aWordCount = count;
				if (bWordsDictionary.TryGetValue(word, out count))
					bWordCount = count;

				// if this word is only in a and not in b or vice versa, keep it, and see if it's just a typo
				if (aWordCount > 0 && bWordCount > 0)
				{
					// for instance, if word X is 2 times in a and 3 times in b, we incur a penalty of a "word-length" chars
					penalty += Math.Abs(aWordCount - bWordCount) * word.Length;
					aDiff = aDiff.Replace(word, "");
					bDiff = bDiff.Replace(word, "");
				}
			}

			// check for word differences
			var aWords = aDiff.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
			var bWords = bDiff.Split(Constants.WORD_DELIMITERS, StringSplitOptions.RemoveEmptyEntries);
			var wordCount = Math.Max(aWords.Length, bWords.Length);
			for (int i = 0; i < wordCount; ++i)
			{
				var aWord = i < aWords.Length ? aWords[i] : null;
				var bWord = i < bWords.Length ? bWords[i] : null;
				if (aWord == null || bWord == null)
				{
					penalty += (aWord ?? bWord).Length;
					continue;
				}
				// here, we have both words
				if (Math.Abs(aWord.Length - b.Length) > 2)
				{
					// words are too different (by length)
					penalty += Math.Max(aWord.Length, bWord.Length);
					continue;
				}

				// words are similar in length
				var idx = 0;
				// ... the idea -> Except deals with sets, so i want to make each char unique, like "a 0" (first a), "a 1" (second a) and so on
				//     (otherwise, 'a' would be counted only once, no matter how many times it exists)
				var aChars = WordToChars(aWord);
				var bChars = WordToChars(bWord);
				var inAOnly = aChars.Except(bChars).ToList();
				var inBOnly = bChars.Except(aChars).ToList();
				if (inAOnly.Count + inBOnly.Count > 3)
				{
					// words are too different (their characters)
					penalty += Math.Max(aWord.Length, bWord.Length);
					continue;
				}

				// the idea: remove the different chars in both words a and b
				// if the remaining is the same, then there were very few diffs. otherwise, it's a word penalty
				foreach (var ch in inAOnly.OrderByDescending(ch => ch.WordIndex))
					aWord = aWord.Substring(0, ch.WordIndex) + aWord.Substring(ch.WordIndex + 1);
				foreach (var ch in inBOnly.OrderByDescending(ch => ch.WordIndex))
					bWord = bWord.Substring(0, ch.WordIndex) + bWord.Substring(ch.WordIndex + 1);

				if (aWord == bWord)
					penalty += inAOnly.Count + inBOnly.Count;
				else
					// words are too different (their characters)
					penalty += Math.Max(aWord.Length, bWord.Length);
			}

			if (penalty >= charCount)
				return 0;
			var result = (double)(charCount - penalty) / (double)charCount;
			if (aPunct != bPunct)
				// if not same punctuation -> remove 4%
				result = Math.Max(result - 0.04, 0);

			result = Math.Max(result - OrderPenalty(a, b), 0);
			return result;
		}
	}
}
