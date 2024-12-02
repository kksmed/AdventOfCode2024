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
    return data.Count(report => IsSafe(report, true) || IsSafe(report.Skip(1).ToArray()));

    bool IsSafe(int[] report, bool withDampening = false)
    {
      var problemDampened = !withDampening;
      var prev = report[0];
      bool? isIncreasing = null;
      for(var i = 1; i < report.Length; i++)
      {
        var current = report[i];
        var increase = current - prev;
        if (Math.Abs(increase) > 3)
        {
          if (problemDampened) return false;
          problemDampened = true;
          continue;
        }

        switch (increase)
        {
          case 0 when problemDampened:
            return false;
          case 0:
            problemDampened = true;
            continue;
          case > 0 when isIncreasing == null:
            isIncreasing = true;
            break;
          case > 0 when !isIncreasing.Value:
          {
            if (problemDampened) return false;
            problemDampened = true;
            continue;
          }
          case < 0 when isIncreasing == null:
            isIncreasing = false;
            break;
          case < 0 when isIncreasing.Value:
          {
            if (problemDampened) return false;
            problemDampened = true;
            continue;
          }
        }

        prev = current;
      }
      return true;
    }
  }
}
