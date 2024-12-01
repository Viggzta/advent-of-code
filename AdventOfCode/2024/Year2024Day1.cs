using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day1 : IDay
{
    public int DayNumber => 1;
    
    public Task<string> RunSolution1Async(IList<string> input)
    {
        input = input.Take(input.Count - 1).ToList();
        List<double> listA = new List<double>();
        List<double> listB = new List<double>();
        
        foreach (var s in input)
        {
            var nums = s.Split("   ");
            listA.Add(double.Parse(nums[0]));
            listB.Add(double.Parse(nums[1]));
        }
        
        listA.Sort();
        listB.Sort();

        double answer = 0;
        for (int i = 0; i < listA.Count; i++)
        {
            answer += Math.Abs(listA[i] - listB[i]);
        }
        
        return Task.FromResult(answer.ToString());
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        input = input.Take(input.Count - 1).ToList();
        Dictionary<double, double> dict1 = new Dictionary<double, double>();
        Dictionary<double, double> dict2 = new Dictionary<double, double>();
        
        foreach (var s in input)
        {
            var nums = s.Split("   ");
            var a = double.Parse(nums[0]);
            var b = double.Parse(nums[1]);

            if (!dict1.ContainsKey(a))
            {
                dict1.Add(a, 0);
            }
            if (!dict2.ContainsKey(b))
            {
                dict2.Add(b, 0);
            }
            
            dict1[a]++;
            dict2[b]++;
        }
        
        double answer = 0;
        foreach (var kvp in dict1)
        {
            answer += kvp.Key * kvp.Value * (dict2.TryGetValue(kvp.Key, out var value) ? value : 0);
        }
        
        return Task.FromResult(answer.ToString());
    }
}