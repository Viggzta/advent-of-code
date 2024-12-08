using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day6 : IDay
{
    public Task<string> RunSolution1Async(IList<string> input)
    {
        input = input.Take(input.Count - 1).ToList();
        
        Dictionary<(int x, int y), bool> obstacles = new Dictionary<(int, int), bool>();
        Dictionary<(int x, int y), bool> visited = new Dictionary<(int, int), bool>();
        (int x, int y) guard = (0, 0);
        (int x, int y) direction = (0, -1);
        (int x, int y) botRight = (input.First().Length, input.Count);

        for (int y = 0; y < input.Count; y++)
        {
            var line = input[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    obstacles.Add((x, y), true);
                }
                
                if (line[x] == '^')
                {
                    guard = (x, y);
                }
            }
        }

        while (guard.x >= 0 && guard.x < botRight.x && guard.y >= 0 && guard.y < botRight.y)
        {
            visited[(guard.x, guard.y)] = true;
            var next = (guard.x + direction.x, guard.y  + direction.y);

            if (obstacles.ContainsKey(next))
            {
                if (direction == (0, -1)) direction = (1, 0);
                else if (direction == (1, 0)) direction = (0, 1);
                else if (direction == (0, 1)) direction = (-1, 0);
                else if (direction == (-1, 0)) direction = (0, -1);
            }
            else
            {
                guard = next;
            }
        }
        
        var result = visited.Count;
        
        return Task.FromResult(result.ToString());
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        input = input.Take(input.Count - 1).ToList();
        
        Dictionary<(int x, int y), bool> obstacles = new Dictionary<(int, int), bool>();
        Dictionary<(int x, int y), bool> visited = new Dictionary<(int, int), bool>();
        (int x, int y) guard = (0, 0);
        (int x, int y) guardOriginal = (0, 0);
        (int x, int y) direction = (0, -1);
        (int x, int y) botRight = (input.First().Length, input.Count);

        for (int y = 0; y < input.Count; y++)
        {
            var line = input[y];
            for (int x = 0; x < line.Length; x++)
            {
                if (line[x] == '#')
                {
                    obstacles.Add((x, y), true);
                }
                
                if (line[x] == '^')
                {
                    guard = (x, y);
                    guardOriginal = (x, y);
                }
            }
        }

        while (guard.x >= 0 && guard.x < botRight.x && guard.y >= 0 && guard.y < botRight.y)
        {
            visited[(guard.x, guard.y)] = true;
            var next = (guard.x + direction.x, guard.y  + direction.y);

            if (obstacles.ContainsKey(next))
            {
                if (direction == (0, -1)) direction = (1, 0);
                else if (direction == (1, 0)) direction = (0, 1);
                else if (direction == (0, 1)) direction = (-1, 0);
                else if (direction == (-1, 0)) direction = (0, -1);
            }
            else
            {
                guard = next;
            }
        }

        var loops = 0;
        foreach (var b in visited)
        {
            guard = guardOriginal;
            direction = (0, -1);
            var visitedAndDirection = new Dictionary<(int x, int y, int dirX, int dirY), bool>();
            var obstacles2 = obstacles.ToDictionary(x => x.Key, x => x.Value);
            obstacles2.Add(b.Key, true);
            var visited2 = new Dictionary<(int x, int y), bool>();
            
            bool isLoop = false;
            while (guard.x >= 0 && guard.x < botRight.x && guard.y >= 0 && guard.y < botRight.y)
            {
                var visDir = (guard.x, guard.y, direction.x, direction.y);
                if (visitedAndDirection.ContainsKey(visDir))
                {
                    isLoop = true;
                    break;
                }
                visited2[(guard.x, guard.y)] = true;
                visitedAndDirection[visDir] = true;
                var next = (guard.x + direction.x, guard.y  + direction.y);

                if (obstacles2.ContainsKey(next))
                {
                    if (direction == (0, -1)) direction = (1, 0);
                    else if (direction == (1, 0)) direction = (0, 1);
                    else if (direction == (0, 1)) direction = (-1, 0);
                    else if (direction == (-1, 0)) direction = (0, -1);
                }
                else
                {
                    guard = next;
                }
            }

            if (isLoop)
            {
                loops++;
            }
        }
        
        var result = loops;
        
        return Task.FromResult(result.ToString());
    }
}