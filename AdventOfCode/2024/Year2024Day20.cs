using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day20 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var inMap= input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.SelectMany((a, y) => a.Select(((b, x) => (b ,(x, y)))))
			.ToHashSet();

		var start = inMap.First(x => x.b == 'S').Item2;
		var end = inMap.First(x => x.b == 'E').Item2;
		var walls = inMap.Where(x => x.b == '#').Select(x => x.Item2).ToHashSet();
		var bounds = ((0, 0), (walls.Max(tuple => tuple.x), walls.Max(tuple => tuple.y)));

		(List<(int x, int y)>, int costToEnd, bool canReachEnd) djikstra1 = Djikstra(start, end, walls, bounds);

		var result = 0;
		foreach (var wall in walls)
		{
			var tempWalls = walls.ToHashSet();
			tempWalls.Remove(wall);

			var djikstra2 = Djikstra(start, end, tempWalls, bounds);
			var distDiff = djikstra1.costToEnd - djikstra2.costToEnd;
			if (distDiff >= 100)
			{
				result++;
			}
		}

		return Task.FromResult(result.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var inMap= input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.SelectMany((a, y) => a.Select(((b, x) => (b ,(x, y)))))
			.ToHashSet();

		var start = inMap.First(x => x.b == 'S').Item2;
		var walls = inMap.Where(x => x.b == '#').Select(x => x.Item2).ToHashSet();
		var distCostMap = GetDistCostMap(start, walls);

		var cheatSteps = 20;
		var availableCheats = new Dictionary<((int x, int y) from, (int x, int y) to), int>(); // Value is saved time
		foreach (var currentEval in distCostMap.Keys)
		{
			var currentCost = distCostMap[currentEval];

			var roadsInRangeCloserToEnd = distCostMap
				.Where(x =>
					ManhattanDistance(x.Key, currentEval) <= cheatSteps &&
					x.Value > currentCost)
				.ToDictionary(x => x.Key, x => x.Value);

			foreach (((int x, int y) n, int nDist) in roadsInRangeCloserToEnd)
			{
				var timeSaved = nDist - currentCost - ManhattanDistance(n, currentEval);
				availableCheats.Add((currentEval, n), timeSaved);
			}
		}

		var targetCheatTimeSavedMin = 100;
		var grouped = availableCheats
			.GroupBy(x => x.Value)
			.ToDictionary(x => x.Key, x => x.Count());
		var result = availableCheats
			.Count(x => x.Value >= targetCheatTimeSavedMin);
		return Task.FromResult(result.ToString());
	}

	private int ManhattanDistance((int x, int y) from, (int x, int y) to)
	{
		return Math.Abs(from.x - to.x) + Math.Abs(from.y - to.y);
	}

	private Dictionary<(int x, int y), int> GetDistCostMap(
		(int x, int y) start,
		HashSet<(int x, int y)> walls)
	{
		var distCostMap = new Dictionary<(int x, int y), int>();
		var openSet = new HashSet<(int x, int y)>();
		openSet.Add(start);
		distCostMap.Add(start, 0);

		while (openSet.Count > 0)
		{
			var current = openSet.First();
			openSet.Remove(current);

			var neighbours = GetNeighbours2(current, walls)
				.Where(n => !distCostMap.ContainsKey(n));
			foreach (var n in neighbours)
			{
				var nDist = distCostMap[current] + 1;
				distCostMap.Add(n, nDist);
				openSet.Add(n);
			}
		}

		return distCostMap;
	}

	private (List<(int x, int y)>, int costToEnd, bool canReachEnd) Djikstra(
		(int x, int y) start,
		(int x, int y) end,
		HashSet<(int x, int y)> walls,
		((int left, int top) lt, (int right, int down) rd) bounds)
	{
		var dist = new Dictionary<(int x, int y), int>();
		var cameFrom = new Dictionary<(int x, int y), (int x, int y)>();

		HashSet<(int x, int y)> openSet = [];
		dist.Add(start, 0);
		openSet.Add(start);

		while (openSet.Count != 0)
		{
			var current = openSet
				.OrderBy(c => dist.GetValueOrDefault(c, int.MaxValue))
				.First();
			openSet.Remove(current);

			var validNeighbours = GetNeighbours(current, walls, bounds);
			foreach ((int x, int y) p in validNeighbours)
			{
				var distToNeighbour = dist[current] + 1;
				if (!dist.ContainsKey(p) || distToNeighbour < dist[p])
				{
					dist[p] = distToNeighbour;
					cameFrom[p] = current;
					openSet.Add(p);
				}
			}
		}

		if (!dist.TryGetValue(end, out var costToEnd))
		{
			return (new(), int.MaxValue, false);
		}

		return (ReconstructPath(cameFrom, end), costToEnd, true);
	}

	private List<(int x, int y)> ReconstructPath(
		Dictionary<(int x, int y), (int x, int y)> cameFrom,
		(int x, int y) current)
	{
		var currentInternal = current;
		var path = new List<(int x, int y)>();
		path.Add(currentInternal);
		while (cameFrom.TryGetValue(currentInternal, out var cameFromLocal))
		{
			currentInternal = cameFromLocal;
			path.Add(currentInternal);
		}

		path.Reverse();

		return path;
	}

	private List<(int x, int y)> GetNeighbours(
		(int x, int y) current,
		HashSet<(int x, int y)> walls,
		((int left, int top) lt, (int right, int down) rd) bounds)
	{
		var p = current;
		List<(int x, int y)> neighbours =
		[
			(p.x + 1, p.y),
			(p.x - 1, p.y),
			(p.x, p.y + 1),
			(p.x, p.y - 1),
		];

		return neighbours
			.Where(b => !walls.Contains(b))
			.Where(p =>
				p.x >= bounds.lt.left &&
				p.x <= bounds.rd.right &&
				p.y >= bounds.lt.top &&
				p.y <= bounds.rd.down)
			.ToList();
	}

	private List<(int x, int y)> GetNeighbours2(
		(int x, int y) current,
		HashSet<(int x, int y)> walls)
	{
		var p = current;
		List<(int x, int y)> neighbours =
		[
			(p.x + 1, p.y),
			(p.x - 1, p.y),
			(p.x, p.y + 1),
			(p.x, p.y - 1),
		];

		return neighbours
			.Where(b => !walls.Contains(b))
			.ToList();
	}
}