using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day10 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		List<(int x, int y)> asteroids = input
			.Index()
			.SelectMany(y => y.Item.Index()
				.Where(x => x.Item == '#')
				.Select(x => (x.Index, y.Index)))
			.ToList();

		List<int> seeCounts = new();
		foreach (var asteroid in asteroids)
		{
			var asteroidDeltas = asteroids
				.Select(a => (a.x - asteroid.x, a.y - asteroid.y))
				.Where(a => a != (0, 0))
				.ToList();
			var seeCount = asteroidDeltas
				.GroupBy(Simplify)
				.ToList()
				.Count;
			seeCounts.Add(seeCount);
		}

		var seeCountMax = seeCounts.Max();
		return Task.FromResult(seeCountMax.ToString());
	}

	private (int x, int y) Simplify((int x, int y) a)
	{
		var d = Math.Abs(GreatestCommonDivisor(a.x, a.y));
		return (a.x / d, a.y / d);
	}

	private int GreatestCommonDivisor(int a, int b)
	{
		while (b != 0)
		{
			var t = b;
			b = a % b;
			a = t;
		}

		return a;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		List<(int x, int y)> asteroids = input
			.Index()
			.SelectMany(y => y.Item.Index()
				.Where(x => x.Item == '#')
				.Select(x => (x.Index, y.Index)))
			.ToList();

		List<(int seeCount, (int x, int y) asteroid)> seeCounts = new();
		foreach (var asteroid in asteroids)
		{
			var asteroidDeltas = asteroids
				.Select(a => (a.x - asteroid.x, a.y - asteroid.y))
				.Where(a => a != (0, 0))
				.ToList();
			var seeCount = asteroidDeltas
				.GroupBy(Simplify)
				.ToList()
				.Count;
			seeCounts.Add((seeCount, asteroid));
		}

		var stationLocation = seeCounts.MaxBy(a => a.seeCount).asteroid;
		var asteroidDeltas2 = asteroids
			.Select(a => (a.x - stationLocation.x, a.y - stationLocation.y))
			.Where(a => a != (0, 0))
			.ToList();
		var groupedByVector = asteroidDeltas2
			.GroupBy(Simplify)
			.Select(g => (GetAngle(g.Key), g.Key, g.ToHashSet()))
			.OrderBy(g => g.Item1)
			.ToList();

		var groupedByVector2 = groupedByVector.Select(g => g.Item3);

		var i = 200;
		(int x, int y) twoHundredAsteroid = (-1, -1);
		while (i > 0)
		{
			foreach (var hashSet in groupedByVector2)
			{
				if (hashSet.Count == 0) continue;
				(int x, int y) lowestDistance = hashSet.MinBy(a => Distance(a, (0, 0)));
				hashSet.Remove(lowestDistance);
				i--;
				if (i == 0)
				{
					twoHundredAsteroid = (stationLocation.x + lowestDistance.x, stationLocation.y + lowestDistance.y);
				}
			}
		}
		var result = twoHundredAsteroid.x * 100 + twoHundredAsteroid.y;
		return Task.FromResult(result.ToString());
	}

	private double Distance((int x, int y) a, (int x, int y) b)
	{
		var x = a.x - b.x;
		var y = a.y - b.y;
		return Math.Sqrt(x * x + y * y);
	}

	private double GetAngle((int x, int y) a)
	{
		return (Math.Atan2(a.y, a.x) * (180d / Math.PI) + 360d + 90d) % 360d;
	}
}