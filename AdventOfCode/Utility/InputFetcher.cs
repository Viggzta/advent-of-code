using System.Net;
using System.Text.RegularExpressions;

namespace AdventOfCode.Utility;

public static class InputFetcher
{
    public static async Task<IList<string>> GetAllInputLinesAsync(int day, int year)
    {
        var cacheDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            $"CachedInput{year}");
        if (!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory);
        }

        var targetFilePath = Path.Combine(cacheDirectory, $"{day}.txt");

        if (File.Exists(targetFilePath))
        {
            return await File.ReadAllLinesAsync(targetFilePath);
        }

        Uri uri = new Uri($"https://adventofcode.com/{year}/day/{day}/input");
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;
        using var client = new HttpClient(handler);
        cookieContainer.Add(uri, new Cookie(
            "session",
            Secret.SessionKey));
        var result = await client.GetStringAsync(uri);

        var resultOutput = result.Split('\n').ToList();

        await File.WriteAllLinesAsync(targetFilePath, resultOutput);
        return resultOutput;
    }

    public static async Task<IList<string>> GetTestInputLinesAsync(int day, int year, string? extra = null)
    {
        var cacheDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            $"CachedTestInput{year}");
        if (!Directory.Exists(cacheDirectory))
        {
            Directory.CreateDirectory(cacheDirectory);
        }

        var fileName = extra is null ? $"{day}.txt" : $"{day}-{extra}.txt";
        var targetFilePath = Path.Combine(cacheDirectory, fileName);

        if (File.Exists(targetFilePath))
        {
            return await File.ReadAllLinesAsync(targetFilePath);
        }

        Uri uri = new Uri($"https://adventofcode.com/{year}/day/{day}");
        var cookieContainer = new CookieContainer();
        using var handler = new HttpClientHandler();
        handler.CookieContainer = cookieContainer;
        using var client = new HttpClient(handler);
        cookieContainer.Add(uri, new Cookie(
            "session",
            Secret.SessionKey));
        var result = await client.GetStringAsync(uri);

        var matches = TestFetchRegex.Matches(result);

        foreach (Match match in matches)
        {
            var currentCode = match.Groups["Code"].Value;
            Console.WriteLine(currentCode);
            Console.WriteLine("Is this the test input? (Y/n)");
            var yesOrNo = Console.ReadLine();
            if (yesOrNo?.Trim().ToLower() == "n")
            {
                continue;
            }
            
            var lines = currentCode.Split('\n');
            await File.WriteAllLinesAsync(targetFilePath, lines);
            return lines.ToList();
        }
        
        Console.WriteLine("No test input");
        return new List<string>();
    }
}