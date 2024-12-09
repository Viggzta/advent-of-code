using System.Diagnostics;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day9 : IDay
{
    public Task<string> RunSolution1Async(IList<string> input)
    {
        var line = input.First();
        List<(int index, int count)> inputPattern = line
            .Index()
            .Select(x => 
                (x.Index % 2 == 0
                    ? (x.Index + 2) / 2 - 1
                    : -1,
                    int.Parse(x.Item.ToString())))
            .ToList();

        for (int i = 0; i < inputPattern.Count; i++)
        {
            var current = inputPattern[i];
            if (current.index != -1 || current.count == 0)
            {
                continue;
            }

            var lastRealNum = inputPattern
                .Index()
                .LastOrDefault(x => x.Item.index != -1 && x.Item.count != 0);
            var take = Math.Min(lastRealNum.Item.count, current.count);
            // move values
            inputPattern[i] = (current.index, current.count - take);
            inputPattern[lastRealNum.Index] = (lastRealNum.Item.index, lastRealNum.Item.count - take);
            inputPattern.Insert(i, (lastRealNum.Item.index, take));
        }

        long result = 0;
        long index = 0;
        foreach (var curr in inputPattern.Where(x => x.count > 0))
        {
            foreach (var j in Enumerable.Range(1, curr.count))
            {
                result += index * curr.index;
                index++;
            }
        }

        /*
        var tempOut = "";
        foreach (var valueTuple in inputPattern)
        {
            foreach (var i in Enumerable.Range(1, valueTuple.count))
            {
                tempOut += valueTuple.index == -1 ? '.' : valueTuple.index;
            }
        }

        Debug.Assert(tempOut == "0099811188827773336446555566");
        Console.WriteLine(tempOut);
        */
        return Task.FromResult(result.ToString());
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        var line = input.First();
        List<(int number, int count)> inputPattern = line
            .Index()
            .Select(x => 
                (x.Index % 2 == 0
                    ? (x.Index + 2) / 2 - 1
                    : -1,
                    int.Parse(x.Item.ToString())))
            .ToList();

        for (int i = inputPattern.Count - 1; i >= 0; i--)
        {
            /*
            Console.WriteLine(GetLine(inputPattern));
            */
            var current = inputPattern[i];
            if (current.number == -1 || current.count == 0)
            {
                continue;
            }

            (int Index, (int index, int count) Item) firstRealBlank = inputPattern
                .Index()
                .FirstOrDefault(x => x.Item.number == -1
                    && x.Item.count != 0
                    && x.Item.count >= current.count
                    && x.Index < i);
            if (firstRealBlank == default) continue; // Might be prone to errors
            
            var take = Math.Min(firstRealBlank.Item.count, current.count);
            // move values
            var firstIndex = firstRealBlank.Index;
                
            var remaining = inputPattern[firstIndex].count - current.count;
            inputPattern[firstIndex] = (
                current.number,
                current.count);
            // deplete
            var totalCount = current.count;
            var tempIndex = i;
            if (firstRealBlank.Index + 1 < inputPattern.Count && inputPattern[firstRealBlank.Index + 1].number == -1)
            {
                totalCount += inputPattern[firstRealBlank.Index + 1].count;
                inputPattern.RemoveAt(firstRealBlank.Index + 1);
            }
            if (inputPattern[firstRealBlank.Index - 1].number == -1)
            {
                totalCount += inputPattern[firstRealBlank.Index - 1].count;
                inputPattern.RemoveAt(firstRealBlank.Index - 1);
                tempIndex--;
            }
            inputPattern[tempIndex] = (index: -1, totalCount);
            
            // input remaining
            if (remaining > 0)
            {
                inputPattern.Insert(firstIndex + 1, (-1, remaining));
                i++;
            }
        }

        long result = 0;
        long index = 0;
        foreach (var valueTuple in inputPattern)
        {
            foreach (var i in Enumerable.Range(1, valueTuple.count))
            {
                result += valueTuple.number == -1 ? 0 : index * valueTuple.number;
                index++;
            }
        }

        // Console.WriteLine(GetLine(inputPattern));
        return Task.FromResult(result.ToString());
    }

    public string GetLine(List<(int number, int count)> inputPattern)
    {
        var tempOut = "";
        foreach (var valueTuple in inputPattern)
        {
            foreach (var i in Enumerable.Range(1, valueTuple.count))
            {
                tempOut += valueTuple.number == -1 ? "." : valueTuple.number.ToString();
            }
        }
        return tempOut;
    }
}