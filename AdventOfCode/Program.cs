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
            "190: 10 19",
            "3267: 81 40 27",
            "83: 17 5",
            "156: 15 6",
            "7290: 6 8 6 15",
            "161011: 16 10 13",
            "192: 17 8 14",
            "21037: 9 7 18 13",
            "292: 11 6 16 20",
            "", 
        };
        */
        
        var input = await InputFetcher.GetAllInputLinesAsync(7, 2024);
        var day = new Year2024Day7();
        var ans = await day.RunSolution2Async(input);
        Console.WriteLine(ans);
    }
}