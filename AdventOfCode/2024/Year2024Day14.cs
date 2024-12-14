using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day14 : IDay
{
	private Regex _inputRegex = new Regex(@"p=(?<px>-?\d+),(?<py>-?\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)");

	public Task<string> RunSolution1Async(IList<string> input)
	{
		List<((int x, int y) p, (int x, int y) v)> ogRobots = input
			.TakeWhile(a => !string.IsNullOrWhiteSpace(a))
			.Select(a => _inputRegex.Match(a).Groups)
			.Select(a => (
				(int.Parse(a["px"].Value), int.Parse(a["py"].Value)),
				(int.Parse(a["vx"].Value), int.Parse(a["vy"].Value))))
			.ToList();

		(int w, int h) roomWH = (101, 103); // -1 since we are 0-based
		/*
		(int w, int h) roomWH = (11, 7);
		*/

		Dictionary<(int x, int y), int> newRobotLocs = new Dictionary<(int x, int y), int>();
			var steps = 100;
		foreach (var robot in ogRobots)
		{
			var newX = (robot.p.x + robot.v.x * steps) % roomWH.w;
			var newY = (robot.p.y + robot.v.y * steps) % roomWH.h;
			if (newX < 0) newX += roomWH.w;
			if (newY < 0) newY += roomWH.h;
			var newRobot = (newX, newY);
			newRobotLocs.TryAdd(newRobot, 0);
			newRobotLocs[newRobot]++;
		}

		var quadrants = ToQuadrants(newRobotLocs, roomWH);
		var quadCount = quadrants
			.Select(q => q.Values.Select(x => (long)x).Sum())
			.ToList();

		var result = quadCount
			.Aggregate((a, b) => a * b);

		return Task.FromResult(result.ToString());
	}

	private List<Dictionary<(int x, int y), int>> ToQuadrants(
		Dictionary<(int x, int y), int> locs,
		(int w, int h) roomWH)
	{
		var quadrants = new List<Dictionary<(int x, int y), int>>();
		var midX = roomWH.w / 2;
		var includeMidX = roomWH.w % 2 == 0 ? 0 : 1;
		var midY = roomWH.h / 2;
		var includeMidY = roomWH.h % 2 == 0 ? 0 : 1;

		for (int i = 0; i <= 1; i++)
		{
			for (int j = 0; j <= 1; j++)
			{

				var minX = (midX + includeMidX) * j;
				var maxX = minX + midX - includeMidX;
				var minY = (midY + includeMidY) * i;
				var maxY = minY + midY - includeMidY;
				Dictionary<(int x, int y), int> current = locs
					.Where(kvp =>
						kvp.Key.x >= minX && kvp.Key.x <= maxX &&
						kvp.Key.y >= minY && kvp.Key.y <= maxY)
					.ToDictionary();
				quadrants.Add(current);
			}
		}

		return quadrants;
	}

	public async Task<string> RunSolution2Async(IList<string> input)
	{
		List<((int x, int y) p, (int x, int y) v)> ogRobots = input
			.TakeWhile(a => !string.IsNullOrWhiteSpace(a))
			.Select(a => _inputRegex.Match(a).Groups)
			.Select(a => (
				(int.Parse(a["px"].Value), int.Parse(a["py"].Value)),
				(int.Parse(a["vx"].Value), int.Parse(a["vy"].Value))))
			.ToList();

		(int w, int h) roomWH = (101, 103); // -1 since we are 0-based
		/*
		(int w, int h) roomWH = (11, 7);
		*/

		var cacheDirectory = Path.Combine(
			Directory.GetCurrentDirectory(),
			"Artifacts");
		if (!Directory.Exists(cacheDirectory)) Directory.CreateDirectory(cacheDirectory);

		Console.WriteLine();
		var steps = 0;
		while (steps < 10000)
		{
			Bitmap bmp = new Bitmap(roomWH.w, roomWH.h);
			Dictionary<(int x, int y), int> newRobotLocs = new Dictionary<(int x, int y), int>();
			foreach (var robot in ogRobots)
			{
				var newX = (robot.p.x + robot.v.x * steps) % roomWH.w;
				var newY = (robot.p.y + robot.v.y * steps) % roomWH.h;
				if (newX < 0) newX += roomWH.w;
				if (newY < 0) newY += roomWH.h;
				var newRobot = (newX, newY);
				newRobotLocs.TryAdd(newRobot, 0);
				newRobotLocs[newRobot]++;
			}

			for (int y = 0; y < roomWH.h; y++)
			{
				for (int x = 0; x < roomWH.w; x++)
				{
					if (newRobotLocs.ContainsKey((x, y)))
					{
						bmp.SetPixel(x, y, Color.Black);
					}
					else
					{
						bmp.SetPixel(x, y, Color.White);
					}
				}
			}

			var outFile = Path.Combine(cacheDirectory, $"{steps}.bmp");
			bmp.Save(outFile, ImageFormat.Bmp);
			steps++;
		}

		return "";
	}
}