using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day1 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		var rotations = input
			.Select(x => new {Dir = x.Substring(0,1), Num = int.Parse(x.Substring(1))})
			.Select(x => x.Dir == "L" ? -x.Num : x.Num)
			.ToList();

		int dial = 50;
		int zeros = 0;
		foreach (var rotation in rotations)
		{
			dial += rotation;
			dial %= 100;

			if (dial == 0)
			{
				zeros++;
			}
		}

		return Task.FromResult(zeros.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		var rotations = input
			.Select(x => new {Dir = x.Substring(0,1), Num = int.Parse(x.Substring(1))})
			.Select(x => x.Dir == "L" ? -x.Num : x.Num)
			.ToList();

		int dial = 50;
		int zeros = 0;
		foreach (var rotation in rotations)
		{
			var prevDial = dial;
			dial += rotation;

			if (dial < 0)
			{
				zeros += -dial / 100;
				if (prevDial != 0)
				{
					zeros++;
				}
			}
			else if (dial == 0)
			{
				zeros++;
			}
			else
			{
				zeros += dial / 100;
			}

			dial %= FixedModulo(dial, 100);
		}

		return Task.FromResult(zeros.ToString());
	}

	private int FixedModulo(int x, int m)
	{
		return (x%m + m)%m;
	}
}