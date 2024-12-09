using AdventOfCode.Utility;

namespace AdventOfCode;

class Program
{
    static async Task Main(string[] args)
    {
        var manualInput = new List<string>
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
        
        var type = typeof(IDay);
        var dayTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p));
        
        var year = 2024;
        var day = 9;
        var part = 2;
        var inputType = InputType.Real;
        string? testExtra = null;
        
        var input = inputType switch
        {
            InputType.Manual => manualInput,
            InputType.Example => await InputFetcher.GetTestInputLinesAsync(day, year, testExtra),
            InputType.Real => await InputFetcher.GetAllInputLinesAsync(day, year),
            _ => throw new ArgumentOutOfRangeException()
        };
        var currentDayType = dayTypes.Single(t => t.Name == $"Year{year}Day{day}");
        var dayInstance = (IDay)Activator.CreateInstance(currentDayType)!;
        var ans = part switch
        {
            1 => await dayInstance.RunSolution1Async(input),
            2 => await dayInstance.RunSolution2Async(input),
            _ => throw new ArgumentOutOfRangeException()
        };
        Console.WriteLine(ans);
        Console.WriteLine("Done!");
    }
}