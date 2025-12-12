using System.Data.SqlTypes;
using AdventOfCode.Utility;

namespace AdventOfCode._2025;

public class Year2025Day12 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		input = input.Take(input.Count - 1).ToList();
		var shapeCount = 5;
		var shapeInput = input.Take(shapeCount * 6).ToList();
		var shapes = ParseShapes(shapeInput);
		var regionDataInput = input.Skip(shapeCount * 6).ToList();
		var regionDatas = ParseRegionData(regionDataInput);

		var validBoxes = 0;
		foreach (var regionData in regionDatas)
		{
			var area = regionData.region.width * regionData.region.height;
			var minTotalArea = regionData.boxCount
				.Index()
				.Sum(b => shapes[b.Index].area * b.Item);
			if (minTotalArea <= area)
			{
				validBoxes++;
			}
		}

		return Task.FromResult(validBoxes.ToString());
	}

	private List<((int width, int height) region, List<int> boxCount)> ParseRegionData(List<string> regionDataInput)
	{
		return regionDataInput
			.Select(line =>
				{
					var split1 = line.Split(": ");
					var split2 = split1[0].Split("x");
					(int width, int height) region = (int.Parse(split2[0]), int.Parse(split2[1]));
					var boxCount = split1[1].Split(' ').Select(int.Parse).ToList();
					return (region, boxCount);
				})
			.ToList();
	}

	private List<(bool[,], int area)> ParseShapes(List<string> shapeInput)
	{
		var skip = 0;
		List<(bool[,], int)> shapes = new();
		while (true)
		{
			var shapeMap = new bool[3,3];
			int area = 0;
			var shapeData = shapeInput.Skip(skip + 1).Take(5 - 1).ToList();
			foreach ((int y, string shapeString) in shapeData.Index())
			{
				foreach ((int x, char shapeChar) in shapeString.Index())
				{
					var isSolid = shapeChar == '#';
					if (isSolid) area++;
					shapeMap[x, y] = isSolid;
				}
			}
			shapes.Add((shapeMap, area));
			skip += 5;
			if (skip >= shapeInput.Count) break;
		}

		return shapes;
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		return Task.FromResult("");
	}
}