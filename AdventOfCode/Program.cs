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
            "7 6 4 2 1",
            "1 2 7 8 9",
            "9 7 6 2 1",
            "1 3 2 4 5",
            "8 6 4 4 1",
            "1 3 6 7 9",
            ""
        };
        */
        
        var input = await InputFetcher.GetAllInputLinesAsync(2, 2024);
        var day = new Year2024Day2();
        var ans = await day.RunSolution2Async(input);
        Console.WriteLine(ans);
    }
}