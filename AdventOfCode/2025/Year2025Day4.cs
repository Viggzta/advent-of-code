using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day4 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		HashSet<(int x, int y)> rolls = new();
		foreach ((int y, string item) in input.Index())
		{
			foreach ((int x, char letter) in item.Index())
			{
				if (letter == '@') rolls.Add((x, y));
			}
		}

		var rollNeighbours = rolls
			.Count(roll => GetNeighbourCount(roll, rolls) < 4);

		return Task.FromResult(rollNeighbours.ToString());
	}

	private int GetNeighbourCount((int x, int y) rollKey, HashSet<(int x, int y)> rolls)
	{
		List<(int x, int y)> neighbours = new();
		for (int x = rollKey.x - 1; x <= rollKey.x + 1; x++)
		{
			for (int y = rollKey.y - 1; y <= rollKey.y + 1; y++)
			{
				if (x == rollKey.x && y == rollKey.y) continue;
				if (rolls.Contains((x, y)))
				{
					neighbours.Add((x, y));
				}
			}
		}

		return neighbours.Count;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		HashSet<(int x, int y)> rolls = new();
		foreach ((int y, string item) in input.Index())
		{
			foreach ((int x, char letter) in item.Index())
			{
				if (letter == '@') rolls.Add((x, y));
			}
		}

		var removedRollCount = 0;
		var preRemovedRollCount = 0;
		do
		{
			preRemovedRollCount = removedRollCount;
			var removableRolls = rolls
				.Where(roll => GetNeighbourCount(roll, rolls) < 4)
				.ToList();
			removedRollCount += removableRolls.Count();

			rolls.RemoveWhere(x => removableRolls.Contains(x));
		}
		while (removedRollCount != preRemovedRollCount);

		return Task.FromResult(removedRollCount.ToString());
	}
}