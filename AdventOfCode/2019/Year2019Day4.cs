using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day4 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var tuple = input.First().Split('-').ToList();
		var a = int.Parse(tuple[0]);
		var b = int.Parse(tuple[1]);

		int validPasswords = 0;
		foreach (var pass in Enumerable.Range(a, b - a))
		{
			var passStr = pass.ToString().Select(c => int.Parse(c.ToString())).ToList();
			var prev = -1;
			var isIncreasing = true;
			var containsDuplicate = false;
			foreach (var num in passStr)
			{
				if (prev == num) containsDuplicate = true;
				if (num < prev)
				{
					isIncreasing = false;
					break;
				}

				prev = num;
			}

			if (isIncreasing && containsDuplicate) validPasswords++;
		}

		return Task.FromResult(validPasswords.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var tuple = input.First().Split('-').ToList();
		var a = int.Parse(tuple[0]);
		var b = int.Parse(tuple[1]);

		int validPasswords = 0;
		foreach (var pass in Enumerable.Range(a, b - a + 1))
		{
			var passStr = pass.ToString().Select(c => int.Parse(c.ToString())).ToList();
			var prev = -1;
			var isIncreasing = true;
			var dupeDictionary = new Dictionary<int, int>();
			foreach (var num in passStr)
			{
				dupeDictionary.TryAdd(num, 0);
				if (prev == num) dupeDictionary[num]++;
				if (num < prev)
				{
					isIncreasing = false;
					break;
				}

				prev = num;
			}

			if (isIncreasing && dupeDictionary.Any(x => x.Value == 1)) validPasswords++;
		}

		return Task.FromResult(validPasswords.ToString());
	}
}