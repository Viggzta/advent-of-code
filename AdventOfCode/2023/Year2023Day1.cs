using AdventOfCode.Utility;

namespace AdventOfCode._2023;

public class Year2023Day1 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		IList<string> inputLines = input.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

		var sum = 0;
		foreach (var line in inputLines)
		{
			var numbers = line.Where(x => char.IsNumber(x)).Select(x => int.Parse(x.ToString())).ToList();

			var firstNum = numbers.First();
			var lastNum = numbers.Last();

			var actualNum = int.Parse(firstNum.ToString() + lastNum);
			sum += actualNum;
		}

		return Task.FromResult(sum.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		throw new NotImplementedException();
	}
}