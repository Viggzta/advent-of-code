using System.Text;

namespace AdventOfCode.Utility;

public static class Map2D {

	public static void PrintMap(
		char emptyChar,
		params (char layerName, HashSet<(int x, int y)> map)[] layers)
	{
		var minX = layers.Min(l => l.map.Min(p => p.x));
		var minY = layers.Min(l => l.map.Min(p => p.y));
		var maxX = layers.Max(l => l.map.Max(p => p.x));
		var maxY = layers.Max(l => l.map.Max(p => p.y));

		for (int y = minY; y <= maxY; y++)
		{
			StringBuilder sb = new();
			for (int x = minX; x <= maxX; x++)
			{
				bool anySpecial = false;
				foreach ((char layerName, HashSet<(int x, int y)> map) in layers)
				{
					if (map.Contains((x, y)))
					{
						sb.Append(layerName);
						anySpecial = true;
						break;
					}
				}

				if (!anySpecial) sb.Append(emptyChar);
			}

			Console.WriteLine(sb.ToString());
		}
	}
}