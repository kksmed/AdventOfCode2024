using Common;

namespace Day2;

public class Solver2 : ISolverLegacy<int[][]>
{
  public int[][] Parse(string[] input) => input.Select(x => x.Split(' ').Select(int.Parse).ToArray()).ToArray();
  public int SolveFirst(int[][] data) => data.Count(IsSafe);

  public int? SolveSecond(int[][] data) => data.Count(IsSafe2);

  public static bool IsSafe2(int[] report) => IsSafe(report) || Enumerable.Range(0, report.Length)
    .Any(x => IsSafe(report.Take(x).Concat(report.Skip(x + 1)).ToArray()));

  static bool IsSafe(int[] report) =>
    (report.SequenceEqual(report.Order()) || report.SequenceEqual(report.OrderDescending()))
    && report.Distinct().Count() == report.Length
    && report.SkipLast(1).Zip(report.Skip(1)).Select(x => Math.Abs(x.First - x.Second)).All(x => x <= 3);
}
