using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day3 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		List<List<int>> banks = input
			.Select(bank => bank.Select(num => int.Parse(num.ToString())).ToList())
			.ToList();

		List<int> joltageNums = new();
		foreach (var bank in banks)
		{
			var nums = bank.Index().OrderBy(x => x.Item).ToList();
			int skips = 0;
			while (true)
			{
				var max = nums
					.Take(nums.Count - skips)
					.MaxBy(x => x.Item);

				if (max.Index == nums.Count - 1)
				{
					skips++;
					continue;
				}

				var secondMax = nums
					.Where(x => x.Index > max.Index)
					.MaxBy(x => x.Item);

				joltageNums.Add(int.Parse($"{max.Item}{secondMax.Item}"));
				break;
			}
		}

		return Task.FromResult(joltageNums.Sum().ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		List<List<int>> banks = input
			.Select(bank => bank.Select(num => int.Parse(num.ToString())).ToList())
			.ToList();

		List<long> joltageNums = new();
		foreach (var bank in banks)
		{
			var nums = bank
				.Index()
				.OrderBy(x => x.Index)
				.ThenByDescending(x => x.Item)
				.ToList();
			int skips = 0;
			var foundNums = new List<(int Index, int Item)>();
			for (int i = 0; i < 12; i++)
			{
				var indexConstraint = i - 1 < 0 ? -1 : foundNums[i - 1].Index;
				var max = nums
					.Where(x => x.Index < nums.Count - (11 - i))
					.Where(x => x.Index > indexConstraint)
					.MaxBy(x => x.Item);
				foundNums.Add(max);
			}

			joltageNums.Add(long.Parse(string.Join("", foundNums.Select(x => x.Item))));
		}

		return Task.FromResult(joltageNums.Sum().ToString());
	}
}