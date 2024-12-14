namespace AdventOfCode.Utility;

public static class BoilerplateGenerator
{
	public static void CreateBoilerplateFile(int year, int day)
	{
#if DEBUG
		var currentPath = Directory.GetCurrentDirectory();
		var codePath = Path.Combine(currentPath, "..\\..\\..");
		var yearPath = Path.Combine(codePath, year.ToString());

		if (!Directory.Exists(yearPath))
		{
			Directory.CreateDirectory(yearPath);
		}

		var dayPath = Path.Combine(yearPath, $"Year{year}Day{day}.cs");

		if (File.Exists(dayPath))
		{
			return;
		}

		var outPutText = _boilerplateTemplate
			.Replace("YearNo", year.ToString())
			.Replace("DayNo", day.ToString());
		File.WriteAllText(dayPath, outPutText);
		Console.WriteLine($"A file for year {year} day {day} has been created.");
		Environment.Exit(0);
#endif
	}

	private const string _boilerplateTemplate =
	@"using AdventOfCode.Utility;

namespace AdventOfCode._YearNo;

public class YearYearNoDayDayNo : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		return Task.FromResult("""");
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		return Task.FromResult("""");
	}
}";
}