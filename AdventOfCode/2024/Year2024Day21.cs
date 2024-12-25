using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
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

		var numpads = new List<Numpad>
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

		var numpads = new List<Numpad>
		{
			new Numpad(nineDigitNumpad),
		};

		foreach (var _ in Enumerable.Range(0, 25))
		{
			numpads.Add(new Numpad(controlNumPad));
		}

		var result = 0L;
		Dictionary<(char, char, int), long> cache = new();
		foreach (var code in codes)
		{
			var codeNumericValue = long.Parse(code.SkipLast(1).Aggregate("", (a, b) => a + b));
			result += codeNumericValue * GetInputLength(code, numpads, cache);
		}

		// 2131838763 too low
		return Task.FromResult(result.ToString());
	}

	private static long GetInputLength(string code, List<Numpad> numpads, Dictionary<(char, char, int), long> cache)
	{
		if (numpads.Count == 0)
		{
			return code.Length;
		}

		var requiredSteps = 0L;
		var currentKey = 'A';
		foreach (var c in code)
		{
			requiredSteps += GetInputLength(currentKey, c, numpads, cache);
			currentKey = c;
		}

		Debug.Assert(currentKey == 'A');
		return requiredSteps;
	}

	private static long GetInputLength(char currentChar, char nextChar, List<Numpad> numpads, Dictionary<(char, char, int), long> cache)
	{
		if (cache.TryGetValue((currentChar, nextChar, numpads.Count), out var cachedValue))
		{
			return cachedValue;
		}

		var numpad = numpads[0];

		(int x, int y) currentPos = numpad.NumpadMap[currentChar];
		(int x, int y) nextPos = numpad.NumpadMap[nextChar];

		var dy = nextPos.y - currentPos.y;
		var vertical = new string(dy < 0 ? '^' : 'v', Math.Abs(dy));
		var dx = nextPos.x - currentPos.x;
		var horizontal = new string(dx < 0 ? '<' : '>', Math.Abs(dx));

		var cost = long.MaxValue;

		(int x, int y) verticalFirst = (currentPos.x, nextPos.y);
		if (numpad.NumpadMap.Values.Any(x => x == verticalFirst))
		{
			cost = Math.Min(cost, GetInputLength($"{vertical}{horizontal}A", numpads[1..], cache));
		}

		(int x, int y) horizontalFirst = (nextPos.x, currentPos.y);
		if (numpad.NumpadMap.Values.Any(x => x == horizontalFirst))
		{
			cost = Math.Min(cost, GetInputLength($"{horizontal}{vertical}A", numpads[1..], cache));
		}

		cache.Add((currentChar, nextChar, numpads.Count), cost);
		return cost;
	}

	private class Numpad(Dictionary<char, (int x, int y)> numpadMap)
	{
		private (int x, int y) _pointerPosition = numpadMap['A'];
		Dictionary<char, string> _chacheMap = new Dictionary<char, string>();

		public Dictionary<char, (int, int)> NumpadMap => numpadMap;

		public string SolveFor(string requiredSteps)
		{
			var requiredActions = "";
			foreach (var s in requiredSteps)
			{
				requiredActions += GetSteps(s);
				requiredActions += 'A';
			}
			return requiredActions;
		}

		string GetSteps(char targetChar)
		{
			if (_chacheMap.TryGetValue(targetChar, out var cachedValue))
			{
				return cachedValue;
			}

			var targetCharPos = numpadMap[targetChar];
			(int x, int y) deltaPos = (
				(targetCharPos.x - _pointerPosition.x),
				(targetCharPos.y - _pointerPosition.y));

			var xChar = deltaPos.x < 0 ? '<' : '>';
			var yChar = deltaPos.y < 0 ? '^' : 'v';

			var outString = "";

			var preferHorizontal = yChar == '^'; // if we just want to move left prioritize horizontal movement

			(int x, int y) horizontalFirstPos = ((_pointerPosition.x + deltaPos.x), _pointerPosition.y);
			var canDoHorizontalFirst = numpadMap.Values.Any(x => x == horizontalFirstPos);
			if (canDoHorizontalFirst && preferHorizontal)
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
			_chacheMap.Add(targetChar, outString);
			return outString;
		}
	}
}