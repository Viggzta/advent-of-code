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

		var connectionsMade = 0;


		HashSet<((int X, int Y, int Z) x, (int X, int Y, int Z) y)> orderedPositions2 = new();

		foreach (var position in positions)
		{
			var a = positions
				.Where(y => position != y)
				.Where(x => !orderedPositions2.Contains((x, position)) && !orderedPositions2.Contains((position, x)))
				.ToList();
			foreach (var x in a)
			{
				orderedPositions2.Add((x, position));
			}
		}
		
		var orderedPositions = orderedPositions2
			.OrderBy(x => Distance(x.x, x.y))
			.ToList();
		
		var networks = new List<HashSet<(int X, int Y, int Z)>>();
		foreach (var o in positions)
		{
			var nw = new HashSet<(int X, int Y, int Z)>();
			nw.Add(o);
			networks.Add(nw);
		}

		int i = 0;
		foreach (var position in orderedPositions)
		{
			i++;
			(int X, int Y, int Z) first = position.x;
			(int X, int Y, int Z) second = position.y;

			// var first = position;
			// var second = posDist;
			// Console.WriteLine($"{first} | {second}");

			connectionsMade++;
			var hasBoth = networks
				.Any(n => n.Contains(first) && n.Contains(second));
			if (hasBoth)
			{
				// Do nothing, already connected
			}
			else
			{
				Console.WriteLine($"{Distance(first, second)}");
				var existingNetworks = networks
				.Where(n => n.Contains(first) || n.Contains(second))
				.ToList();
				if (existingNetworks.Count == 2)
				{
					foreach (var n in existingNetworks[1])
					{
						existingNetworks[0].Add(n);
					}
					networks.Remove(existingNetworks[1]);
				}
				else
				{
					Console.WriteLine("Err");
				}

				// var limit = 1000;
				var limit = 1000;
				if (connectionsMade == (limit - 1))
				{
					//Console.WriteLine($"break at {i}");
					break;
				}
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
		input = input.Take(input.Count - 1).ToList();

		List<(int X, int Y, int Z)> positions = input.Select(x =>
			{
				var split = x.Split(',');
				return (int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
			})
			.ToList();

		var connectionsMade = 0;


		HashSet<((int X, int Y, int Z) x, (int X, int Y, int Z) y)> orderedPositions2 = new();

		foreach (var position in positions)
		{
			var a = positions
				.Where(y => position != y)
				.Where(x => !orderedPositions2.Contains((x, position)) && !orderedPositions2.Contains((position, x)))
				.ToList();
			foreach (var x in a)
			{
				orderedPositions2.Add((x, position));
			}
		}
		
		var orderedPositions = orderedPositions2
			.OrderBy(x => Distance(x.x, x.y))
			.ToList();
		
		var networks = new List<HashSet<(int X, int Y, int Z)>>();
		foreach (var o in positions)
		{
			var nw = new HashSet<(int X, int Y, int Z)>();
			nw.Add(o);
			networks.Add(nw);
		}

		int i = 0;
		foreach (var position in orderedPositions)
		{
			i++;
			(int X, int Y, int Z) first = position.x;
			(int X, int Y, int Z) second = position.y;

			// var first = position;
			// var second = posDist;
			// Console.WriteLine($"{first} | {second}");

			connectionsMade++;
			var hasBoth = networks
				.Any(n => n.Contains(first) && n.Contains(second));
			if (hasBoth)
			{
				// Do nothing, already connected
			}
			else
			{
				var existingNetworks = networks
				.Where(n => n.Contains(first) || n.Contains(second))
				.ToList();
				if (existingNetworks.Count == 2)
				{
					foreach (var n in existingNetworks[1])
					{
						existingNetworks[0].Add(n);
					}
					networks.Remove(existingNetworks[1]);
					if (networks.Count == 1)
					{
						return Task.FromResult(((long)first.X * (long)second.X).ToString());
					}
				}
				else
				{
					Console.WriteLine("Err");
				}

			}
		}

		return Task.FromResult("Nope");
	}
}