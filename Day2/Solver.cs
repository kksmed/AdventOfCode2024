using Common;

namespace Day2;

public class Solver : ISolver<int[][], int>
{
  public virtual int Solve(int[][] data) => data.Count(report => IsSafe(report));

  protected static bool IsSafe(int[] report, bool withDampening = false)
  {
    var problemDampened = !withDampening;
    var previous = report.First();
    bool? isIncreasing = null;
    foreach (var current in report.Skip(1))
    {
      var step = current - previous;
      if (Math.Abs(step) > 3)
      {
        if (problemDampened) return false;
        problemDampened = true;
        continue;
      }

      switch (step)
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

      previous = current;
    }

    return true;
  }
}

public class SolverPart2 : Solver
{
  public override int Solve(int[][] data) => data.Count(IsSafe2);

  public static bool IsSafe2(int[] report) => IsSafe(report, true) || IsSafe(report.Skip(1).ToArray());
}
