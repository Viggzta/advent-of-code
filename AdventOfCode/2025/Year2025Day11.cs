using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day11 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		Dictionary<string, HashSet<string>> nodesAndConnections = new();
		foreach (var item in input)
		{
			var split = item.Split(": ");
			var split2 = split[1].Split(" ").ToHashSet();

			foreach (var n in split2)
			{
				if (!nodesAndConnections.ContainsKey(n))
				{
					var newHashSet = new HashSet<string>();
					newHashSet.Add(split[0]);
					nodesAndConnections.Add(n, newHashSet);
				}
				else
				{
					nodesAndConnections[n].Add(split[0]);
				}
			}
		}

		var nodesConnectedToOut = new Dictionary<string, List<string>>();
		nodesConnectedToOut.Add("out", new List<string>());
		var openSet = new Stack<string>();
		openSet.Push("out");
		while (openSet.Count != 0)
		{
			var node = openSet.Pop();
			if (node == "you") continue;

			var nodesToVisit = nodesAndConnections
				.TryGetValue(node, out var connection) ? connection : new HashSet<string>();
			foreach (var n in nodesToVisit)
			{
				if (nodesConnectedToOut.ContainsKey(n))
				{
					nodesConnectedToOut[n].Add(node);
				}
				else
				{
					nodesConnectedToOut.Add(n, new List<string>{node});
					openSet.Push(n);
				}
			}
		}

		var result = GetCount(nodesConnectedToOut, nodesConnectedToOut["you"]);
		return Task.FromResult(result.ToString());
	}

	private long GetCount(
		Dictionary<string, List<string>> nodesConnectedToOut,
		List<string> list)
	{
		long sum = 0;
		foreach (var l in list)
		{
			if (l == "out")
			{
				sum += 1;
				continue;
			}
			var temp = nodesConnectedToOut[l];
			sum += GetCount(nodesConnectedToOut, temp);
		}

		return sum;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		Dictionary<string, HashSet<string>> nodesAndConnections = new();
		foreach (var item in input)
		{
			var split = item.Split(": ");
			var split2 = split[1].Split(" ").ToHashSet();

			foreach (var n in split2)
			{
				if (!nodesAndConnections.ContainsKey(n))
				{
					var newHashSet = new HashSet<string>();
					newHashSet.Add(split[0]);
					nodesAndConnections.Add(n, newHashSet);
				}
				else
				{
					nodesAndConnections[n].Add(split[0]);
				}
			}
		}

		var nodesConnectedToOut = new Dictionary<string, List<string>>();
		nodesConnectedToOut.Add("out", new List<string>());
		var openSet = new Stack<string>();
		openSet.Push("out");
		while (openSet.Count != 0)
		{
			var node = openSet.Pop();
			if (node == "svr") continue;

			var nodesToVisit = nodesAndConnections
				.TryGetValue(node, out var connection) ? connection : new HashSet<string>();
			foreach (var n in nodesToVisit)
			{
				if (nodesConnectedToOut.ContainsKey(n))
				{
					nodesConnectedToOut[n].Add(node);
				}
				else
				{
					nodesConnectedToOut.Add(n, new List<string>{node});
					openSet.Push(n);
				}
			}
		}

		var result = GetCount2(nodesConnectedToOut, nodesConnectedToOut["svr"], false, false);
		return Task.FromResult(result.ToString());
	}

	private long GetCount2(
		Dictionary<string, List<string>> nodesConnectedToOut,
		List<string> list,
		bool hasVisitedDAC,
		bool hasVisitedFFT)
	{
		long sum = 0;
		foreach (var l in list)
		{
			if (l == "out")
			{
				if (hasVisitedDAC && hasVisitedFFT)
				{
					sum += 1;
				}
				continue;
			}
			var temp = nodesConnectedToOut[l];
			if (l == "dac")
			{
				sum += GetCount2(nodesConnectedToOut, temp, true, hasVisitedFFT);
			}
			else if (l == "fft")
			{
				sum += GetCount2(nodesConnectedToOut, temp, hasVisitedDAC, true);
			}
			else
			{
				sum += GetCount2(nodesConnectedToOut, temp, hasVisitedDAC, hasVisitedFFT);
			}
		}

		return sum;
	}
}