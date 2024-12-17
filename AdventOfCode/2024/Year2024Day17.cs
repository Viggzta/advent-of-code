using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day17 : IDay
{
	public async Task<string> RunSolution1Async(IList<string> input)
	{
		var regLines = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => long.Parse(x.Split(':').Last().Trim()))
			.ToList();
		var prog = input
			.Skip(regLines.Count + 1)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.First()
			.Split(':')
			.Last()
			.Trim()
			.Split(',')
			.Select(long.Parse)
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
			.Select(x => long.Parse(x.Split(':').Last().Trim()))
			.ToList();
		var prog = input
			.Skip(regLines.Count + 1)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.First()
			.Split(':')
			.Last()
			.Trim()
			.Split(',')
			.Select(long.Parse)
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

		// int aReg = 294216978;
		// long aReg = 0b111111111111111111111111111111111111111111111111;
		var currentNum = 0;
		long maxNumber = 7;
		List<long> nums = Enumerable.Range(0, prog.Count).Select(_ => maxNumber).ToList();
		long aReg = nums.Select((n, i) => n << (i * 3)).Sum();
		//long aReg = 164541034753724;
		Console.WriteLine();
		while (true)
		{
			var comp = new ThreeBitComputer.ThreeBitComputer(
				prog,
				aReg,
				regLines[1],
				regLines[2]);
			await comp.RunAsync();
			var outString = string.Join(",", comp.GetOutputBuffer());
			/*
			if (outString == progAsString)
			{
				break;
			}
			*/

			Console.Clear();
			Console.WriteLine(
				"{0,24} | {1,40} | {2,48} | {3}",
				"RegA",
				"Program output",
				"RegA binary",
				"3bit section (0-7)");
			Console.WriteLine(
				"{0,24} | {1,35} ({2,1}) | {3,48} | {4}",
				aReg,
				outString,
				comp.GetOutputBuffer().Count,
				Convert.ToString(aReg, 2),
				string.Join(",", nums.Select((n, i) => (i == currentNum) ? $"_{n}_" : n.ToString())));
			Console.WriteLine("{0,62}", $"Target: {progAsString}");
			var key = Console.ReadKey();
			if (key.Key == ConsoleKey.UpArrow) nums[currentNum] = Math.Clamp(nums[currentNum] + 1, 0, maxNumber);
			if (key.Key == ConsoleKey.DownArrow) nums[currentNum] = Math.Clamp(nums[currentNum] - 1, 0, maxNumber);
			if (key.Key == ConsoleKey.LeftArrow) currentNum = Math.Clamp(currentNum - 1, 0, nums.Count-1);
			if (key.Key == ConsoleKey.RightArrow) currentNum = Math.Clamp(currentNum + 1, 0, nums.Count-1);
			aReg = nums.Select((n, i) => n << (i * 3)).Sum();
			//aReg--;
		}

		// Answer was 164541017976509
		return aReg.ToString();
	}
}