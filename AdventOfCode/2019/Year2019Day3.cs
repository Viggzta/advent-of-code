using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day3 : IDay
{
	private Dictionary<char, (int x, int y)> directionMap = new()
	{
		{ 'R', (1, 0) },
		{ 'L', (-1, 0) },
		{ 'U', (0, 1) },
		{ 'D', (0, -1) },
	};

	public Task<string> RunSolution1Async(IList<string> input)
	{
		var lines = input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList();

		var lineMovePos = new Dictionary<int, HashSet<(int x, int y)>>();
		foreach (var line in lines.Index())
		{
			var currentMovePos = new HashSet<(int x, int y)>();
			var instructions = line.Item
				.Split(',')
				.Select(a => (directionMap[a[0]], int.Parse(a[1..])))
				.ToList();
			(int x, int y) currentPos = (0, 0);
			foreach (((int x, int y) direction, int repeat) in instructions)
			{
				for (int i = 0; i < repeat; i++)
				{
					currentPos = (currentPos.x + direction.x, currentPos.y + direction.y);
					currentMovePos.Add(currentPos);
				}
			}

			lineMovePos.Add(line.Index, currentMovePos);
		}

		var result = lineMovePos
			.SelectMany(a => a.Value.ToList())
			.GroupBy(a => a)
			.Where(a => a.Count() > 1)
			.Select(k => k.Key)
			.Select(a => ManhattanDistance((0, 0), a))
			.Min();

		return Task.FromResult(result.ToString());
	}

	private int ManhattanDistance((int x, int y) a, (int x, int y) b)
	{
		return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var lines = input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList();

		var lineMovePos = new Dictionary<int, HashSet<(int x, int y)>>();
		foreach (var line in lines.Index())
		{
			var currentMovePos = new HashSet<(int x, int y)>();
			var instructions = line.Item
				.Split(',')
				.Select(a => (directionMap[a[0]], int.Parse(a[1..])))
				.ToList();
			(int x, int y) currentPos = (0, 0);
			foreach (((int x, int y) direction, int repeat) in instructions)
			{
				for (int i = 0; i < repeat; i++)
				{
					currentPos = (currentPos.x + direction.x, currentPos.y + direction.y);
					currentMovePos.Add(currentPos);
				}
			}

			lineMovePos.Add(line.Index, currentMovePos);
		}

		var intersections = lineMovePos
			.SelectMany(a => a.Value.ToList())
			.GroupBy(a => a)
			.Where(a => a.Count() > 1)
			.Select(k => k.Key)
			.ToList();

		var intersectionSteps = intersections.ToDictionary(a => a, _ => new[] {-1L, -1L});
		foreach (var line in lines.Index())
		{
			var instructions = line.Item
				.Split(',')
				.Select(a => (directionMap[a[0]], int.Parse(a[1..])))
				.ToList();
			(int x, int y) currentPos = (0, 0);
			long stepsMoved = 0;
			foreach (((int x, int y) direction, int repeat) in instructions)
			{
				for (int i = 0; i < repeat; i++)
				{
					stepsMoved++;
					currentPos = (currentPos.x + direction.x, currentPos.y + direction.y);
					if (intersectionSteps.ContainsKey(currentPos))
					{
						intersectionSteps[currentPos][line.Index] =
							intersectionSteps[currentPos][line.Index] == -1
							? stepsMoved
							: intersectionSteps[currentPos][line.Index];
					}
				}
			}
		}

		var result = intersectionSteps.Select(a => a.Value.Sum()).Min();
		return Task.FromResult(result.ToString());
	}
}