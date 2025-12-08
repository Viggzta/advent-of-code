using System.Linq.Expressions;
using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day8 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		List<(int X, int Y, int Z)> positions = input.Select(x =>
			{
				var split = x.Split(',');
				return (int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
			})
			.ToList();

		var used = new HashSet<(int X, int Y, int Z)>();
		var networks = new List<HashSet<(int X, int Y, int Z)>>();
		var orderedPositions = positions
			.Where(x => !used.Contains(x))
			.OrderBy(
				x => positions
					.Where(y => x != y)
					.Select(y => Distance(x, y))
					.Min())
			.ToList();
		foreach (var position in orderedPositions)
		{
			var posDist = positions
				.Where(x => x != position)
				.OrderBy(x => Distance(x, position))
				.FirstOrDefault();

			var first = position;
			var second = posDist;
			Console.WriteLine($"{first} | {second}");

			if (posDist != default)
			{
				var hasBoth = networks
					.Any(n => n.Contains(posDist) && n.Contains(position));
				if (hasBoth)
				{
					// Do nothing
				}
				else
				{
					var hasOneNetwork = networks
					.FirstOrDefault(n => n.Contains(posDist) || n.Contains(position));
					if (hasOneNetwork != null)
					{
						hasOneNetwork.Add(posDist);
						hasOneNetwork.Add(position);
					}
					else
					{
						var newNetwork = new HashSet<(int X, int Y, int Z)>();
						newNetwork.Add(position);
						newNetwork.Add(posDist);
						networks.Add(newNetwork);
					}

					used.Add(posDist);
					used.Add(position);
				}
			}
			else
			{
				var newNetwork = new HashSet<(int X, int Y, int Z)>();
				newNetwork.Add(position);
				networks.Add(newNetwork);
				used.Add(position);
			}
		}

		var result = networks
			.Select(x => (long)x.Count)
			.OrderByDescending(x => x)
			.Take(3)
			.Aggregate((a, b) => a * b);

		return Task.FromResult(result.ToString());
	}

	private Dictionary<((int Xa, int Ya, int Za), (int Xb, int Yb, int Zb)), double> mem = new();
	public double Distance((int X, int Y, int Z) a, (int X, int Y, int Z) b)
	{
		if (mem.TryGetValue((a, b), out var distance))
		{
			return distance;
		}

		(long X, long Y, long Z) temp = (a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		var dist =
			Math.Sqrt(
			(temp.X * temp.X) +
			(temp.Y * temp.Y) +
			(temp.Z * temp.Z));
		mem[(a, b)] = dist;
		return dist;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		return Task.FromResult("");
	}
}