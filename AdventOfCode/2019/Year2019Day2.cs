using AdventOfCode._2019.IntComputer;
using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day2 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var code = input.First().Split(',').Select(int.Parse).ToList();
		code[1] = 12;
		code[2] = 2;
		var intComputer = new Intcomputer(code);
		intComputer.Run();
		var result = intComputer.GetAtAddress(0);

		return Task.FromResult(result.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var code = input.First().Split(',').Select(int.Parse).ToList();

		var possibleCodes = new List<(List<int> code, int noun, int verb)>();
		for (var i = 0; i < 100; i++)
		{
			for (var j = 0; j < 100; j++)
			{
				var newCode = code.ToList();
				newCode[1] = i;
				newCode[2] = j;
				possibleCodes.Add((newCode, i, j));
			}
		}

		var resultTuple = possibleCodes
			.Select(t =>
			{
				var intComputer = new Intcomputer(t.code);
				intComputer.Run();
				return (intComputer.GetAtAddress(0), t.noun, t.verb);
			})
			.Single(t => t.Item1 == 19690720);
		var result = 100 * resultTuple.noun + resultTuple.verb;
		return Task.FromResult(result.ToString());
	}
}