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
           "MMMSXXMASM",
           "MSAMXMSMSA",
           "AMXSXMAAMM",
           "MSAMASMSMX",
           "XMASAMXAMM",
           "XXAMMXXAMA",
           "SMSMSASXSS",
           "SAXAMASAAA",
           "MAMMMXMMMM",
           "MXMXAXMASX",
            ""
        };
        */
        
        var input = await InputFetcher.GetAllInputLinesAsync(4, 2024);
        var day = new Year2024Day4();
        var ans = await day.RunSolution2Async(input);
        Console.WriteLine(ans);
    }
}