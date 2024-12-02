using Common;

namespace Day2;

public class Solver : ISolver<int[][]>
{
  public int[][] Parse(string[] input) => input.Select(x => x.Split(' ').Select(int.Parse).ToArray()).ToArray();

  public int SolveFirst(int[][] data)
  {
    var safeCount = 0;
    foreach (var report in data)
    {
      var isSafe = true;
      var prev = report[0];
      bool? isIncreasing = null;
      for(var i = 1; isSafe && i < report.Length; i++)
      {
        var current = report[i];
        var increase = current - prev;
        if (Math.Abs(increase) > 3)
          isSafe = false;
        if (increase == 0)
          isSafe = false;
        if (increase > 0)
        {
          if (isIncreasing == null) isIncreasing = true;
          else if (!isIncreasing.Value) isSafe = false;
        }
        if (increase < 0)
        {
          if (isIncreasing == null) isIncreasing = false;
          else if (isIncreasing.Value) isSafe = false;
        }
        prev = current;
      }
      if (isSafe)
        safeCount++;
    }

    return safeCount;
  }

  public int? SolveSecond(int[][] data)
  {
    return null;
  }
}
