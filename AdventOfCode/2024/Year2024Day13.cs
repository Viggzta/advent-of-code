using System.Data.Common;
using System.Text.RegularExpressions;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day13 : IDay
{
	static Regex parseRegex = new Regex(
		@"(Button [AB]: X\+(?<X>\d+), Y\+(?<Y>\d+))|(Prize: X=(?<X>\d+), Y=(?<Y>(\d+)))", RegexOptions.Compiled);

	private static List<((int x, int y) buttonA, (int x, int y) buttonB, (int x, int y) prize)> ParseInput(IList<string> input)
	{
		List<((int x, int y) buttonA, (int x, int y) buttonB, (int x, int y) prize)> groups = new();
		for (int i = 0; i < input.Count; i++)
		{
			var g = input.Skip(i).Take(3).ToList();
			var valueTuples = g.Select(x =>
				{
					Match m = parseRegex.Match(x);
					if (m.Success)
					{
						return (int.Parse(m.Groups["X"].Value), int.Parse(m.Groups["Y"].Value));
					}

					return (-1, -1);
				})
				.ToList();

			groups.Add((valueTuples[0], valueTuples[1], valueTuples[2]));
			i += 3;
		}

		return groups;
	}

	public Task<string> RunSolution1Async(IList<string> input)
	{
		List<((int x, int y) buttonA, (int x, int y) buttonB, (int x, int y) prize)> groups = ParseInput(input);

		var totalSpend = 0L;
		foreach (var group in groups)
		{
			var validSolutions = new List<(int a, int b, long cost)>();
			for (int a = 0; a < 100; a++)
			{
				for (int b = 0; b < 100; b++)
				{
					if ((group.buttonA.x * a + group.buttonB.x * b) == group.prize.x &&
							(group.buttonA.y * a + group.buttonB.y * b) == group.prize.y)
					{
						var cost = a * 3L + b;
						validSolutions.Add((a, b, cost));
					}
				}
			}

			if (validSolutions.Any())
			{
				var minPrize = validSolutions.OrderBy(a => a.cost).First();
				totalSpend += minPrize.cost;
			}
		}

		var result = totalSpend.ToString();
		return Task.FromResult(result);
	}

	private static List<((long x, long y) buttonA, (long x, long y) buttonB, (long x, long y) prize)> ParseInput2(IList<string> input, bool withAdd = true)
	{
		List<((long x, long y) buttonA, (long x, long y) buttonB, (long x, long y) prize)> groups = new();
		for (int i = 0; i < input.Count; i++)
		{
			var g = input.Skip(i).Take(3).ToList();
			var valueTuples = g.Select(x =>
				{
					Match m = parseRegex.Match(x);
					if (m.Success)
					{
						return (long.Parse(m.Groups["X"].Value), long.Parse(m.Groups["Y"].Value));
					}

					return (-1, -1);
				})
				.ToList();

			var prize = (
				(withAdd ? 10000000000000 : 0) + valueTuples[2].Item1,
				(withAdd ? 10000000000000 : 0) + valueTuples[2].Item2
				);
			groups.Add((valueTuples[0], valueTuples[1], prize));
			i += 3;
		}

		return groups;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		List<((long x, long y) buttonA, (long x, long y) buttonB, (long x, long y) prize)> groups =
			ParseInput2(input);

		var totalSpend = 0L;
		foreach (var group in groups)
		{
			var validSolutions = new List<(long a, long b, long cost)>();

			var cheapest = GetCheapest(
				group.buttonA.x,
				group.buttonA.y,
				group.buttonB.x,
				group.buttonB.y,
				group.prize.x,
				group.prize.y);

			if (cheapest != (-1, -1))
			{
				var cost = cheapest.a * 3L + cheapest.b;
				validSolutions.Add((cheapest.a, cheapest.b, cost));
			}

			if (validSolutions.Any())
			{
				var minPrize = validSolutions.OrderBy(a => a.cost).First();
				totalSpend += minPrize.cost;
			}
		}

		var result = totalSpend.ToString();
		return Task.FromResult(result);
	}

	public (long a, long b) GetCheapest(long a1, long a2, long b1, long b2, long c1, long c2)
	{
		// Cramer's rule
		var denominator = a1 * b2 - b1 * a2;
		long d1 = (c1 * b2 - b1 * c2) / denominator;
		long d2 = (a1 * c2 - c1 * a2) / denominator;
		if ((d1 * a1) + (d2 * b1) == c1 && (d1 * a2) + (d2 * b2) == c2)
		{
			return (d1, d2);
		}

		return (-1, -1);
	}
}