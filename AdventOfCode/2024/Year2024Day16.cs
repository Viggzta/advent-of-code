using System.Collections;
using AdventOfCode.Utility;
using System.Collections.Generic;

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

		return (ReconstructPath1(cameFrom, end), dist[end]);
	}

	private List<(int x, int y)> ReconstructPath1(
		Dictionary<(int x, int y), ((int x, int y) p, (int x, int y) dir)> cameFrom,
		(int x, int y) current)
	{
		var currentInternal = current;
		var path = new System.Collections.Generic.List<(int x, int y)>();
		path.Add(currentInternal);
		while (cameFrom.TryGetValue(currentInternal, out var cameFromLocal))
		{
			currentInternal = cameFromLocal.p;
			path.Add(currentInternal);
		}

		return path;
	}

	private List<((int x, int y) p, (int x, int y) dir)> ReconstructPath(
		Dictionary<((int x, int y) p, (int x, int y) dir), ((int x, int y) p, (int x, int y) dir)> cameFrom,
		((int x, int y) p, (int x, int y) dir) current)
	{
		var path = new List<((int x, int y) p, (int x, int y) dir)>();
		path.Add(current);
		while (cameFrom.TryGetValue(current, out var cameFromLocal))
		{
			current = cameFromLocal;
			path.Add(current);
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
		var paths = Djikstra2(
			(start, facingDir),
			end,
			walkables,
			costToEnd);

		Map2D.PrintMap(
			'.',
			('X', paths),
			('\u2588', walls));

		return Task.FromResult(paths.Count.ToString());
	}

	private HashSet<(int x, int y)> Djikstra2(
		((int x, int y) p, (int x, int y) dir) start,
		(int x, int y) end,
		HashSet<(int x, int y)> walkables,
		int maxCost,
		int initialCost = 0)
	{
		var dist = new Dictionary<((int x, int y) p, (int x, int y) dir), int>();
		var cameFrom = new Dictionary<
			((int x, int y) p, (int x, int y) dir),
			List<((int x, int y) p, (int x, int y) dir)>>();

		HashSet<((int x, int y) p, (int x, int y) facingDir)> openSet =
			new HashSet<((int x, int y) p, (int x, int y) facingDir)>();
		dist.Add(start, initialCost);
		openSet.Add(start);

		while (openSet.Count != 0)
		{
			var current = openSet
				.OrderBy(a => dist.GetValueOrDefault(a, int.MaxValue))
				.First();
			openSet.Remove(current);

			var neighbours = GetNeighbours(current, walkables);
			foreach (var n in neighbours)
			{
				var tentativeCost = dist[current] + 1 + 1000 * n.rotations;
				if (tentativeCost > maxCost) continue;

				var nNormal = (n.p, n.dir);
				if (!dist.ContainsKey(nNormal) || tentativeCost <= dist[nNormal])
				{
					if (!cameFrom.ContainsKey(nNormal)) cameFrom[nNormal] = new();
					if (!cameFrom[nNormal].Contains(current) ||
							dist.GetValueOrDefault(nNormal, Int32.MaxValue) > tentativeCost)
					{
						dist[nNormal] = tentativeCost;
						openSet.Add(nNormal);
					}

					cameFrom[nNormal].Add(current);
				}
			}
		}
		List<((int x, int y) p, (int x, int y) dir)> sources = AllDirections
			.Select(a => (end, a))
			.ToList();
		HashSet<(int x, int y)> megaPath = ReconstructPath2(sources, cameFrom);
		return megaPath;
	}

	private HashSet<(int x, int y)> ReconstructPath2(
		List<((int x, int y) p, (int x, int y) dir)> sources,
		Dictionary<(
			(int x, int y) p, (int x, int y) dir),
			List<((int x, int y) p, (int x, int y) dir)>> cameFrom)
	{
		var path = new HashSet<(int, int)>();
		foreach (var source in sources)
		{
			path.Add(source.p);
			if (cameFrom.TryGetValue(source, out var value))
			{
				var internalPaths = ReconstructPath2(value, cameFrom);
				foreach (var internalPath in internalPaths)
				{
					path.Add(internalPath);
				}
			}
		}

		return path;
	}

	private static readonly List<(int x, int y)> AllDirections =
	[
		(1, 0),
		(-1, 0),
		(0, 1),
		(0, -1)
	];
}