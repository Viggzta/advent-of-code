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

		var srvFft = GetCount2(nodesConnectedToOut, nodesConnectedToOut["svr"], "fft");
		var srvDac = GetCount2(nodesConnectedToOut, nodesConnectedToOut["svr"], "dac");
		var fftOut = GetCount2(nodesConnectedToOut, nodesConnectedToOut["fft"], "out");
		var dacOut = GetCount2(nodesConnectedToOut, nodesConnectedToOut["dac"], "out");
		return Task.FromResult(srvFft.ToString());
	}

	private long GetCount2(
		Dictionary<string, List<string>> nodesConnectedToOut,
		List<string> list,
		string targetNode)
	{
		long sum = 0;
		foreach (var l in list)
		{
			if (l == targetNode)
			{
				sum += 1;
				continue;
			}
			var temp = nodesConnectedToOut[l];
			sum += GetCount2(nodesConnectedToOut, temp, targetNode);
		}

		return sum;
	}
}