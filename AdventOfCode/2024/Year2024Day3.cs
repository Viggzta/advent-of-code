using System.Text.RegularExpressions;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day3 : IDay
{
    public Task<string> RunSolution1Async(IList<string> input)
    {
        var inputLine = string.Join("", input);
        Regex regex = new Regex("mul\\((?<val1>[0-9]{1,3}),(?<val2>[0-9]{1,3})\\)");
        var matches = regex.Matches(inputLine);
        var sum = matches
            .Select(
                x => long.Parse(x.Groups["val1"].Value) *
                     long.Parse(x.Groups["val2"].Value))
            .Sum();

        return Task.FromResult(sum.ToString());
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        var inputLine = string.Join("", input);
        
        var doLines = new List<string>();
        var dontLines = new List<string>();

        var start = 0;
        const string _dont = "don't()";
        const string _do = "do()";
        bool isDo = true;
        for (int i = _dont.Length; i < inputLine.Length; i++)
        {
            if (isDo)
            {
                if (inputLine.Substring(i-_dont.Length, _dont.Length) == _dont)
                {
                    doLines.Add(inputLine.Substring(start, i - start));
                    start = i;
                    isDo = false;
                }
            }
            else
            {
                if (inputLine.Substring(i-_do.Length, _do.Length) == _do)
                {
                    dontLines.Add(inputLine.Substring(start, i - start));
                    start = i;
                    isDo = true;
                }
            }
        }
        if (isDo)
        {
            doLines.Add(inputLine.Substring(start));
        }
        else
        {
            dontLines.Add(inputLine.Substring(start));
        }
        
        var newInputLines = string.Join("", doLines);
        Regex regex = new Regex("mul\\((?<val1>[0-9]{1,3}),(?<val2>[0-9]{1,3})\\)");
        var matches = regex.Matches(newInputLines);
        var sum = matches
            .Select(
                x => long.Parse(x.Groups["val1"].Value) *
                     long.Parse(x.Groups["val2"].Value))
            .Sum();

        return Task.FromResult(sum.ToString());
    }
}