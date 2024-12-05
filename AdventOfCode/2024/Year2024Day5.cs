using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day5 : IDay
{
    public int DayNumber => 5;
    public Task<string> RunSolution1Async(IList<string> input)
    {
        var rules = input
            .TakeWhile(r => !string.IsNullOrWhiteSpace(r))
            .Select(s =>
            {
                var parts = s.Split("|").Select(long.Parse).ToArray();
                return (parts[0], parts[1]);
            })
            .ToList();
        var lists = input
            .Skip(rules.Count + 1)
            .TakeWhile(r => !string.IsNullOrWhiteSpace(r))
            .Select(l => l.Split(',').Select(long.Parse).ToList())
            .ToList();

        long resultSum = 0;
        foreach (var numbers in lists)
        {
            var relevantLocalRules = rules
                .Where(r => numbers.Contains(r.Item1) && numbers.Contains(r.Item2))
                .ToList();
            var localRules = relevantLocalRules
                .DistinctBy(x => x.Item2)
                .ToDictionary(x => x.Item2, y => new List<long>());
            relevantLocalRules
                .ForEach(x => localRules[x.Item2].Add(x.Item1));
            
            bool isValid = true;
            List<long> printedNumbers = new List<long>();
            foreach (var num in numbers)
            {
                if (!localRules.TryGetValue(num, out var requiredNumber))
                {
                    printedNumbers.Add(num);
                    continue;
                }

                if (!requiredNumber.All(r => printedNumbers.Contains(r)))
                {
                    isValid = false;
                    break;
                }
                printedNumbers.Add(num);
            }

            if (isValid)
            {
                resultSum += numbers.Skip(numbers.Count / 2).First();
            }
        }

        return Task.FromResult(resultSum.ToString());
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        var rules = input
            .TakeWhile(r => !string.IsNullOrWhiteSpace(r))
            .Select(s =>
            {
                var parts = s.Split("|").Select(long.Parse).ToArray();
                return (parts[0], parts[1]);
            })
            .ToList();
        var lists = input
            .Skip(rules.Count + 1)
            .TakeWhile(r => !string.IsNullOrWhiteSpace(r))
            .Select(l => l.Split(',').Select(long.Parse).ToList())
            .ToList();

        long resultSum = 0;
        foreach (var numbers in lists)
        {
            var relevantLocalRules = rules
                .Where(r => numbers.Contains(r.Item1) && numbers.Contains(r.Item2))
                .ToList();
            var localRules = relevantLocalRules
                .DistinctBy(x => x.Item2)
                .ToDictionary(x => x.Item2, y => new List<long>());
            relevantLocalRules
                .ForEach(x => localRules[x.Item2].Add(x.Item1));
            var localRules2 = relevantLocalRules
                .DistinctBy(x => x.Item1)
                .ToDictionary(x => x.Item1, y => new List<long>());
            relevantLocalRules
                .ForEach(x => localRules2[x.Item1].Add(x.Item2));

            bool isChanged = false;
            List<long> printedNumbers = new List<long>();
            for (int i = 0; i < numbers.Count; i++)
            {
                var num = numbers[i];
                
                if (!localRules.TryGetValue(num, out var requiredNumber))
                {
                    printedNumbers.Add(num);
                    continue;
                }

                printedNumbers.Add(num);
                if (!requiredNumber.All(r => printedNumbers.Contains(r)))
                {
                    isChanged = true;

                    var maxIndex = localRules[num].Select(x => numbers.IndexOf(x)).Max();
                    numbers.Insert(maxIndex + 1, num);
                    numbers.RemoveAt(i);

                    i = -1;
                    printedNumbers.Clear();
                }
            }

            if (isChanged)
            {
                resultSum += numbers.Skip(numbers.Count / 2).First();
            }
        }

        return Task.FromResult(resultSum.ToString());
    }
}