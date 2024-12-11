using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day11 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var numbers = input.First().Split(' ').Select(long.Parse).ToList();

		for (var times = 0; times < 25; times++)
		{
			for (int i = numbers.Count - 1; i >= 0; i--)
			{
				var current = numbers[i];
				if (current == 0)
				{
					numbers[i] = 1;
				}
				else if (current.ToString() is { } numString && numString.Length % 2 == 0)
				{
					var take = numString.Length / 2;
					var num1 = long.Parse(numString.Substring(0, take));
					var num2 = long.Parse(numString.Substring(numString.Length / 2, take));
					numbers[i] = num1;
					numbers.Insert(i, num2);
				}
				else
				{
					numbers[i] = numbers[i] * 2024;
				}
			}
		}

		return Task.FromResult(numbers.Count.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		Dictionary<long, long> numbers = input
			.First()
			.Split(' ')
			.Select(long.Parse)
			.GroupBy(x => x)
			.ToDictionary(x => x.Key, x => (long)x.Count());

		for (int times = 0; times < 75; times++)
		{
			Dictionary<long, long> nextSetOfNumbers = numbers
				.ToDictionary(x => x.Key, x => x.Value);
			foreach (KeyValuePair<long, long> current in numbers)
			{
				if (current.Key == 0)
				{
					nextSetOfNumbers.TryAdd(1, 0);
					nextSetOfNumbers[1] += current.Value;
				}
				else if (current.Key.ToString() is { } numString && numString.Length % 2 == 0)
				{
					int take = numString.Length / 2;
					long num1 = long.Parse(numString.Substring(0, take));
					long num2 = long.Parse(numString.Substring(numString.Length / 2, take));
					nextSetOfNumbers.TryAdd(num1, 0);
					nextSetOfNumbers[num1] += current.Value;
					nextSetOfNumbers.TryAdd(num2, 0);
					nextSetOfNumbers[num2] += current.Value;
				}
				else
				{
					long newNum = current.Key * 2024;
					nextSetOfNumbers.TryAdd(newNum, 0);
					nextSetOfNumbers[newNum] += current.Value;
				}
				nextSetOfNumbers[current.Key] -= current.Value;
			}

			numbers = nextSetOfNumbers
				.Where(x => x.Value > 0)
				.ToDictionary(x => x.Key, x => x.Value);
		}

		return Task.FromResult(numbers.Sum(x => x.Value).ToString());
	}
}