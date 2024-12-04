using Common;

namespace Day2;

public class Solver2 : ISolver<int[][]>
{
  public int[][] Parse(string[] input) => input.Select(x => x.Split(' ').Select(int.Parse).ToArray()).ToArray();
  public int SolveFirst(int[][] data)
  {
    return data.Count(IsSafe);
  }

  public int? SolveSecond(int[][] data)
  {
    return data.Count(
      report => IsSafe(report) || Enumerable.Range(0, report.Length - 1)
        .Any(x => IsSafe(report.Take(x).Concat(report.Skip(x + 1)).ToArray())));
  }

  static bool IsSafe(int[] report) =>
    (report.SequenceEqual(report.Order()) || report.SequenceEqual(report.OrderDescending()))
    && report.Distinct().Count() == report.Length
    && report.Skip(1).Aggregate(
      (Value: report[0], IsSafe: true),
      (prev, i) => (Value: i, IsSafe: prev.IsSafe && Math.Abs(i - prev.Value) <= 3)).IsSafe;
}
