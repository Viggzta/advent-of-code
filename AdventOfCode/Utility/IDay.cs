namespace AdventOfCode.Utility;

public interface IDay
{
    int DayNumber { get; }

    Task<string> RunSolution1Async(IList<string> input);
    
    Task<string> RunSolution2Async(IList<string> input);
}