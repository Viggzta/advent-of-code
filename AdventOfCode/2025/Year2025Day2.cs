using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day2 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var pairs = input
			.First()
			.Split(',')
			.Select(
				x =>
				{
					var split = x.Split('-');
					return new
					{
						Start = long.Parse(split.First()),
						End = long.Parse(split.Last()),
					};
				})
			.ToList();

		List<long> invalidIds = new();
		foreach (var pair in pairs)
		{
			for (long num = pair.Start; num <= pair.End; num++)
			{
				var numString = num.ToString();
				if (!IsValid(numString))
				{
					invalidIds.Add(num);
				}
			}
		}

		var result = invalidIds.Sum().ToString();

		return Task.FromResult(result);
	}

	private bool IsValid(string numString)
	{
		var numbersToCompare = numString.Length / 2;
		if (numbersToCompare * 2 != numString.Length) return true;

		var a = numString.Substring(0, numbersToCompare);
		var b = numString.Substring(numbersToCompare, numbersToCompare);
		if (a == b)
		{
			return false;
		}

		return true;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var pairs = input
			.First()
			.Split(',')
			.Select(
				x =>
				{
					var split = x.Split('-');
					return new
					{
						Start = long.Parse(split.First()),
						End = long.Parse(split.Last()),
					};
				})
			.ToList();

		List<long> invalidIds = new();
		foreach (var pair in pairs)
		{
			for (long num = pair.Start; num <= pair.End; num++)
			{
				var numString = num.ToString();
				if (!IsValid2(numString))
				{
					invalidIds.Add(num);
				}
			}
		}

		var result = invalidIds.Sum().ToString();

		return Task.FromResult(result);
	}

	private bool IsValid2(string numString)
	{
		var numbersToCompare = numString.Length / 2;

		while (numbersToCompare > 0)
		{
			var pairs = new List<string>();
			int takenChars = 0;
			while (takenChars < numString.Length)
			{
				pairs.Add(new string(numString.Skip(takenChars).Take(numbersToCompare).ToArray()));
				takenChars += numbersToCompare;
			}

			if (pairs.All(x => x == pairs.First()))
			{
				return false;
			}

			numbersToCompare--;
			if (numbersToCompare == 0) break;
			while (numString.Length % numbersToCompare != 0)
			{
				numbersToCompare--;
			}
		}

		return true;
	}
}