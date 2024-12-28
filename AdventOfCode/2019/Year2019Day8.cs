using AdventOfCode.Utility;

namespace AdventOfCode._2019;

public class Year2019Day8 : IDay
{
	public Task<string> RunSolution1Async(IList<string> input)
	{
		var layers = new Dictionary<int, List<int>>();
		var width = 25;
		var height = 6;
		for (var i = 0; i < input.First().Length; i++)
		{
			var layer = i / (width * height);
			layers.TryAdd(layer, new List<int>());

			layers[layer].Add(int.Parse(input[0][i].ToString()));
		}

		var minLayer = layers
			.MinBy(x => x.Value.Count(y => y == 0));
		var ones = minLayer.Value.Count(y => y == 1);
		var twos = minLayer.Value.Count(y => y == 2);

		var result = ones * twos;
		return Task.FromResult(result.ToString());
	}

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var image = new Dictionary<int, int>();
		var width = 25;
		var height = 6;
		for (var i = 0; i < input.First().Length; i++)
		{
			var pixel = i % (width * height);
			var color = int.Parse(input[0][i].ToString());
			if (color == 2) continue;
			image.TryAdd(pixel, color);
		}

		string result = "";
		for (var y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				if (image.TryGetValue(y * width + x, out var c))
				{
					result += c == 1 ? "#" : " ";
				}
			}

			result += "\n";
		}

		return Task.FromResult(result);
	}
}