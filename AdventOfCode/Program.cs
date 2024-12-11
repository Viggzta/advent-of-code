using System.Diagnostics;
using AdventOfCode.Utility;

namespace AdventOfCode;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var manualInput = new List<string>
		{
			"R75,D30,R83,U83,L12,D49,R71,U7,L72",
			"U62,R66,U55,R34,D71,R55,D58,R83",
			""
		};

		var type = typeof(IDay);
		var dayTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(s => s.GetTypes())
			.Where(p => type.IsAssignableFrom(p));

		var year = 2024;
		var day = 12;
		var part = 1;
		var inputType = InputType.Example;
		string? testExtra = null;

		BoilerplateGenerator.CreateBoilerplateFile(year, day);

		var input = inputType switch
		{
			InputType.Manual => manualInput,
			InputType.Example => await InputFetcher.GetTestInputLinesAsync(day, year, testExtra),
			InputType.Real => await InputFetcher.GetAllInputLinesAsync(day, year),
			_ => throw new ArgumentOutOfRangeException()
		};
		var currentDayType = dayTypes.Single(t => t.Name == $"Year{year}Day{day}");
		var dayInstance = (IDay)Activator.CreateInstance(currentDayType)!;
		var stopwatch = Stopwatch.StartNew();
		var ans = part switch
		{
			1 => await dayInstance.RunSolution1Async(input),
			2 => await dayInstance.RunSolution2Async(input),
			_ => throw new ArgumentOutOfRangeException()
		};
		Console.WriteLine(ans);
		Console.WriteLine($"Done in {stopwatch.ElapsedMilliseconds}ms!");
	}
}