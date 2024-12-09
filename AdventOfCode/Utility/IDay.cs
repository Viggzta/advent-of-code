namespace AdventOfCode.Utility;

public interface IDay
{
	Task<string> RunSolution1Async(IList<string> input);

	Task<string> RunSolution2Async(IList<string> input);
}