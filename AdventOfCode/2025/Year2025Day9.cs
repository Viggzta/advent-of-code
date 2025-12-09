using System.Formats.Asn1;
using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day9 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		List<(long X, long Y)> positions = input
			.Select(x =>
			{
				var split = x.Split(',');
				return (long.Parse(split[0]), long.Parse(split[1]));
			})
			.ToList();

		// var largestArea = positions
		// 	.SelectMany(x => positions
		// 		.Select(y => (x, y, GetArea(x, y))))
		// 	.ToList();
		var largestArea = positions
			.Max(x => positions
				.Max(y => GetArea(x, y)));

		return Task.FromResult(largestArea.ToString());
	}

	private long GetArea((long X, long Y) a, (long X, long Y) b)
	{
		var width = Math.Abs(b.X - a.X) + 1;
		var height = Math.Abs(b.Y - a.Y) + 1;
		return width * height;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();

		List<(long X, long Y)> positions = input
			.Select(x =>
			{
				var split = x.Split(',');
				return (long.Parse(split[0]), long.Parse(split[1]));
			})
			.ToList();


		var polygonLines = new List<((long X, long Y) a, (long X, long Y) b)>();
		for (int i = 1; i <= positions.Count; i++)
		{
			var start = positions[i - 1];
			var end = i == positions.Count ? positions[0] : positions[i] ;
			polygonLines.Add((start, end));
		}

		List<((long X, long Y) a, (long X, long Y) b)> areaRectangles = positions
			.SelectMany(x => positions
				.Select(y => (a: x,b: y)))
			.Where(p => p.a != p.b)
			.Where(r => IsInside(r, polygonLines))
			.ToList();
		var largestArea = areaRectangles
			.Max(r => GetArea(r.a, r.b));

		return Task.FromResult(largestArea.ToString());
	}

	private bool IsInside(
		((long X, long Y) a, (long X, long Y) b) rectangle,
		List<((long X, long Y) a, (long X, long Y) b)> polygonLines)
	{
		List<((long X, long Y) a, (long X, long Y) b)> rectangleLines =
		[
			(a: (rectangle.a.X, rectangle.a.Y), b: (rectangle.b.X, rectangle.a.Y)),
			(a: (rectangle.b.X, rectangle.a.Y), b: (rectangle.b.X, rectangle.b.Y)),
			(a: (rectangle.b.X, rectangle.b.Y), b: (rectangle.a.X, rectangle.b.Y)),
			(a: (rectangle.a.X, rectangle.b.Y), b: (rectangle.a.X, rectangle.a.Y)),
		];

		return !rectangleLines.Any(x => polygonLines.Any(y => Intersects(x, y)));
	}

	private bool Intersects(
		((long X, long Y) a, (long X, long Y) b) line1,
		((long X, long Y) a, (long X, long Y) b) line2)
	{
		if (line1.a == line1.b)
		{
			// Somehow point?
			return false;
		}

		var line1IsHorizontal = IsHorizontal(line1);
		var line2IsHorizontal = IsHorizontal(line2);
		if (line1IsHorizontal && line2IsHorizontal)
		{
			// Make sure that line1 fully covers line2
			// Possible edge-case
			return false;
		}
		else if (!line1IsHorizontal && !line2IsHorizontal)
		{
			// Make sure that line1 fully covers line2
			// Possible edge-case
			return false;
		}
		else
		{
			if (line1IsHorizontal)
			{
				var x = line2.a.X;
				var y = line1.a.Y;
				(long start, long end) line1AdjustedX = (Math.Min(line1.a.X, line1.b.X), Math.Max(line1.a.X, line1.b.X));
				var isInsideHorizontalLine = x >= line1AdjustedX.start && x <= line1AdjustedX.end;
				(long start, long end) line2AdjustedY = (Math.Min(line2.a.Y, line2.b.Y), Math.Max(line2.a.Y, line2.b.Y));
				var isInsideVerticalLine = y >= line2AdjustedY.start && y <= line2AdjustedY.end;
				return isInsideHorizontalLine && isInsideVerticalLine;
			}
			else
			{
				var x = line1.a.X;
				var y = line2.a.Y;
				(long start, long end) line2AdjustedX = (Math.Min(line2.a.X, line2.b.X), Math.Max(line2.a.X, line2.b.X));
				var isInsideHorizontalLine = x >= line2AdjustedX.start && x <= line2AdjustedX.end;
				(long start, long end) line1AdjustedY = (Math.Min(line1.a.Y, line1.b.Y), Math.Max(line1.a.Y, line1.b.Y));
				var isInsideVerticalLine = y >= line1AdjustedY.start && y <= line1AdjustedY.end;
				return isInsideHorizontalLine && isInsideVerticalLine;
			}
		}

		Console.WriteLine("Error");
		return false;
	}

	private bool IsHorizontal(
		((long X, long Y) a, (long X, long Y) b) line1)
	{
		return line1.a.Y == line1.b.Y;
	}
}