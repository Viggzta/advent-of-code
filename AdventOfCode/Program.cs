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
            "....#.....",
            ".........#",
            "..........",
            "..#.......",
            ".......#..",
            "..........",
            ".#..^.....",
            "........#.",
            "#.........",
            "......#...", 
            ""
        };
        */
        
        var input = await InputFetcher.GetAllInputLinesAsync(6, 2024);
        var day = new Year2024Day6();
        var ans = await day.RunSolution2Async(input);
        Console.WriteLine(ans);
    }
}