using Common;

namespace Day2;

public class Solver : ISolver<int[][]>
{
  public int[][] Parse(string[] input) => input.Select(x => x.Split(' ').Select(int.Parse).ToArray()).ToArray();

  public int SolveFirst(int[][] data) => data.Count(report => IsSafe(report));

  public int? SolveSecond(int[][] data) =>
    data.Count(report => IsSafe(report, true) || IsSafe(report.Skip(1).ToArray()));

  static bool IsSafe(int[] report, bool withDampening = false)
  {
    var problemDampened = !withDampening;
    var prev = report[0];
    bool? isIncreasing = null;
    for (var i = 1; i < report.Length; i++)
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
