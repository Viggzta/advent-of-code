using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day5 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		List<(long From, long To)> ranges = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x =>
			{
				var split = x.Split('-');
				return (long.Parse(split[0]), long.Parse(split[1]));
			})
			.ToList();

		var ids = input
			.Skip(ranges.Count + 1)
			.Select(long.Parse)
			.ToList();

		var freshIds = ids
			.Count(x => ranges.Any(y => y.From <= x && y.To >= x));

		return Task.FromResult(freshIds.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		List<(long From, long To)> ranges = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x =>
			{
				var split = x.Split('-');
				return (long.Parse(split[0]), long.Parse(split[1]));
			})
			.OrderBy(x => x.Item1)
			.ToList();

		var newRanges = new List<(long From, long To)>();
		foreach (var range in ranges)
		{
			var overlap = newRanges.FirstOrDefault(x => Overlaps(x, range));
			if (overlap == default((int, int)))
			{
				newRanges.Add(range);
			}
			else
			{
				newRanges.Remove(overlap);
				newRanges.Add((Math.Min(range.From, overlap.From), Math.Max(range.To, overlap.To)));
			}
		}

		long count = 0;
		foreach (var range in newRanges)
		{
			count += range.To - range.From + 1;
		}

		return Task.FromResult(count.ToString());
	}

	private bool Overlaps((long From, long To) range1, (long From, long To) range2)
	{
		if (range1.From <= range2.To && range1.To >= range2.From)
		{
			return true;
		}

		return false;
	}
}