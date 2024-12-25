using System.Runtime.InteropServices;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day25 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var lockAndKeys = ParseLockAndKeys(input);
		List<(int pin1, int pin2, int pin3, int pin4, int pin5)> locks = lockAndKeys.locks;
		List<(int pin1, int pin2, int pin3, int pin4, int pin5)> keys = lockAndKeys.keys;

		var result = 0L;
		foreach (var l in locks)
		{
			foreach (var k in keys)
			{
				if (Fits(l, k))
					result++;
			}
		}

		return Task.FromResult(result.ToString());
	}

	private static bool Fits(
		(int pin1, int pin2, int pin3, int pin4, int pin5) lockIn,
		(int pin1, int pin2, int pin3, int pin4, int pin5) keyIn)
	{
		return
			lockIn.pin1 + keyIn.pin1 <= 5 &&
			lockIn.pin2 + keyIn.pin2 <= 5 &&
			lockIn.pin3 + keyIn.pin3 <= 5 &&
			lockIn.pin4 + keyIn.pin4 <= 5 &&
			lockIn.pin5 + keyIn.pin5 <= 5;
	}

	private static (
		List<(int pin1, int pin2, int pin3, int pin4, int pin5)> locks,
		List<(int pin1, int pin2, int pin3, int pin4, int pin5)> keys) ParseLockAndKeys(IList<string> input)
	{
		List<(int pin1, int pin2, int pin3, int pin4, int pin5)> keys = new();
		List<(int pin1, int pin2, int pin3, int pin4, int pin5)> locks = new();
		var takenRows = 0;
		while (takenRows < input.Count)
		{
			var lockKeyList = input
				.Skip(takenRows)
				.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
				.ToList();

			var pins = new Dictionary<int, int>();
			for (int y = 0; y < lockKeyList.Count; y++)
			{
				for (int x = 0; x < lockKeyList.First().Length; x++)
				{
					pins.TryAdd(x, 0);
					pins[x] += lockKeyList[y][x] == '#' ? 1 : 0;
				}
			}

			if (lockKeyList.First() == "#####")
			{
				// Lock
				locks.Add((pins[0] - 1, pins[1] - 1, pins[2] - 1, pins[3] - 1, pins[4] - 1));
			}
			else
			{
				keys.Add((pins[0] - 1, pins[1] - 1, pins[2] - 1, pins[3] - 1, pins[4] - 1));
			}

			takenRows += lockKeyList.Count + 1;
		}

		return (locks, keys);
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		return Task.FromResult("");
	}
}