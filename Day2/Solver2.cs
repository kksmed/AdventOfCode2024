using Common;

namespace Day2;

public class Solver2Part1 : ISolver<int[][], int>
{
  public virtual int Solve(int[][] data) => data.Count(IsSafe);

  protected static bool IsSafe(int[] report) =>
    (report.SequenceEqual(report.Order()) || report.SequenceEqual(report.OrderDescending()))
    && report.Distinct().Count() == report.Length
    && report.SkipLast(1).Zip(report.Skip(1)).Select(x => Math.Abs(x.First - x.Second)).All(x => x <= 3);
}

public class Solver2Part2 : Solver2Part1
{
  public override int Solve(int[][] data) => data.Count(IsSafe2);

  public static bool IsSafe2(int[] report) => IsSafe(report) || Enumerable.Range(0, report.Length)
    .Any(x => IsSafe(report.Take(x).Concat(report.Skip(x + 1)).ToArray()));
}