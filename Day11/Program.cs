using System.Diagnostics;

using Common;

var example= "125 17";

// Solving.Go(example, new Parser(), new Solver(), new Solver2());

var total = Stopwatch.StartNew();
var input = File.ReadAllLines("input.txt");
var data = new Parser().Parse(input);
var answer = new Solver2().Solve(data);
Console.WriteLine($"# Answer Part 2: {answer}");
Console.WriteLine($"# In: {total.Elapsed}");
public class Parser : IParser<string[]>
{
  public string[] Parse(string[] values) => values.SelectMany(x => x.Split(' ')).ToArray();
}

public class Solver : ISolver<string[], int>
{
  protected virtual int Blinks => 25;
  public int Solve(string[] values)
  {
    var stones = values.ToList();
    for (var i = 0; i < Blinks; i++)
    {
      var sw = Stopwatch.StartNew();
      stones = Blink(stones).ToList();
      Console.WriteLine($"Blink {i} {stones.Count} in {sw.ElapsedMilliseconds}ms");
    }
    return stones.Count();
  }

  static IEnumerable<string> Blink(IEnumerable<string> stones)
  {
    foreach (var stone in stones)
    {
      if (stone == "0")
      {
        yield return "1";
        continue;
      }

      var digits = stone.Length;
      if (digits % 2 == 0)
      {
        var half = digits / 2;
        yield return stone[..half];
        yield return stone[half..].TrimStart('0');
        continue;
      }

      yield return (long.Parse(stone) * 2024).ToString();
    }
  }
}

public class Solver2 : Solver
{
  protected override int Blinks => 40;
}