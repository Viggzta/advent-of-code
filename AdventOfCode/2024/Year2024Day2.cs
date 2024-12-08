using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day2 : IDay
{
    public Task<string> RunSolution1Async(IList<string> input)
    {
        input = input.Take(input.Count - 1).ToList();
        double safeResults = 0;

        foreach (var report in input)
        {
            var reportVals = report.Split(" ").Select(double.Parse).ToList();
            (bool isSafe, bool isIncrease) safeAndDirection = IsCompareSafe(reportVals[0], reportVals[1]);
            var isSafe = safeAndDirection.isSafe;
            for (int i = 1; i < reportVals.Count; i++)
            {
                if (!isSafe) break;
                isSafe &= IsCompareSafe(
                    reportVals[i - 1],
                    reportVals[i],
                    safeAndDirection.isIncrease);
            }
            safeResults += isSafe ? 1 : 0;
        }
        
        return Task.FromResult(safeResults.ToString());
    }

    private (bool isSafe, bool isIncrease) IsCompareSafe(double prev, double next)
    {
        var dist = Math.Abs(prev - next);
        return (prev > next || prev < next && dist is <= 3 and > 0, prev < next);
    }
    
    private bool IsCompareSafe(double prev, double curr, bool isIncrease)
    {
        var dist = Math.Abs(prev - curr);
        if (isIncrease)
        {
            return prev < curr && dist is <= 3 and > 0;
        }
        return prev > curr && dist is <= 3 and > 0;
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        input = input.Take(input.Count - 1).ToList();
        double safeResults = 0;

        foreach (var report in input)
        {
            var reportVals = report.Split(" ").Select(double.Parse).ToList();

            var reportVariations = new List<List<double>>();
            for (int i = 0; i < reportVals.Count; i++)
            {
                var copy = reportVals.ToList();
                copy.RemoveAt(i);
                reportVariations.Add(copy);
            }

            var isSafe = false;
            foreach (var r in reportVariations)
            {
                isSafe |= IsReportSafe(r);
            }
            safeResults += isSafe ? 1 : 0;
        }
        
        return Task.FromResult(safeResults.ToString());
    }

    public bool IsReportSafe(List<double> reportVals)
    {
        (bool isSafe, bool isIncrease) safeAndDirection = IsCompareSafe(reportVals[0], reportVals[1]);
        var isSafe = safeAndDirection.isSafe;
        for (int i = 1; i < reportVals.Count; i++)
        {
            if (!isSafe) return false;
            isSafe &= IsCompareSafe(
                reportVals[i - 1],
                reportVals[i],
                safeAndDirection.isIncrease);
        }

        return isSafe;
    }
}