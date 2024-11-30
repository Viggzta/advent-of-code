using AdventOfCode._2023;
using AdventOfCode.Utility;

namespace AdventOfCode;

class Program
{
    static async Task Main(string[] args)
    {
        var input = await InputFetcher.GetAllInputLinesAsync(1, 2023);
        var day = new Year2023Day1();
        var ans = await day.RunSolution1Async(input);
        Console.WriteLine(ans);
    }
}