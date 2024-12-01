using AdventOfCode._2023;
using AdventOfCode._2024;
using AdventOfCode.Utility;

namespace AdventOfCode;

class Program
{
    static async Task Main(string[] args)
    {
        /*
        var input = new List<string>()
        {
            "3   4",
            "4   3",
            "2   5",
            "1   3",
            "3   9",
            "3   3",
            ""
        };
        */
        
        var input = await InputFetcher.GetAllInputLinesAsync(1, 2024);
        var day = new Year2024Day1();
        var ans = await day.RunSolution2Async(input);
        Console.WriteLine(ans);
    }
}