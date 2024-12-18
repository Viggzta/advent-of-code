using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day18 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var dataPoints = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Split(',').Select(int.Parse).ToList())
			.Select(x => (x[0], x[1]))
			.ToList();

		var corruptedBytes = dataPoints.Take(1024).ToHashSet();
		var start = (0, 0);
		var startHash = new HashSet<(int, int)> { start };
		var end = (70, 70);
		var endHash = new HashSet<(int, int)> { end };

		(List<(int x, int y)> pathList, int costToEnd, _) = Djikstra(
			start,
			end,
			corruptedBytes,
			(start, end));
		var pathHash = pathList.ToHashSet();

		/*
		Map2D.PrintMap(
			'.',
			('#', corruptedBytes),
			('O', pathHash),
			('S', startHash),
			('E', endHash));
		*/
		return Task.FromResult(costToEnd.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var dataPoints = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Split(',').Select(int.Parse).ToList())
			.Select(x => (x[0], x[1]))
			.ToList();

		//var takeAmount = 12;
		var takeAmount = 1024;
		var start = (0, 0);
		//var end = (6, 6);
		var end = (70, 70);
		var corruptedBytes = dataPoints.Take(takeAmount).ToHashSet();
		var startHash = new HashSet<(int, int)> { start };
		var endHash = new HashSet<(int, int)> { end };

		/*
		Map2D.PrintMap(
			'.',
			('#', corruptedBytesOriginal),
			('S', startHash),
			('E', endHash));
			*/

		var noPathPoint = (-1, -1);
		var evalPoints = dataPoints.Skip(takeAmount).ToList();
		foreach (var newPoint in evalPoints)
		{
			corruptedBytes.Add(newPoint);
			(List<(int x, int y)> pathList, int costToEnd, bool canReachEnd) = Djikstra(
				start,
				end,
				corruptedBytes,
				(start, end));

			if (!canReachEnd)
			{
				noPathPoint = newPoint;
				break;
			}
		}

		return Task.FromResult(noPathPoint.ToString());
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
}