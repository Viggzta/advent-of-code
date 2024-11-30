using System.Net;

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
}