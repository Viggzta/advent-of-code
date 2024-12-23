using System.Diagnostics;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day23 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		Dictionary<string, List<string>> connections = new();
		foreach (var row in input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)))
		{
			var temp = row.Split('-');
			connections.TryAdd(temp[0], new List<string>());
			connections.TryAdd(temp[1], new List<string>());
			connections[temp[0]].Add(temp[1]);
			connections[temp[1]].Add(temp[0]);
		}

		var triPairs = new List<string>();
		foreach (var connection in connections.Keys)
		{
			GetAllSharedConnections(connection, connections, ref triPairs);
		}

		var resultPre = triPairs
			.Where(x => x.Split('-').Any(y => y[0] == 't'))
			.ToHashSet();
		var result = resultPre.Count.ToString();
		return Task.FromResult(result);
	}

	private void GetAllSharedConnections(
		string start,
		Dictionary<string, List<string>> connections,
		ref List<string> triPairs)
	{
		foreach (var startN in connections[start])
		{
			foreach (var nN in connections[startN])
			{
				if (!connections[nN].Contains(start)) continue;

				var tempVal = string.Join("-", new List<string>
				{
					start,
					startN,
					nN,
				}.Order());
				if (!triPairs.Contains(tempVal))
				{
					triPairs.Add(tempVal);
				}
			}
		}
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		Dictionary<string, HashSet<string>> connections = new();
		foreach (var row in input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)))
		{
			var temp = row.Split('-');
			connections.TryAdd(temp[0], new HashSet<string>());
			connections.TryAdd(temp[1], new HashSet<string>());
			connections[temp[0]].Add(temp[1]);
			connections[temp[1]].Add(temp[0]);
		}

		var maxCliques = new List<HashSet<string>>();
		BronKerbosch(
			new HashSet<string>(),
			new HashSet<string>(connections.Keys),
			new HashSet<string>(),
			ref connections,
			ref maxCliques);

		var largestClique = maxCliques
			.OrderByDescending(x => x.Count)
			.First();
		var result = string.Join(',', largestClique.ToList().Order());

		return Task.FromResult(result);
	}

	private void BronKerbosch(
		HashSet<string> currentClique,
		HashSet<string> potentialClique,
		HashSet<string> visited,
		ref Dictionary<string, HashSet<string>> connections,
		ref List<HashSet<string>> maxCliques)
	{
		if (!potentialClique.Any() && !visited.Any())
		{
			maxCliques.Add(currentClique);
			return;
		}

		foreach (var potInternal in potentialClique.ToList())
		{
			var neighbours = connections[potInternal];

			BronKerbosch(
				new HashSet<string>(currentClique) { potInternal },
				new HashSet<string>(potentialClique.Intersect(neighbours)),
				new HashSet<string>(visited.Intersect(neighbours)),
				ref connections,
				ref maxCliques);

			potentialClique.Remove(potInternal);
			visited.Remove(potInternal);
		}
	}
}