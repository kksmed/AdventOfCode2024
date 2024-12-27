using System.Diagnostics;
using System.Text.RegularExpressions;

using Common;

var example =
  """
  r, wr, b, g, bwu, rb, gb, br

  brwrr
  bggr
  gbbr
  rrbgbr
  ubwu
  bwurrg
  brgr
  bbrgwb
  """;

Solving.Go(example, new Parser(), new Solver());

class Parser : IParser<Data>
{

  public Data Parse(string[] input)
  {
    return new(input[0].Split(", "), input.Skip(2).ToArray());
  }
}

class Solver : ISolver<Data, int>
{
  public int Solve(Data data)
  {
    var timeout = TimeSpan.FromSeconds(5);
    var regex = new Regex("^(" + string.Join("|", data.Towels.Select(Regex.Escape)) + ")+$", RegexOptions.IgnoreCase, timeout);
    var count = 0;
    foreach (var p in data.Patterns)
    {
      var sw = Stopwatch.StartNew();
      try
      {
        var isMatch = regex.IsMatch(p);
        if (isMatch)
          count++;
        sw.Stop();
        Console.WriteLine($"{p} - {isMatch} - in {sw.Elapsed} (ticks: {sw.ElapsedTicks})");
      }
      catch (RegexMatchTimeoutException)
      {
        Console.WriteLine($"{p} - Timed out (after {timeout})");
      }
    }

    return count;
  }

  static bool IsSolvable(string[] towels, string pattern) =>
    pattern == "" || towels.Any(x => pattern.StartsWith(x) && IsSolvable(towels, pattern[x.Length..]));
}

record Data(string[] Towels, string[] Patterns);

