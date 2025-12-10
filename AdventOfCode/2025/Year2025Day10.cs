using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day10 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		var machines =
			ParseInput(input);

		var result = 0;
		foreach (var machine in machines)
		{
			var discoveredStates = new Dictionary<int, int>();
			var openSet = new Dictionary<int, int>();
			openSet.Add(0, 0);
			while (!discoveredStates.ContainsKey(machine.TargetLight))
			{
				foreach (var state in openSet.ToList())
				{
					openSet.Remove(state.Key);
					foreach (var button in machine.ButtonWirings)
					{
						var newState = state.Key ^ button;
						if (!discoveredStates.ContainsKey(newState))
						{
							discoveredStates.Add(newState, state.Value + 1);
							openSet.Add(newState, state.Value + 1);
						}
					}
				}
			}

			result += discoveredStates[machine.TargetLight];
		}

		return Task.FromResult(result.ToString());
	}

	private int ToNumber(List<bool> machineTargetLight)
	{
		var result = 0;
		foreach (var ml in machineTargetLight.Index().Where(x => x.Item))
		{
			result += 1 << (machineTargetLight.Count - 1 - ml.Index);
		}
		return result;
	}
	private int ToNumber(List<int> offsets, int length)
	{
		var result = 0;
		foreach (var offset in offsets)
		{
			result += 1 << (length - 1 - offset);
		}
		return result;
	}

	private List<(int TargetLight, List<int> ButtonWirings, List<int> JoltageRequirement, Dictionary<int, List<int>> ButtonDict)> ParseInput(
		IList<string> input)
	{
		return input
			.Select(x =>
			{
				var splits = x.Split(' ');
				var targetLightString = splits.First();
				var targetLightList = targetLightString
					.Substring(1, targetLightString.Length - 2)
					.Select(y => y == '#')
					.ToList();
				var targetLight = ToNumber(targetLightList);

				var buttonWirings1 = splits
					.Skip(1)
					.Take(splits.Length - 2)
					.Select(y => y
						.Substring(1, y.Length - 2)
						.Split(',')
						.Select(int.Parse)
						.ToList());
				var buttonWiringsDict = buttonWirings1
					.ToDictionary(
						y => ToNumber(y, targetLightList.Count),
						y => y);
				var buttonWirings = buttonWiringsDict.Keys.ToList();

				var joltageRequirementString = splits.Last();
				var joltageRequirement = joltageRequirementString
					.Substring(1, joltageRequirementString.Length - 2)
					.Split(',')
					.Select(int.Parse)
					.ToList();

				return (targetLight, buttonWirings, joltageRequirement, buttonWiringsDict);
			})
			.ToList();
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		var machines =
			ParseInput(input);

		var result = 0;
		foreach (var machine in machines)
		{
			var discoveredStates = new Dictionary<int, List<int>>();
			var openSet = new Dictionary<int, List<int>>();
			openSet.Add(0, machine.JoltageRequirement.Select(_ => 0).ToList());
			while (!discoveredStates.ContainsKey(machine.TargetLight))
			{
				foreach (var state in openSet.ToList())
				{
					openSet.Remove(state.Key);
					foreach (var button in machine.ButtonWirings)
					{
						var newState = state.Key ^ button;
						if (!discoveredStates.ContainsKey(newState))
						{
							var buttonNums = machine.ButtonDict[button];
							var values = state.Value.ToList();
							var newValue = values.Index().Select(x => x.Item + buttonNums[x.Index]).ToList();
							discoveredStates.Add(newState, newValue);
							if (!newValue.Index().Any(x => x.Item > machine.JoltageRequirement[x.Index]))
							{
								openSet.Add(newState, newValue);
							}
						}
					}
				}
			}

			//result += discoveredStates[machine.TargetLight];
			// we need to keep track of the count too
		}

		return Task.FromResult(result.ToString());
	}
}