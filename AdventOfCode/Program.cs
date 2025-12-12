using System.Diagnostics;
using AdventOfCode.Utility;

namespace AdventOfCode;

internal class Program
{
	private static async Task Main(string[] args)
	{
		var manualInput = new List<string>();

		var type = typeof(IDay);
		var dayTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(s => s.GetTypes())
			.Where(p => type.IsAssignableFrom(p));

		var year = 2025;
		var day = 12;
		var part = 1;
		var inputType = InputType.Real;
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