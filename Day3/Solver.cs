using System.Text.RegularExpressions;
using Common;

namespace Day3;

public partial class Parser : IParser<IEnumerable<(int, int)>>
{
    readonly Regex regex = MyRegex();

    public IEnumerable<(int, int)> Parse(string[] input) =>
        input
            .SelectMany(x => regex.Matches(x))
            .Select(x => (int.Parse(x.Groups[1].Value), int.Parse(x.Groups[2].Value)));

    [GeneratedRegex(@"mul\((\d+),(\d+)\)")]
    private static partial Regex MyRegex();
}

public class Solver : ISolver<IEnumerable<(int, int)>, int>
{
    public int Solve(IEnumerable<(int, int)> data) => data.Sum(x => x.Item1 * x.Item2);
}
