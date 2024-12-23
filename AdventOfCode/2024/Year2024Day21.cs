using System.Diagnostics;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day21 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var codes = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.ToList();

		var nineDigitNumpad = new Dictionary<char, (int, int)>
		{
			['7'] = (0, 0),
			['8'] = (1, 0),
			['9'] = (2, 0),
			['4'] = (0, 1),
			['5'] = (1, 1),
			['6'] = (2, 1),
			['1'] = (0, 2),
			['2'] = (1, 2),
			['3'] = (2, 2),
			['0'] = (1, 3),
			['A'] = (2, 3),
		};
		var controlNumPad = new Dictionary<char, (int, int)>
		{
			['^'] = (1, 0),
			['A'] = (2, 0),
			['<'] = (0, 1),
			['v'] = (1, 1),
			['>'] = (2, 1),
		};

		List<Numpad> numpads = new List<Numpad>
		{
			new Numpad(nineDigitNumpad),
			new Numpad(controlNumPad),
			new Numpad(controlNumPad),
		};

		var solvedCodes = new Dictionary<string, string>();
		foreach (var code in codes)
		{
			var solvedFor = "";
			foreach (var c in code)
			{
				var requiredSteps = c.ToString();
				foreach (var numpad in numpads)
				{
					requiredSteps = numpad.SolveFor(requiredSteps);
				}
				solvedFor += requiredSteps;
			}
			solvedCodes[code] = solvedFor;
		}

		long result = 0;
		foreach (var code in solvedCodes)
		{
			var codeNumericValue = long.Parse(code.Key.SkipLast(1).Aggregate("", (a, b) => a + b));
			var codeValLength = code.Value.Length;
			var tempResult = codeNumericValue * codeValLength;
			result += tempResult;
			Console.WriteLine($"Code: {code.Key} | {code.Value.Length} | {tempResult} | {code.Value}");
		}

		// 184712 Too high
		return Task.FromResult(result.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		return Task.FromResult("");
	}

	private class Numpad
	{
		(int x, int y) _pointerPosition;
		Dictionary<char, (int x, int y)> _numpadMap;

		public Numpad(Dictionary<char, (int x, int y)> numpadMap)
		{
			_numpadMap = numpadMap;
			_pointerPosition = numpadMap['A'];
		}

		public string SolveFor(string requiredSteps)
		{
			var requredActions = "";
			foreach (var s in requiredSteps)
			{
				requredActions += GetSteps(s);
				requredActions += 'A';
			}
			return requredActions;
		}

		string GetSteps(char targetChar)
		{
			var targetCharPos = _numpadMap[targetChar];
			(int x, int y) deltaPos = (
				(targetCharPos.x - _pointerPosition.x),
				(targetCharPos.y - _pointerPosition.y));

			var xChar = deltaPos.x < 0 ? '<' : '>';
			var yChar = deltaPos.y < 0 ? '^' : 'v';

			var outString = "";

			(int x, int y) horizontalFirstPos = ((_pointerPosition.x + deltaPos.x), _pointerPosition.y);
			var canDoHorizontalFirst = _numpadMap.Values.Any(x => x == horizontalFirstPos);
			if (canDoHorizontalFirst)
			{
				outString += new string(xChar, Math.Abs(deltaPos.x));
				outString += new string(yChar, Math.Abs(deltaPos.y));
			}
			else
			{
				outString += new string(yChar, Math.Abs(deltaPos.y));
				outString += new string(xChar, Math.Abs(deltaPos.x));
			}

			_pointerPosition = targetCharPos;
			return outString;
		}

		private static readonly Dictionary<(int x, int y), char> dirToCharConvertMap = new()
		{
			{ (1, 0), '>' },
			{ (-1, 0), '<' },
			{ (0, 1), 'v' },
			{ (0, -1), '^' },
		};
	}
}