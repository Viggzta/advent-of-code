using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day8 : IDay
{
    public int DayNumber => 8;
    
    public Task<string> RunSolution1Async(IList<string> input)
    {
        input = input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList();
        Dictionary<char, List<(int x, int y)>> beacons = new();
        HashSet<(int x, int y)> allBeaconLocations = new();
        for (int y = 0; y < input.Count; y++)
        {
            var line = input[y];
            for (int x = 0; x < line.Length; x++)
            {
                var ch = line[x];

                if (ch != '.')
                {
                    if (!beacons.ContainsKey(ch)) beacons.Add(ch, new List<(int x, int y)>());
                    beacons[ch].Add((x, y));
                    allBeaconLocations.Add((x, y));
                }
            }
        }

        var maxX = input[0].Length;
        var maxY = input.Count;
        bool IsInside((int x, int y) tuple)
        {
            if (tuple.x < 0 || tuple.x >= maxX) return false;
            if (tuple.y < 0 || tuple.y >= maxY) return false;
            return true;
        }

        var antinodeLocations = new List<(int x, int y)>();
        foreach (var beacon in beacons)
        {
            foreach ((int x, int y) tuple in beacon.Value)
            {
                var internalMirrors = beacon.Value
                    .Select(other => (tuple.x - other.x, tuple.y - other.y))
                    .Where(diff => diff.Item1 != 0 || diff.Item2 != 0)
                    .Select(diff => (tuple.x + diff.Item1, tuple.y + diff.Item2))
                    .Where(IsInside)
                    .ToList();
                
                antinodeLocations.AddRange(internalMirrors);
            }
        }
                    
        var result = antinodeLocations
            .Distinct()
            .Count();

        return Task.FromResult(result.ToString());
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        input = input.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList();
        Dictionary<char, List<(int x, int y)>> beacons = new();
        HashSet<(int x, int y)> allBeaconLocations = new();
        for (int y = 0; y < input.Count; y++)
        {
            var line = input[y];
            for (int x = 0; x < line.Length; x++)
            {
                var ch = line[x];

                if (ch != '.')
                {
                    if (!beacons.ContainsKey(ch)) beacons.Add(ch, new List<(int x, int y)>());
                    beacons[ch].Add((x, y));
                    allBeaconLocations.Add((x, y));
                }
            }
        }

        var maxX = input[0].Length;
        var maxY = input.Count;
        var maxLength = Math.Max(maxX, maxY);
        bool IsInside((int x, int y) tuple)
        {
            if (tuple.x < 0 || tuple.x >= maxX) return false;
            if (tuple.y < 0 || tuple.y >= maxY) return false;
            return true;
        }

        var antinodeLocations = new List<(int x, int y)>();
        foreach (var beacon in beacons)
        {
            foreach ((int x, int y) tuple in beacon.Value)
            {
                var internalMirrors = beacon.Value
                    .Select(other => (tuple.x - other.x, tuple.y - other.y))
                    .SelectMany(diff => Enumerable.Range(0, maxLength)
                        .Select(i => (tuple.x + diff.Item1 * i, tuple.y + diff.Item2 * i)))
                    .Where(IsInside)
                    .ToList();
                
                antinodeLocations.AddRange(internalMirrors);
            }
        }
        
        antinodeLocations.Sort((a, b) => a.y.CompareTo(b.y));
        antinodeLocations = antinodeLocations.Distinct().ToList();
                    
        var result = antinodeLocations.Count;

        return Task.FromResult(result.ToString());
    }
}