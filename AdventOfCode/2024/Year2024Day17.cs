using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day17 : IDay
{
	public async Task<string> RunSolution1Async(IList<string> input)
	{
		var regLines = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => int.Parse(x.Split(':').Last().Trim()))
			.ToList();
		var prog = input
			.Skip(regLines.Count + 1)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.First()
			.Split(':')
			.Last()
			.Trim()
			.Split(',')
			.Select(int.Parse)
			.ToList();

		var comp = new ThreeBitComputer.ThreeBitComputer(
			prog,
			regLines[0],
			regLines[1],
			regLines[2]);

		await comp.RunAsync();
		var result = string.Join(",", comp.GetOutputBuffer());
		return result;
	}

	public async Task<string> RunSolution2Async(IList<string> input)
	{
		var regLines = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => int.Parse(x.Split(':').Last().Trim()))
			.ToList();
		var prog = input
			.Skip(regLines.Count + 1)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.First()
			.Split(':')
			.Last()
			.Trim()
			.Split(',')
			.Select(int.Parse)
			.ToList();
		var progAsString = string.Join(",", prog);

		/*
		do
    {
      b = a % 8;
      b ^= 1;
      c = a / 2 << b;
      b ^= 5;
      b ^= c;
      a = a / 8;
      OUTPUT => B % 8;
    }
    while (a != 0)
    */

		int aReg = 294216978;
		while (true)
		{
			var comp = new ThreeBitComputer.ThreeBitComputer(
				prog,
				aReg,
				regLines[1],
				regLines[2]);
			await comp.RunAsync();
			var outString = string.Join(",", comp.GetOutputBuffer());
			if (outString == progAsString)
			{
				break;
			}

			aReg++;
		}

		return aReg.ToString();
	}
}