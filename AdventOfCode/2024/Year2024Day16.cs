using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day16 : IDay
{
	private ((int x, int y) start, (int x, int y) end, HashSet<(int x, int y)> walls, HashSet<(int x, int y)> walkables)
		ParseInput(List<string> input)
	{
		(int x, int y) start = (-1, -1);
		(int x, int y) end = (-1, -1);
		HashSet<(int x, int y)> walls = new();
		HashSet<(int x, int y)> walkables = new();

		foreach (var row in input.Index())
		{
			foreach (var cell in row.Item.Index())
			{
				var currentCoordinate = (cell.Index, row.Index);
				switch (cell.Item)
				{
					case '#':
						walls.Add(currentCoordinate);
						break;
					case '.':
						walkables.Add(currentCoordinate);
						break;
					case 'S':
						start = currentCoordinate;
						break;
					case 'E':
						end = currentCoordinate;
						break;
				}
			}
		}

		walkables.Add(start);
		walkables.Add(end);

		return (start, end, walls, walkables);
	}
	public Task<string> RunSolution1Async(IList<string> input)
	{
			((int x, int y) start, (int x, int y) end, HashSet<(int x, int y)> walls, HashSet<(int x, int y)> walkables) =
				ParseInput(input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList());

		(int x, int y) facingDir = (1, 0);

		(List<(int x, int y)> path, int costToEnd) = Djikstra(start, facingDir, end, walkables);

		/*
		var pathMap = path.ToHashSet();
		Map2D.PrintMap(
			'.',
			('X', pathMap),
			('\u2588', walls));
			*/

		return Task.FromResult(costToEnd.ToString());
	}

	private (List<(int x, int y)>, int costToEnd) Djikstra(
		(int x, int y) start,
		(int x, int y) facingDir,
		(int x, int y) end,
		HashSet<(int x, int y)> walkables)
	{
		var dist = new Dictionary<(int x, int y), int>();
		var cameFrom = new Dictionary<(int x, int y), ((int x, int y) p, (int x, int y) dir)>();

		HashSet<((int x, int y) p, (int x, int y) facingDir)> openSet = new HashSet<((int x, int y) p, (int x, int y) facingDir)>();
		dist.Add(start, 0);
		openSet.Add((start, facingDir));

		while (openSet.Count != 0)
		{
			var current = openSet
				.OrderBy(c => dist.GetValueOrDefault(c.p, int.MaxValue))
				.First();
			openSet.Remove(current);

			var validNeighbours = GetNeighbours(
				current, walkables);
			foreach (((int x, int y) p, (int x, int y) dir, int rotations) n in validNeighbours)
			{
				var distToNeighbour = dist[current.p] + 1 + 1000 * n.rotations;
				if (!dist.ContainsKey(n.p) || distToNeighbour < dist[n.p])
				{
					dist[n.p] = distToNeighbour;
					cameFrom[n.p] = current;
					openSet.Add((n.p, n.dir));
				}
			}
		}

		return (ReconstructPath(cameFrom, end), dist[end]);
	}

	private List<(int x, int y)> ReconstructPath(
		Dictionary<(int x, int y), ((int x, int y) p, (int x, int y) dir)> cameFrom,
		(int x, int y) current)
	{
		var currentInternal = current;
		var path = new List<(int x, int y)>();
		path.Add(currentInternal);
		while (cameFrom.TryGetValue(currentInternal, out var cameFromLocal))
		{
			currentInternal = cameFromLocal.p;
			path.Add(currentInternal);
		}

		path.Reverse();

		return path;
	}

	private List<((int x, int y) p, (int x, int y) dir, int rotations)> GetNeighbours(
		((int x, int y) p, (int x, int y) dir) current,
		HashSet<(int x, int y)> walkables)
	{
		var p = current.p;
		var dir = current.dir;
		(int x, int y) rotatedDir = (dir.y, dir.x);
		List<((int x, int y) p, (int x, int y) dir, int rotations)> neighbours =
		[
			((p.x + dir.x, p.y + dir.y), (dir), 0),
			((p.x + rotatedDir.x, p.y + rotatedDir.y), (rotatedDir), 1),
			((p.x - rotatedDir.x, p.y - rotatedDir.y), (-rotatedDir.x, -rotatedDir.y), 1)
		];

		return neighbours.Where(b => walkables.Contains(b.p)).ToList();
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
			((int x, int y) start, (int x, int y) end, HashSet<(int x, int y)> walls, HashSet<(int x, int y)> walkables) =
				ParseInput(input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList());

		(int x, int y) facingDir = (1, 0);

		(_, int costToEnd) = Djikstra(start, facingDir, end, walkables);
		List<List<(int x, int y)>> paths = Djikstra2(start, facingDir, end, walkables, costToEnd);

		var flattened = paths
			.SelectMany(x => x)
			.ToHashSet();

		Map2D.PrintMap(
			'.',
			('X', flattened),
			('\u2588', walls));

		return Task.FromResult(flattened.Count.ToString());
	}

	private List<List<(int x, int y)>> Djikstra2(
		(int x, int y) start,
		(int x, int y) facingDir,
		(int x, int y) end,
		HashSet<(int x, int y)> walkables,
		int maxCost,
		int initialCost = 0)
	{
		var dist = new Dictionary<(int x, int y), int>();
		var cameFrom = new Dictionary<(int x, int y), ((int x, int y) p, (int x, int y) dir)>();

		HashSet<((int x, int y) p, (int x, int y) facingDir)> openSet = new HashSet<((int x, int y) p, (int x, int y) facingDir)>();
		dist.Add(start, initialCost);
		openSet.Add((start, facingDir));

		while (openSet.Count != 0)
		{
			var current = openSet
				.OrderBy(c => dist.GetValueOrDefault(c.p, int.MaxValue))
				.First();
			openSet.Remove(current);

			var validNeighbours = GetNeighbours(
				current, walkables);
			if (validNeighbours.Count > 1)
			{
				List<List<(int x, int y)>> path = new();
				bool anyPath = false;
				foreach (var n in validNeighbours)
				{
					var distToNeighbour = dist[current.p] + 1 + 1000 * n.rotations;
					var pathsToEnd = Djikstra2(n.p, n.dir, end, walkables, maxCost, distToNeighbour);
					foreach (var possiblePath in pathsToEnd)
					{
						if (possiblePath.Count != 0)
						{
							path.Add(possiblePath);
							anyPath = true;
						}
					}
				}

				if (anyPath)
				{
					var lPath = ReconstructPath(cameFrom, current.p);
					path.Add(lPath);
				}

				return path;
			}
			else
			{
				foreach (((int x, int y) p, (int x, int y) dir, int rotations) n in validNeighbours)
				{
					var distToNeighbour = dist[current.p] + 1 + 1000 * n.rotations;
					if (distToNeighbour <= maxCost && (!dist.ContainsKey(n.p) || distToNeighbour < dist[n.p]))
					{
						dist[n.p] = distToNeighbour;
						cameFrom[n.p] = current;
						openSet.Add((n.p, n.dir));
					}
				}
			}
		}

		if (!dist.TryGetValue(end, out int value) || value != maxCost)
		{
			return [];
		}

		return new List<List<(int x, int y)>>
		{
			ReconstructPath(cameFrom, end)
		};
	}
}