using System.Diagnostics;
using System.Text.RegularExpressions;
using AdventOfCode.Utility;

namespace AdventOfCode._2024;

public class Year2024Day4 : IDay
{
    public int DayNumber => 4;
    
    public Task<string> RunSolution1Async(IList<string> input)
    {
        input = input.Take(input.Count - 1).ToList();
        var founds = 0;

        var xmas = "XMAS";
        var samx = "SAMX";
        for (var i = 0; i < input.Count; i++)
        {
            for (int j = 0; j < input[0].Length; j++)
            {
                var ul = "";
                for (int k = 0; k < xmas.Length; k++)
                {
                    try
                    {
                        ul += input[i - k][j - k];
                    }
                    catch
                    {
                        // ignored
                    }
                }
                if (ul == xmas || ul == samx) founds++;
                
                var ur = "";
                for (int k = 0; k < xmas.Length; k++)
                {
                    try
                    {
                        ur += input[i - k][j + k];
                    }
                    catch
                    {
                    }
                }
                if (ur == xmas || ur == samx) founds++;
                
                var u = "";
                for (int k = 0; k < xmas.Length; k++)
                {
                    try
                    {
                        u += input[i - k][j];
                    }
                    catch
                    {
                    }
                }
                if (u == xmas || u == samx) founds++;
                
                var r = "";
                for (int k = 0; k < xmas.Length; k++)
                {
                    try
                    {
                        r += input[i][j + k];
                    }
                    catch
                    {
                    }
                }
                if (r == xmas || r == samx) founds++;
            }
        }
        
        return Task.FromResult(founds.ToString());
    }

    public Task<string> RunSolution2Async(IList<string> input)
    {
        input = input.Take(input.Count - 1).ToList();
        var founds = 0;

        var mas = "MAS";
        for (var i = 1; i < input.Count-1; i++)
        {
            for (int j = 1; j < input[0].Length-1; j++)
            {
                if (input[i][j] != 'A') continue;
                var diag1 = input[i - 1][j - 1].ToString() + input[i][j].ToString() + input[i + 1][j + 1].ToString();
                var diag2 = input[i - 1][j + 1].ToString() + input[i][j].ToString() + input[i + 1][j - 1].ToString();
                if ((diag1 == "MAS" || diag1 == "SAM") && (diag2 == "MAS" || diag2 == "SAM"))
                {
                    founds++;
                }
            }
        }
        
        return Task.FromResult(founds.ToString());
    }
}