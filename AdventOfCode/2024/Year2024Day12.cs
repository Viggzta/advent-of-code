using System.Diagnostics;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day12 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		List<(int x, int y, char areaCode)> map = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Index()
			.SelectMany(a => a.Item.Select((b, j) => ((j, a.Index), b)))
			.Select(a => (a.Item1.j, a.Item1.Index, a.b))
			.ToList();

		List<List<(int x, int y, char areaCode)>> regions = [];
		var exploredTiles = new HashSet<(int x, int y, char c)>();
		(int x, int y, char areaCode) currentNode = map.First();
		while (currentNode.areaCode != default(char))
		{
			var currentRegion = new List<(int x, int y, char areaCode)>();
			var openSet = new HashSet<(int x, int y, char areaCode)>();
			openSet.Add(currentNode);
			while (openSet.Any())
			{
				var internalCurrent = openSet.First();
				openSet.Remove(internalCurrent);
				exploredTiles.Add(internalCurrent);
				currentRegion.Add(internalCurrent);
				var neighbours = GetNeighbours(
					internalCurrent,
					map,
					i => i == internalCurrent.areaCode)
					.Except(exploredTiles);
				foreach (var n in neighbours)
				{
					openSet.Add(n);
				}
			}
			regions.Add(currentRegion);
			currentNode = map.Except(exploredTiles).FirstOrDefault();
		}

		List<long> costs = new List<long>();
		foreach (var region in regions)
		{
			var regionSimple = region.Select(a => (a.x, a.y)).ToList();
			var perimeterNodes = regionSimple
				.SelectMany(GetGrowOne)
				.GroupBy(a => a)
				.Where(g => !regionSimple.Contains(g.Key))
				.ToDictionary(g => g.Key, g => g.Count());
			var area = regionSimple.Count;
			var perimeter = perimeterNodes.Sum(x => x.Value);
			costs.Add(area * perimeter);
		}

		var result = costs.Sum();
		return Task.FromResult(result.ToString());
	}

	private List<(int x, int y)> GetGrowOne(
		(int x, int y) position)
	{
			return
			[
				(position.x + 1, position.y),
				(position.x - 1, position.y),
				(position.x, position.y + 1),
				(position.x, position.y - 1)
			];
	}

	private List<(int x, int y, char areaCode)> GetNeighbours(
		(int x, int y, char value) position,
		List<(int x, int y, char areaCode)> map,
		Func<char, bool> condition)
	{
		return map
			.Where(pos =>
				(pos.x == position.x + 1 && pos.y == position.y) ||
				(pos.x == position.x - 1 && pos.y == position.y) ||
				(pos.x == position.x && pos.y + 1 == position.y) ||
				(pos.x == position.x && pos.y - 1 == position.y))
			.Where(a => condition(a.areaCode))
			.ToList();
	}

	private List<(int x, int y)> GetLeftRightNeighbours(
		(int x, int y) position,
		List<(int x, int y)> map)
	{
		return map
			.Where(pos =>
				(pos.x == position.x + 1 && pos.y == position.y) ||
				(pos.x == position.x - 1 && pos.y == position.y))
			.ToList();
	}
	private List<(int x, int y)> GetUpDownNeighbours(
		(int x, int y) position,
		List<(int x, int y)> map)
	{
		return map
			.Where(pos =>
				(pos.x == position.x && pos.y + 1 == position.y) ||
				(pos.x == position.x && pos.y - 1 == position.y))
			.ToList();
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		List<(int x, int y, char areaCode)> map = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Index()
			.SelectMany(a => a.Item.Select((b, j) => ((j, a.Index), b)))
			.Select(a => (a.Item1.j, a.Item1.Index, a.b))
			.ToList();

		List<List<(int x, int y, char areaCode)>> regions = [];
		var exploredTiles = new HashSet<(int x, int y, char c)>();
		(int x, int y, char areaCode) currentNode = map.First();
		while (currentNode.areaCode != default(char))
		{
			var currentRegion = new List<(int x, int y, char areaCode)>();
			var openSet = new HashSet<(int x, int y, char areaCode)>();
			openSet.Add(currentNode);
			while (openSet.Any())
			{
				var internalCurrent = openSet.First();
				openSet.Remove(internalCurrent);
				exploredTiles.Add(internalCurrent);
				currentRegion.Add(internalCurrent);
				var neighbours = GetNeighbours(
					internalCurrent,
					map,
					i => i == internalCurrent.areaCode)
					.Except(exploredTiles);
				foreach (var n in neighbours)
				{
					openSet.Add(n);
				}
			}
			regions.Add(currentRegion);
			currentNode = map.Except(exploredTiles).FirstOrDefault();
		}

		long costs = 0;
		List<string> resultStrings = new List<string>();
		foreach (var region in regions)
		{
			var localRegionAreaCode = region.First().areaCode;
			var regionSimple = region.Select(a => (a.x, a.y)).ToList();
			Dictionary<(int x, int y), int> perimeterNodes = regionSimple
				.SelectMany(GetGrowOne)
				.GroupBy(a => a)
				.Where(g => !regionSimple.Contains(g.Key))
				.ToDictionary(g => g.Key, g => g.Count());
			List<(int x, int y)> perimeterList = perimeterNodes.Keys.ToList();

			var countedNodes = new Dictionary<(int x, int y), List<(int dirX, int dirY)>>(); // Value is times counted
			var sidesInternal = 0;
			foreach (var perimNode in perimeterNodes)
			{
				var directionsWithWall = GetGrowOne(perimNode.Key)
					.Where(regionSimple.Contains)
					.Select(temp => (temp.x - perimNode.Key.x, temp.y - perimNode.Key.y))
					.ToList();
				countedNodes.TryAdd(perimNode.Key, new List<(int dirX, int dirY)>(4));
				if (countedNodes.TryGetValue(perimNode.Key, out var timesCount)
						&& directionsWithWall.All(a => timesCount.Contains(a)))
				{
					// This node doesn't need counting
					continue;
				}

				foreach ((int x, int y) dir in directionsWithWall.Except(countedNodes[perimNode.Key]))
				{
					if (dir == (1, 0) || dir == (-1, 0))
					{
						// Do up down search
						var openSet = new HashSet<(int x, int y)>();
						openSet.Add(perimNode.Key);
						while (openSet.Any())
						{
							var current = openSet.First();
							openSet.Remove(current);
							countedNodes.TryAdd(current, new List<(int dirX, int dirY)>());
							countedNodes[current].Add(dir);
							var neighbours = GetUpDownNeighbours(
								current,
								perimeterList);
							foreach (var n in neighbours)
							{
								if (countedNodes.TryGetValue(n, out var exploredDirs)
										&& exploredDirs.Contains(dir))
								{
									continue;
								}

								(int x, int y) expectedWallLocation = (n.x + dir.x, n.y + dir.y); // New
								if (region.Contains((expectedWallLocation.x, expectedWallLocation.y, localRegionAreaCode)))
								{
									openSet.Add(n);
								}
							}
						}
					}
					else
					{
						// Do left right search
						var openSet = new HashSet<(int x, int y)>();
						openSet.Add(perimNode.Key);
						while (openSet.Any())
						{
							var current = openSet.First();
							openSet.Remove(current);
							countedNodes.TryAdd(current, new List<(int dirX, int dirY)>());
							countedNodes[current].Add(dir);
							var neighbours = GetLeftRightNeighbours(
								current,
								perimeterList);
							foreach (var n in neighbours)
							{
								if (countedNodes.TryGetValue(n, out var exploredDirs)
										&& exploredDirs.Contains(dir))
								{
									continue;
								}
								(int x, int y) expectedWallLocation = (n.x + dir.x, n.y + dir.y); // New
								if (region.Contains((expectedWallLocation.x, expectedWallLocation.y, localRegionAreaCode)))
								{
									openSet.Add(n);
								}
							}
						}
					}

					sidesInternal++;
				}
			}

			long sides = sidesInternal;
			long area = regionSimple.Count;
			long thisCost = area * sides;
			costs += thisCost;
			//resultStrings.Add($"Area: {area}, Sides: {sides}, Cost: {thisCost}");
		}
		//File.WriteAllLines("B:\\Downloads\\asdf\\output2.txt", resultStrings);

		var result = costs;
		return Task.FromResult(result.ToString());
	}
}