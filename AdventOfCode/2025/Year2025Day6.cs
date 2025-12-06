using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day6 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		var problems = new Dictionary<int, List<string>>();

		foreach (var line in input)
		{
			var cols = line.Split(' ')
				.Where(x => !string.IsNullOrWhiteSpace(x))
				.Index()
				.ToList();
			foreach (var col in cols)
			{
				if (!problems.ContainsKey(col.Index))
				{
					problems.Add(col.Index, new List<string>());
				}

				problems[col.Index].Add(col.Item);
			}
		}

		var answers = new List<long>();
		foreach ((int Index, List<string> N) in problems)
		{
			var symbol = N.Last();
			var numbers = N.Take(N.Count - 1).Select(long.Parse).ToList();

			if (symbol == "*")
			{
				answers.Add(numbers.Aggregate((a,b) => a * b));
			}
			else if (symbol == "+")
			{
				answers.Add(numbers.Sum());
			}
			else
			{
				throw new Exception();
			}
		}

		return Task.FromResult(answers.Sum().ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		Dictionary<int, Dictionary<int, List<int>>> problems = new();
		Dictionary<int, char> symbolList = new();
		foreach (var line in input)
		{
			bool hasSeenFirstNumber = false;
			int numberIndex = 0;
			char? previousNumber = null;
			foreach ((int Index, char Item) number in line.Index())
			{
				if (char.IsDigit(number.Item))
				{
					hasSeenFirstNumber = true;
					if (!problems.ContainsKey(numberIndex))
					{
						problems[numberIndex] = new();
					}

					if (!problems[numberIndex].ContainsKey(number.Index))
					{
						problems[numberIndex][number.Index] = new();
						problems[numberIndex][number.Index].Add(int.Parse(number.Item.ToString()));
					}
					else
					{
						problems[numberIndex][number.Index].Add(int.Parse(number.Item.ToString()));
					}
				}
				else if (number.Item == '+' || number.Item == '*')
				{
					symbolList[numberIndex] = number.Item;
					hasSeenFirstNumber = true;
				}
				else
				{
					if (hasSeenFirstNumber && (previousNumber != null && previousNumber != ' '))
					{
						numberIndex++;
					}
				}

				previousNumber = number.Item;
			}
		}

		var results = new List<long>();
		foreach (KeyValuePair<int, Dictionary<int, List<int>>> kvp in problems)
		{
			var numbers = kvp.Value.Select(x => long.Parse(string.Join("", x.Value))).ToList();
			if (symbolList[kvp.Key] == '*')
			{
				results.Add(numbers.Aggregate((a,b) => a * b));
			}
			else
			{
				results.Add(numbers.Sum());
			}
		}

		return Task.FromResult(results.Sum().ToString());
	}
}