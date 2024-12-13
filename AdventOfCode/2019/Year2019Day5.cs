using AdventOfCode._2019.IntComputer;
using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day5 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var code = input.First().Split(',').Select(int.Parse).ToList();
		Intcomputer intComputer = new(code);
		intComputer.Run();
		Console.WriteLine(string.Join("",intComputer.GetOutputBuffer()));

		return Task.FromResult("");
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var code = input.First().Split(',').Select(int.Parse).ToList();
		Intcomputer intComputer = new(code);
		intComputer.Run();
		Console.WriteLine(string.Join("",intComputer.GetOutputBuffer()));

		return Task.FromResult("");
	}
}