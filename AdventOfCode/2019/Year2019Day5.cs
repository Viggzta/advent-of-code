using AdventOfCode._2019.IntComputer;
using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day5 : IDay
{
	public async Task<string> RunSolution1Async(IList<string> input)
	{
		var code = input.First().Split(',').Select(long.Parse).ToList();
		Intcomputer intComputer = new(code);
		await intComputer.RunAsync();
		Console.WriteLine(string.Join("",intComputer.GetOutputBuffer()));

		return "";
	}

	public async Task<string> RunSolution2Async(IList<string> input)
	{
		var code = input.First().Split(',').Select(long.Parse).ToList();
		Intcomputer intComputer = new(code);
		await intComputer.RunAsync();
		Console.WriteLine(string.Join("",intComputer.GetOutputBuffer()));

		return "";
	}
}