using AdventOfCode._2019.IntComputer;
using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day9 : IDay
{
	public async Task<string> RunSolution1Async(IList<string> input)
	{
		var program = input.First().Split(',').Select(long.Parse).ToList();
		var computer = new Intcomputer(program);
		await computer.RunAsync();

		return string.Join(',', computer.GetOutputBuffer());
	}

	public async Task<string> RunSolution2Async(IList<string> input)
	{
		var program = input.First().Split(',').Select(long.Parse).ToList();
		var computer = new Intcomputer(program);
		await computer.RunAsync();

		return string.Join(',', computer.GetOutputBuffer());
	}
}