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

// Solving.Go(example, new Parser(), new Solver());

//Solving.Go(example, new Parser(), new Solver2());

Solving.Go(null, new Parser(), new Solver2());
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
    var timeout = TimeSpan.FromSeconds(1);
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
}

class Solver2 : ISolver<Data, int>
{
  public int Solve(Data data)
  {
    var swTreeBuilding = Stopwatch.StartNew();
    TowelNode tree = new(' ');
    foreach (var towel in data.Towels)
    {
      var node = tree;
      foreach (var c in towel)
      {
        node = c switch
        {
          'w' => node.White ??= new(c),
          'u' => node.Blue ??= new(c),
          'b' => node.Black ??= new(c),
          'r' => node.Red ??= new(c),
          'g' => node.Green ??= new(c),
          _ => throw new InvalidOperationException()
        };
      }
      if (node.Towel != null)
        throw new InvalidOperationException("Duplicate towel!");
      node.Towel = towel;
    }
    swTreeBuilding.Stop();
    Console.WriteLine($"Tree built in {swTreeBuilding.Elapsed} (ticks: {swTreeBuilding.ElapsedTicks})");
    var count = 0;
    foreach (var p in data.Patterns)
    {
      var sw = Stopwatch.StartNew();
      var matches = FindMatches(tree, p);
      count += matches;
      sw.Stop();
      Console.WriteLine($"{p} - {matches} - in {sw.Elapsed} (ticks: {sw.ElapsedTicks})");
    }

    return count;
  }

  static int FindMatches(TowelNode towels, string pattern)
  {
    if (pattern.Length == 0)
      return 1;

    var count = 0;
    var node = towels;
    for (var i = 0; i < pattern.Length; i++)
    {
      var c = pattern[i];
      var newNode = c switch
      {
        'w' => node.White,
        'u' => node.Blue,
        'b' => node.Black,
        'r' => node.Red,
        'g' => node.Green,
        _ => throw new InvalidOperationException()
      };

      if (newNode == null)
        return count;

      node = newNode;
      if (node.Towel != null)
      {
        count += FindMatches(towels, pattern[(i + 1)..]);
      }
    }

    return count;
  }
}

record Data(string[] Towels, string[] Patterns);

record TowelNode(char Color)
{
  public string? Towel { get; set; }
  public TowelNode? White { get; set; } // (w)
  public TowelNode? Blue { get; set; } // (u)
  public TowelNode? Black { get; set; } // (b)
  public TowelNode? Red { get; set; } // (r)
  public TowelNode? Green { get; set; } // (g)
}
