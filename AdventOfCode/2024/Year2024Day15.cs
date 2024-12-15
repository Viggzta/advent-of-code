using System.Text;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day15 : IDay
{
	private ((int x, int y) robot, HashSet<(int x, int y)> walls, HashSet<(int x, int y)> boxes)
		ParseInput(List<string> input)
	{
		(int x, int y) robot = (-1, -1);
		HashSet<(int x, int y)> walls = new();
		HashSet<(int x, int y)> boxes = new();

		foreach (var row in input.Index())
		{
			foreach (var cell in row.Item.Index())
			{
				var currentCoordinate = (cell.Index, row.Index);
				switch (cell.Item)
				{
					case '#':
						walls.Add(currentCoordinate);
						break;
					case 'O':
						boxes.Add(currentCoordinate);
						break;
					case '@':
						robot = currentCoordinate;
						break;
				}
			}
		}

		return (robot, walls, boxes);
	}

	private (int x, int y) ToDirection(char c)
	{
		return c switch
		{
			'^' => (0, -1),
			'v' => (0, 1),
			'>' => (1, 0),
			'<' => (-1, 0),
			_ => throw new ArgumentOutOfRangeException(nameof(c), c, "Invalid direction"),
		};
	}

	public Task<string> RunSolution1Async(IList<string> input)
	{
		var mapInput = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.ToList();
		((int x, int y) robot, HashSet<(int x, int y)> walls, HashSet<(int x, int y)> boxes) =
			ParseInput(mapInput);
		List<(int x, int y)> robotMoves = input
			.Skip(mapInput.Count + 1)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Replace("&lt;", "<").Replace("&gt;", ">"))
			.Aggregate((a, b) => a + b)
			.Select(ToDirection)
			.ToList();

		foreach ((int x, int y) moveDir in robotMoves)
		{
			if (CanMoveTo(robot, moveDir, boxes, walls))
			{
				robot = (robot.x + moveDir.x, robot.y + moveDir.y);
				ApplyMoveBoxesTo(robot, moveDir, boxes);
			}
		}

		/*
		var robotHash = new HashSet<(int x, int y)>();
		robotHash.Add(robot);
		PrintMap(
			'.',
			('@', robotHash),
			('O', boxes),
			('#', walls));
			*/

		var result = boxes.Select(p => 100L * p.y + p.x).Sum();

		return Task.FromResult(result.ToString());
	}

	private static bool CanMoveTo(
		(int x, int y) pos,
		(int x, int y) direction,
		HashSet<(int x, int y)> boxes,
		HashSet<(int x, int y)> walls)
	{
		var targetLocation = (pos.x +  direction.x, pos.y +  direction.y);
		if (walls.Contains(targetLocation)) return false;
		if (boxes.TryGetValue(targetLocation, out (int x, int y) box))
		{
			return CanMoveTo(targetLocation, direction, boxes, walls);
		}

		return true;
	}

	private static void ApplyMoveBoxesTo(
		(int x, int y) pos,
		(int x, int y) direction,
		HashSet<(int x, int y)> boxes)
	{
		if (!boxes.TryGetValue(pos, out _))
		{
			// Nothing to cascade
			return;
		}

		var targetLocation = (pos.x +  direction.x, pos.y +  direction.y);
		if (boxes.TryGetValue(targetLocation, out _))
		{
			ApplyMoveBoxesTo(targetLocation, direction, boxes);
		}

		if (boxes.TryGetValue(pos, out var box))
		{
			boxes.Remove(box);
			boxes.Add(targetLocation);
		}
	}

	private void PrintMap(char emptyChar, params (char layerName, HashSet<(int x, int y)> map)[] layers)
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

	public Task<string> RunSolution2Async(IList<string> input)
	{
		var mapInput = input
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.ToList();
		((int x, int y) robot, HashSet<(int x, int y)> walls, HashSet<((int x, int y) a, (int x, int y) b)> boxes) =
			ParseInput2(mapInput);
		List<(int x, int y)> robotMoves = input
			.Skip(mapInput.Count + 1)
			.TakeWhile(x => !string.IsNullOrWhiteSpace(x))
			.Select(x => x.Replace("&lt;", "<").Replace("&gt;", ">"))
			.Aggregate((a, b) => a + b)
			.Select(ToDirection)
			.ToList();

			/*
			var robotHash = new HashSet<(int x, int y)>();
			robotHash.Add(robot);
			var boxLeftHash = boxes.Select(box => box.a).ToHashSet();
			var boxRightHash = boxes.Select(box => box.b).ToHashSet();
			PrintMap(
				'.',
				('@', robotHash),
				('[', boxLeftHash),
				(']', boxRightHash),
				('#', walls));
			Console.ReadKey();
			*/

		foreach ((int x, int y) moveDir in robotMoves)
		{
			var robotAsBox = (robot, (-999, -999));

			if (CanMoveTo2(robotAsBox, moveDir, boxes, walls))
			{
				robot = (robot.x + moveDir.x, robot.y + moveDir.y);
				ApplyMoveBoxesTo2(robotAsBox, moveDir, boxes);
			}

			/*
			Console.Clear();
			robotHash = new HashSet<(int x, int y)>();
			robotHash.Add(robot);
			boxLeftHash = boxes.Select(box => box.a).ToHashSet();
			boxRightHash = boxes.Select(box => box.b).ToHashSet();
			PrintMap(
				'.',
				('@', robotHash),
				('[', boxLeftHash),
				(']', boxRightHash),
				('#', walls));
			Console.ReadKey();
		*/
		}

		var robotHash = new HashSet<(int x, int y)>();
		robotHash.Add(robot);
		var boxLeftHash = boxes.Select(box => box.a).ToHashSet();
		var boxRightHash = boxes.Select(box => box.b).ToHashSet();
		PrintMap(
			'.',
			('@', robotHash),
			('[', boxLeftHash),
			(']', boxRightHash),
			('#', walls));
		var bounds = (walls.Max(x => x.x), walls.Max(y => y.y));
		Console.WriteLine($"bounds: {bounds}");
		var boxScores = boxes
			.OrderBy(x => x.a.y).ThenBy(x=>x.a.x)
			.Select(p => ToGPSScore(p, bounds))
			.ToList();

		var result = boxScores.Sum();

		return Task.FromResult(result.ToString());
	}

	private long ToGPSScore(((int x, int y) a, (int x, int y) b) box, (int right, int bottom) bounds)
	{
		/*
		var xDist1 = Math.Min(box.a.x, bounds.right - box.a.x);
		var xDist2 = Math.Min(box.b.x, bounds.right - box.b.x);
		var xDist = Math.Min(xDist1, xDist2);

		var yDist1 = Math.Min(box.a.y, bounds.bottom - box.a.y);
		var yDist2 = Math.Min(box.b.y, bounds.bottom - box.b.y);
		var yDist = Math.Min(yDist1, yDist2);
		*/

		var xDist = box.a.x;
		var yDist = box.a.y;

		return 100L * yDist + xDist;
	}

	private static bool CanMoveTo2(
		((int x, int y) a, (int x, int y) b) pos,
		(int x, int y) direction,
		HashSet<((int x, int y) a, (int x, int y) b)> boxes,
		HashSet<(int x, int y)> walls)
	{
		var tarLoc1 = (pos.a.x +  direction.x, pos.a.y +  direction.y);
		var tarLoc2 = (pos.b.x +  direction.x, pos.b.y +  direction.y);
		if (walls.Contains(tarLoc1) || walls.Contains(tarLoc2)) return false;
		var collideableBoxes = boxes
			.Where(box =>
				(box.a == tarLoc1 || box.a == tarLoc2 ||
				box.b == tarLoc1 || box.b == tarLoc2)
				&& box != pos)
			.ToList();

		if (collideableBoxes.Count != 0)
		{
			return collideableBoxes.All(box => CanMoveTo2(box, direction, boxes, walls));
		}

		return true;
	}

	private static void ApplyMoveBoxesTo2(
		((int x, int y) a, (int x, int y) b) pos,
		(int x, int y) direction,
		HashSet<((int x, int y) a, (int x, int y) b)> boxes)
	{
		var tarLoc1 = (pos.a.x +  direction.x, pos.a.y +  direction.y);
		var tarLoc2 = (pos.b.x +  direction.x, pos.b.y +  direction.y);

		var collideableBoxes = boxes
			.Where(box =>
				(box.a == tarLoc1 || box.a == tarLoc2 ||
				box.b == tarLoc1 || box.b == tarLoc2)
				&& box != pos)
			.ToList();

		foreach (var boxX in collideableBoxes)
		{
			ApplyMoveBoxesTo2(boxX, direction, boxes);
		}

		if ( (pos.b.x != -999 || pos.b.y != -999) &&
			boxes.TryGetValue(pos, out var boxTemp))
		{
			boxes.Remove(boxTemp);
			var boxA = (boxTemp.a.x + direction.x, boxTemp.a.y + direction.y);
			var boxB = (boxTemp.b.x + direction.x, boxTemp.b.y + direction.y);
			var newBox = (boxA, boxB);
			boxes.Add(newBox);
		}
	}

	private (
		(int x, int y) robot,
		HashSet<(int x, int y)> walls,
		HashSet<((int x, int y) a, (int x, int y) b)> boxes
		) ParseInput2(List<string> input)
	{
		(int x, int y) robot = (-1, -1);
		HashSet<(int x, int y)> walls = new();
		HashSet<((int x, int y) a, (int x, int y) b)> boxes = new();

		foreach (var row in input.Index())
		{
			foreach (var cell in row.Item.Index())
			{
				var currentCoordinate = (cell.Index * 2, row.Index);
				var currentWide = (cell.Index * 2 + 1, row.Index);
				switch (cell.Item)
				{
					case '#':
						walls.Add(currentCoordinate);
						walls.Add(currentWide);
						break;
					case 'O':
						boxes.Add((currentCoordinate, currentWide));
						break;
					case '@':
						robot = currentCoordinate;
						break;
				}
			}
		}

		return (robot, walls, boxes);
	}
}