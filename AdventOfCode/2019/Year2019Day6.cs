using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day6 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		List<(string orbits, string objectCode)> orbitInput = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Split(')'))
			.Select(x => (x[0], x[1]))
			.ToList();

		var first = orbitInput.First(o => o.orbits == "COM");
		var openSet = new HashSet<(string orbits, string objectCode)>();
		openSet.Add(first);

		Dictionary<string, int> orbitCount = new();
		orbitCount.Add("COM", 0);
		while (openSet.Any())
		{
			var current = openSet.First();
			openSet.Remove(current);
			orbitCount.Add(current.objectCode, orbitCount[current.orbits]+1);
			orbitInput
				.Where(o => o.orbits == current.objectCode)
				.ToList()
				.ForEach(o => openSet.Add(o));
		}

		var result = orbitCount.Sum(o => o.Value);
		return Task.FromResult(result.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		List<(string orbits, string objectCode)> orbitInput = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Split(')'))
			.Select(x => (x[0], x[1]))
			.ToList();

		var first = orbitInput.First(o => o.objectCode == "YOU");
		var openSet = new HashSet<(string orbits, string objectCode)>();
		openSet.Add(first);

		Dictionary<string, int> orbitCount = new();
		Dictionary<string, string> cameFrom = new();
		orbitCount.Add("YOU", 0);
		cameFrom.Add("YOU", "YOU"); // Hack to always have a parent
		while (openSet.Any())
		{
			var current = openSet.First();
			openSet.Remove(current);
			orbitCount.TryAdd(current.objectCode, orbitCount[cameFrom[current.objectCode]]+1);
			orbitInput
				.Where(o =>
					(o.orbits == current.objectCode ||
					o.objectCode == current.orbits) &&
					!cameFrom.ContainsKey(o.objectCode))
				.ToList()
				.ForEach(o =>
				{
					openSet.Add(o);
					cameFrom.Add(o.objectCode, current.objectCode);
				});
		}

		var result = orbitCount["SAN"] - 2; // Remove 2 to not count YOU and SAN
		return Task.FromResult(result.ToString());
	}
}