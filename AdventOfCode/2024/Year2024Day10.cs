using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day10 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var lines = input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList();
		Dictionary<(int x, int y), int> elevationMap = lines
			.Index()
			.SelectMany(
				a => a.Item
					.Index()
					.Select(b => (a.Index, b.Index, int.Parse(b.Item.ToString()))))
			.ToDictionary(a => (a.Item2, a.Item1), a => a.Item3);

		var startPositions = elevationMap
			.Where(kvp => kvp.Value == 0)
			.Select(x => (x.Key.x, x.Key.y, x.Value))
			.ToList();
		var allTrailHeads = new List<int>();
		foreach ((int x, int y, int value) startPos in startPositions)
		{
			var trailHeads = 0;
			var openSet = new HashSet<(int x, int y, int value)>();
			var exploredSet = new HashSet<(int x, int y, int value)>();
			openSet.Add(startPos);
			while (openSet.Count > 0)
			{
				var current = openSet.First();
				openSet.Remove(current);

				if (current.value == 9)
				{
					trailHeads++;
				}

				var neighbours = GetNeighbours(
					current,
					elevationMap,
					x => x == current.value + 1)
					.Except(exploredSet);

				foreach (var n in neighbours)
				{
					openSet.Add((n.x, n.y, n.value));
				}
				exploredSet.Add(current);
			}
			allTrailHeads.Add(trailHeads);
		}

		var result = allTrailHeads.Sum();

		return Task.FromResult(result.ToString());
	}

	private List<(int x, int y, int value)> GetNeighbours(
		(int x, int y, int value) position,
		Dictionary<(int x, int y), int> map,
		Func<int, bool> condition)
	{
		return map
			.Where(pos =>
				(pos.Key.x == position.x + 1 && pos.Key.y == position.y) ||
				(pos.Key.x == position.x - 1 && pos.Key.y == position.y) ||
				(pos.Key.x == position.x && pos.Key.y + 1 == position.y) ||
				(pos.Key.x == position.x && pos.Key.y - 1 == position.y))
			.Where(x => condition(x.Value))
			.Select(x => (x.Key.x, x.Key.y, x.Value))
			.ToList();
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var lines = input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList();
		Dictionary<(int x, int y), int> elevationMap = lines
			.Index()
			.SelectMany(
				a => a.Item
					.Index()
					.Select(b =>
						(a.Index, b.Index, b.Item == '.' ? -1 : int.Parse(b.Item.ToString()))))
			.ToDictionary(a => (a.Item2, a.Item1), a => a.Item3);

		var startPositions = elevationMap
			.Where(kvp => kvp.Value == 0)
			.Select(x => (x.Key.x, x.Key.y, x.Value))
			.ToList();
		var allTrailHeads = new List<int>();
		foreach ((int x, int y, int value) startPos in startPositions)
		{
			var trailHeads = 0;
			var openSet = new List<(int x, int y, int value)>();
			openSet.Add(startPos);
			while (openSet.Count > 0)
			{
				var current = openSet.First();
				openSet.RemoveAt(0);

				if (current.value == 9)
				{
					trailHeads++;
				}

				var neighbours = GetNeighbours(
					current,
					elevationMap,
					x => x == current.value + 1);

				foreach (var n in neighbours)
				{
					openSet.Add((n.x, n.y, n.value));
				}
			}
			allTrailHeads.Add(trailHeads);
		}

		var result = allTrailHeads.Sum();

		return Task.FromResult(result.ToString());
	}
}