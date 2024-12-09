using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day7 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var operators = new Dictionary<char, Func<long, long, long>>
		{
			['+'] = (x, y) => x + y,
			['*'] = (x, y) => x * y
		};
		return Task.FromResult(SolveAsync(input, operators).ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var operators = new Dictionary<char, Func<long, long, long>>
		{
			['+'] = (x, y) => x + y,
			['*'] = (x, y) => x * y,
			['|'] = (x, y) => long.Parse(x.ToString() + y.ToString())
		};
		return Task.FromResult(SolveAsync(input, operators).ToString());
	}

	public long SolveAsync(IList<string> input, Dictionary<char, Func<long, long, long>> operators)
	{
		List<(long expected, List<long> values)> lines = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x =>
			{
				var y = x.Split(": ");
				return (
					long.Parse(y[0]),
					y[1]
						.Split(' ')
						.Select(long.Parse)
						.ToList()
				);
			})
			.ToList();

		long result = 0;
		foreach (var line in lines)
		{
			List<long>? values = null;
			foreach (var lineValue in line.values)
			{
				if (values is null)
				{
					values = new List<long> { lineValue };
					continue;
				}

				values = values.SelectMany(
						x => operators
							.Select(op => op.Value(x, lineValue)))
					.ToList();
			}

			if (values is not null && values.Any(x => x == line.expected)) result += line.expected;
		}

		return result;
	}
}