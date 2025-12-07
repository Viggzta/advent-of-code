using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day7 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		List<(int X, int Y, char Item)> items = input
			.Index()
			.SelectMany(
				y => y.Item
					.Index()
					.Select(x => (x.Index, y.Index, x.Item)))
			.ToList();

		var start = items.First(x => x.Item == 'S');
		var splitters = items
			.Where(x => x.Item == '^')
			.Select(x => (x.X, x.Y))
			.OrderBy(x => x.Y)
			.ToList();
		var splits = 0;

		var beams = new Stack<(int X, int Y)>();
		beams.Push((start.X, start.Y));
		var seenBeams = new HashSet<(int X, int Y)>();

		while (beams.Count > 0)
		{
			var beam = beams.Pop();
			seenBeams.Add(beam);
			var nextSplit = splitters
				.FirstOrDefault(x => x.X == beam.X && x.Y > beam.Y);

			if (nextSplit != default((int, int)))
			{
				var a = (nextSplit.X + 1, nextSplit.Y);
				var b = (nextSplit.X - 1, nextSplit.Y);
				var didSplit = false;
				if (!seenBeams.Contains(a))
				{
					beams.Push(a);
					didSplit = true;
				}

				if (!seenBeams.Contains(b))
				{
					beams.Push(b);
					didSplit = true;
				}

				if (didSplit) splits++;
			}
		}

		return Task.FromResult(splits.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		List<(int X, int Y, char Item)> items = input
			.Index()
			.SelectMany(
				y => y.Item
					.Index()
					.Select(x => (x.Index, y.Index, x.Item)))
			.ToList();

		var start = items.First(x => x.Item == 'S');
		List<(int X, int Y)> splitters = items
			.Where(x => x.Item == '^')
			.Select(x => (x.X, x.Y))
			.OrderBy(x => x.Y)
			.ToList();
		Dictionary<(int X, int Y), long> memoized = new();
		var timelines = GetTimelines((start.X, start.Y), splitters, memoized);

		return Task.FromResult(timelines.ToString());
	}

	private long GetTimelines(
		(int X, int Y) beam,
		List<(int X, int Y)> splitters,
		Dictionary<(int X, int Y), long> mem)
	{
			if (mem.TryGetValue(beam, out var timelines))
			{
				return timelines;
			}

			var nextSplit = splitters
				.FirstOrDefault(x => x.X == beam.X && x.Y > beam.Y);

			if (nextSplit != default((int, int)))
			{
				var a = (nextSplit.X + 1, nextSplit.Y);
				var b = (nextSplit.X - 1, nextSplit.Y);
				var beamMem = GetTimelines(a, splitters, mem) + GetTimelines(b, splitters, mem);
				mem.Add(beam, beamMem);
				return beamMem;
			}

			mem.Add(beam, 1);
			return 1;
	}
}