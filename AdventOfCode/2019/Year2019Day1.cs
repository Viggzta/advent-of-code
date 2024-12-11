using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day1 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var result = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => long.Parse(x) / 3 - 2)
			.Sum();
		return Task.FromResult(result.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var masses = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => long.Parse(x));

		long result = 0;
		foreach (var mass in masses)
		{
			long fuelCost = 0;
			long currentMass = mass;
			while (currentMass > 5)
			{
				currentMass = currentMass / 3 - 2;
				fuelCost += currentMass;
			}

			result += fuelCost;
		}

		return Task.FromResult(result.ToString());
	}
}