using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day19 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var availableTowels = input.First().Split(", ").ToList();
		var desiredTowels = input
			.Skip(2)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.ToList();

		var desiredTowelsFoundCount = 0;
		foreach (var desiredTowel in desiredTowels)
		{
			string startingPoint = "";
			if (TryBuildTowel(
						startingPoint,
						desiredTowel,
						availableTowels,
						new(),
						out var towel))
			{
				desiredTowelsFoundCount++;
			}
		}

		return Task.FromResult(desiredTowelsFoundCount.ToString());
	}

	private bool TryBuildTowel(
		string startingPoint,
		string desiredTowel,
		List<string> availableTowels,
		HashSet<string> alreadyEvaluatedTowels,
		out string? towel)
	{
		var candidateTowel = availableTowels
			.Select(a => startingPoint + a)
			.Where(a =>
				a.Length <= desiredTowel.Length &&
				a == desiredTowel[..a.Length] &&
				!alreadyEvaluatedTowels.Contains(a))
			.ToList();

		foreach (var candidate in candidateTowel)
		{
			alreadyEvaluatedTowels.Add(candidate);
			if (candidate == desiredTowel)
			{
				towel = candidate;
				return true;
			}
			if (TryBuildTowel(
						candidate,
						desiredTowel,
						availableTowels,
						alreadyEvaluatedTowels,
						out towel))
			{
				towel = candidate;
				return true;
			}
		}

		towel = null;
		return false;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var availableTowels = input.First().Split(", ").ToList();
		var desiredTowels = input
			.Skip(2)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.ToList();


		desiredTowels = desiredTowels
			.Where(x => TryBuildTowel(
				"",
				x,
				availableTowels,
				new(),
				out _))
			.ToList();

		List<long> resultList = new List<long>();
		foreach (var desiredTowel in desiredTowels)
		{
			var possibleCombos = new Dictionary<string, long>();
			string startingPoint = "";
			var result = TryBuildTowel2(
				startingPoint,
				desiredTowel,
				availableTowels,
				possibleCombos);
				resultList.Add(result);
		}

		return Task.FromResult(resultList.Sum().ToString());
	}

	private long TryBuildTowel2(
		string startingPoint,
		string desiredTowel,
		List<string> availableTowels,
		Dictionary<string, long> alreadyEvaluatedTowels)
	{
		if (alreadyEvaluatedTowels.TryGetValue(startingPoint, out long result))
		{
			return result;
		}

		if (startingPoint == desiredTowel)
		{
			return 1;
		}

		var candidateTowel = availableTowels
			.Select(a => startingPoint + a)
			.Where(a =>
				a.Length <= desiredTowel.Length &&
				a == desiredTowel[..a.Length])
			.ToList();

		alreadyEvaluatedTowels[startingPoint] = candidateTowel.Sum(candidate =>
		{
			alreadyEvaluatedTowels[candidate] = TryBuildTowel2(
      				candidate,
      				desiredTowel,
      				availableTowels,
      				alreadyEvaluatedTowels);
			return alreadyEvaluatedTowels[candidate];
		});

		return alreadyEvaluatedTowels[startingPoint];
	}
}