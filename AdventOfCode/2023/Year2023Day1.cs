using AdventOfCode.Utility;

namespace AdventOfCode._2023;

public class Year2023Day1 : IDay
{
    public int DayNumber => 1;
    
    public Task<string> RunSolution1Async(IList<string> input)
    {
        IList<string> inputLines = input.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        int sum = 0;
        foreach (string line in inputLines)
        {
            List<int> numbers = line.Where(x => char.IsNumber(x)).Select(x => int.Parse(x.ToString())).ToList();

            int firstNum = numbers.First();
            int lastNum = numbers.Last();

            int actualNum = int.Parse(firstNum.ToString() + lastNum.ToString());
            sum += actualNum;
        }

        return Task.FromResult(sum.ToString());
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        throw new NotImplementedException();
    }
}