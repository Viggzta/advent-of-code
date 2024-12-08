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
            "............",
            "........0...",
            ".....0......",
            ".......0....",
            "....0.......",
            "......A.....",
            "............",
            "............",
            "........A...",
            ".........A..",
            "............",
            "............", 
            "", 
        };
        */
        
        var input = await InputFetcher.GetAllInputLinesAsync(8, 2024);
        var day = new Year2024Day8();
        var ans = await day.RunSolution2Async(input);
        Console.WriteLine(ans);
    }
}