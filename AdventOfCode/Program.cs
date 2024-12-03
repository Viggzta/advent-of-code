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
            "xmul(2,4)&mul[3,7]!^don't()don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))",
            ""
        };
        */
        
        var input = await InputFetcher.GetAllInputLinesAsync(3, 2024);
        var day = new Year2024Day3();
        var ans = await day.RunSolution2Async(input);
        Console.WriteLine(ans);
    }
}