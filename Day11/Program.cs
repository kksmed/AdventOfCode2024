using System.Diagnostics;

using Common;

var example= "125 17";

// Solving.Go(example, new Parser(), new Solver(), new Solver2());

var total = Stopwatch.StartNew();
var input = File.ReadAllLines("input.txt");
var data = new Parser().Parse(input);
var answer = new SolverWithTree().Solve(data);
Console.WriteLine($"# Answer Part 2: {answer}");
Console.WriteLine($"# In: {total.Elapsed}");
public class Parser : IParser<int[]>
{
  public int[] Parse(string[] values) => values.SelectMany(x => x.Split(' ')).Select(int.Parse).ToArray();
}

public class Solver : ISolver<int[], int>
{
  protected virtual int Blinks => 25;
  public int Solve(int[] values)
  {
    var stones = values.Select(x => (long)x);
    for (var i = 0; i < Blinks; i++)
    {
      var sw = Stopwatch.StartNew();
      stones = Blink(stones).ToList();
      Console.WriteLine($"Blink {i} {stones.Count()} in {sw.ElapsedMilliseconds}ms");
    }
    return stones.Count();
  }

  static IEnumerable<long> Blink(IEnumerable<long> stones)
  {
    foreach (var stone in stones)
    {
      if (stone == 0)
      {
        yield return 1;
        continue;
      }

      var engraving = stone.ToString();
      var digits = engraving.Length;
      if (digits % 2 == 0)
      {
        var half = digits / 2;
        yield return long.Parse(engraving[..half]);
        yield return long.Parse(engraving[half..]);
        continue;
      }

      yield return stone * 2024;
    }
  }
}

public class Solver2 : Solver
{
  protected override int Blinks => 40;
}

public class SolverWithTree : ISolver<int[], int>
{
  Dictionary<long, Stone> cached = new();
  protected virtual int Blinks => 40;
  public int Solve(int[] values)
  {
    var count = 0;
    foreach (var stone in values)
    {
      var sw = Stopwatch.StartNew();
      List<(long Engraving, int Blinks)> stones = [(stone, 0)];
      for(var i = 0; i < stones.Count; i++)
      {
        var (engraving, blinks) = stones[i];
        if (cached.TryGetValue(engraving, out var cachedStone))
        {
          
        }

        for (var b = blinks + 1; b <= Blinks; b++)
        {
          (engraving, var extra) = Blink(engraving);
          if (extra.HasValue)
            stones.Add((extra.Value, b));
        }
      }

      count += stones.Count;
      Console.WriteLine($"Blink {count} {stone} in {sw.Elapsed}");
    }

    return count;
  }

  Stone BuildTree(long engraving, int blinks)
  {
    // Root element

    for (int i = 0; i < blinks; i++)
    {
      if (cached.TryGetValue(engraving, out var cachedStone))
      {

    }
      if (cachedStone.Next.HasValue)
        return cachedStone.Next.Value.Count;
      else
      {
        
      }
    }
    else
    {
      
    }
    
  }
  
  int Count(Stone stone, int blinks)
  {
    var current = stone;
    for(var b = 0; b < blinks; b++)
    {
      var next = stone.Next.Value;
      if (stone.Next == null)
        stone.Next = new[] { new Stone(Blink(stone.Engraving).Item1) };
      else
        stone.Next = stone.Next.Select(x => new Stone(Blink(x.Engraving).Item1)).ToArray();
    }
    if (stone.Next == null)
      return 1;
    return stone.Next.Sum(Count);
  }
  
  static (long, long?) Blink(long stone)
  {
    if (stone == 0)
    {
      return (1, null);
    }

    var engraving = stone.ToString();
    var digits = engraving.Length;
    if (digits % 2 == 0)
    {
      var half = digits / 2;
      return (long.Parse(engraving[..half]), long.Parse(engraving[half..]));
    }

    return (stone * 2024, null);
  }
}

record Stone(long Engraving)
{
  public (Stone, Stone?)? Next { get; set; }
}
