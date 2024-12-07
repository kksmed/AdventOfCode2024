using System.Text.RegularExpressions;

using Common;

namespace Day3;

public partial class Solver : ISolverLegacy<IEnumerable<(int, int)>>
{
  readonly Regex regex = MyRegex();

  public IEnumerable<(int, int)> Parse(string[] input) =>
    input.SelectMany(x => regex.Matches(x))
      .Select(x => (int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value)));

  public int SolveFirst(IEnumerable<(int, int)> data) => data.Sum(x => x.Item1 * x.Item2);

  public int? SolveSecond(IEnumerable<(int, int)> data) => null;

  [GeneratedRegex(@"mul\((\d+),(\d+)\)")]
  private static partial Regex MyRegex();
}
