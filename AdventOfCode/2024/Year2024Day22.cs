using System.Collections.Concurrent;
using System.Diagnostics;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day22 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var secretNumbers = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(long.Parse)
			.ToList();

		var twoKRandomNum = new List<long>();
		foreach (var number in secretNumbers)
		{
			var randNum = number;
			for (int i = 0; i < 2000; i++)
			{
				randNum = GetPsuedoRandomNumber(randNum);
			}
			twoKRandomNum.Add(randNum);
		}

		var result = twoKRandomNum.Sum();
		return Task.FromResult(result.ToString());
	}

	public async Task<string> RunSolution2Async(IList<string> input)
	{
		var secretNumbers = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(long.Parse)
			.ToList();

		var priceLists = new List<List<long>>();
		foreach (var number in secretNumbers)
		{
			var randNum = number;
			var prices = new List<long>();
			for (int i = 0; i < 2000; i++)
			{
				randNum = GetPsuedoRandomNumber(randNum);
				var price = GetItemPrice(randNum);
				prices.Add(price);
			}
			priceLists.Add(prices);
		}

		var priceChanges = priceLists
			.Select(x => GetPriceChanges(x).ToList())
			.ToList();

		var possibleTailNumbers = priceLists
			.Select(GetPriceChanges)
			.SelectMany(x => GetTailNumber(x.ToList()))
			.Distinct()
			.ToList();

		/*
		var possibleMaxes = possibleTailNumbers
			.Select(x => SellItemsAsync(priceLists, x))
			.ToList();

		var possibleMaxesValues = await Task.WhenAll(possibleMaxes);
		*/

		var lookupDictionaries = GetPatternLookupDictionaries(priceLists, priceChanges);

		ConcurrentBag<long> results = new();
		var maxIter = possibleTailNumbers.Count;
		var iter = 0;

		Parallel.ForEach(
			possibleTailNumbers,
			() => long.MinValue,
			(tailNumber, _, max) =>
			{
				var gained = SellItems(lookupDictionaries, tailNumber);
				return Math.Max(gained, max);
			},
			max =>
			{
				Interlocked.Increment(ref iter);
				results.Add(max);
			});

		var result = results.Max();
		return result.ToString();
	}

	private List<Dictionary<(long, long, long, long), long>> GetPatternLookupDictionaries(
		List<List<long>> priceLists,
		List<List<long>> priceChanges)
	{
		var outList = new List<Dictionary<(long, long, long, long), long>>();
		for (int j = 0; j < priceLists.Count; j++)
		{
			Dictionary<(long, long, long, long), long> tempDict = new();
			var price = priceLists[j];
			var changes = priceChanges[j];

			for (int i = 3; i < changes.Count; i++)
			{
				tempDict.TryAdd((changes[i - 3], changes[i - 2], changes[i - 1], changes[i]), price[i + 1]);
			}

			outList.Add(tempDict);
		}

		return outList;
	}

	private long SellItems(
		List<Dictionary<(long, long, long, long), long>> sellLookupDictionaries,
		(long a, long b, long c, long d) tailPattern)
	{
		var result = 0L;
		foreach (var dict in sellLookupDictionaries)
		{
			result += dict.TryGetValue(tailPattern, out var value) ? value : 0;
		}
		/*
		for (int j = 0; j < priceLists.Count; j++)
		{
			var price = priceLists[j];
			var changes = priceChanges[j];

			for (int i = 3; i < changes.Count; i++)
			{
				if (changes[i - 3] == tailPattern.a &&
						changes[i - 2] == tailPattern.b &&
						changes[i - 1] == tailPattern.c &&
						changes[i] == tailPattern.d)
				{
					result += price[i + 1]; // Might be off by 1
					break;
				}
			}
		}
		*/

		return result;
	}

	private IEnumerable<(long a, long b, long c, long d)> GetTailNumber(
		IReadOnlyList<long> priceChange)
	{
		for (int i = 3; i < priceChange.Count; i++)
		{
			yield return (priceChange[i - 3], priceChange[i - 2], priceChange[i - 1], priceChange[i]);
		}
	}

	private IEnumerable<long> GetPriceChanges(List<long> prices)
	{
		for (int i = 1; i < prices.Count; i++)
		{
			yield return prices[i] - prices[i - 1];
		}
	}

	private long GetItemPrice(long input) => input % 10;

	private long GetPsuedoRandomNumber(long input) => Step3(Step2(Step1(input)));

	private long Step1(long input) => Prune(Mix(input * 64, input));

	private long Step2(long input) => Prune(Mix(input / 32, input));

	private long Step3(long input) => Prune(Mix(input * 2048, input));

	private long Mix(long a, long b) => a ^ b;

	private long Prune(long input) => input % 16777216;
}