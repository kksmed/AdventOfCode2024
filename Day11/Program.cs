﻿using System.Diagnostics;

using Common;

var example= "125 17";

// Solving.Go(example, new Parser(), new SolverWithTree());

var total = Stopwatch.StartNew();
var input = File.ReadAllLines("input.txt");
var data = new Parser().Parse(input);
var answer = new SolverWithTree(50).Solve(data);
Console.WriteLine($"# Answer Part 2: {answer}");
Console.WriteLine($"# In: {total.Elapsed}");
public class Parser : IParser<int[]>
{
  public int[] Parse(string[] values) => values.SelectMany(x => x.Split(' ')).Select(int.Parse).ToArray();
}

public class Solver : ISolver<int[], int>
{
  protected virtual int Blinks => 6;
  public int Solve(int[] values)
  {
    var stones = values.Select(x => (long)x).ToList();
    for (var i = 0; i < Blinks; i++)
    {
      var sw = Stopwatch.StartNew();
      stones = Blink(stones).ToList();
      // Console.WriteLine($"Blink {i} {stones.Count()} in {sw.ElapsedMilliseconds}ms");
    }

    Console.WriteLine($"Stones: {string.Join(", ",stones)}");
    return stones.Count;
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

public class SolverWithTree(int Blinks = 75) : ISolver<int[], int>
{
  readonly Dictionary<long, Stone> treeCache = new();
  readonly Dictionary<long, List<int>> countCache = new();

  public int Solve(int[] values)
  {
    var count = 0;
    foreach (var stone in values)
    {
      var sw = Stopwatch.StartNew();
      var tree = BuildTree(stone, 0);
      Console.WriteLine($"{stone}: Tree build in {sw.Elapsed}");
      var subCount = Count2(tree);
      Console.WriteLine($"{stone}: Count: {subCount} in {sw.Elapsed}");
      count += subCount;
    }

    return count;
  }

  Stone BuildTree(long engraving, int blinks)
  {
    if (treeCache.TryGetValue(engraving, out var cachedStone))
    {
      ExpandTree(cachedStone, Blinks - blinks, blinks);
      if (cachedStone.Depth < Blinks - blinks) throw new InvalidOperationException("Tree incomplete");
      return cachedStone;
    }

    var stone = new Stone(engraving);
    treeCache[engraving] = stone;

    if (blinks == Blinks) return stone;

    SetNext(stone, blinks);

    return stone;
  }

  void ExpandTree(Stone stone, int requiredDepth, int blinks)
  {
    if (stone.Depth >= requiredDepth)
      return;

    if (stone.Next == null)
    {
      SetNext(stone, blinks);
      return;
    }

    var next = stone.Next ?? throw new InvalidOperationException("Tree incomplete");

    ExpandTree(next.Item1, requiredDepth - 1, blinks + 1);

    if (next.Item2 != null)
      ExpandTree(next.Item2, requiredDepth - 1, blinks + 1);

    stone.Depth = int.Min(next.Item1.Depth, next.Item2?.Depth ?? int.MaxValue) + 1;
    if (stone.Depth < Blinks - blinks) throw new InvalidOperationException("Tree incomplete");
  }

  int Count(Stone stone, int blinks)
  {
    if (blinks == Blinks)
    {
      // Console.Write($"{stone.Engraving} ");
      return 1;
    }

    blinks++;

    var next = stone.Next ?? throw new InvalidOperationException("Tree incomplete");

    return Count(next.Item1, blinks) + (next.Item2 == null ? 0 : Count(next.Item2, blinks));
  }

  int Count2(Stone stone)
  {
    var count = 0;
    var stack = new Stack<(Stone Stone, int Blinks)>();
    stack.Push((stone, Blinks));
    while(stack.Count > 0)
    {
      var (current, blinks) = stack.Pop();
      for(var i = 0; i < blinks; i++)
      {
        var next = current.Next ?? throw new InvalidOperationException("Tree incomplete");
        if (next.Item2 != null)
          stack.Push((next.Item2, blinks - 1-i));

        current = next.Item1;
      }

      count++;
    }

    return count;
  }

  void SetNext(Stone stone, int blinks)
  {
    var (engraving1, engraving2) = Blink(stone.Engraving);
    var next = (BuildTree(engraving1, blinks + 1),
      engraving2.HasValue ? BuildTree(engraving2.Value, blinks + 1) : null);
    stone.Next = next;
    stone.Depth = int.Min(next.Item1.Depth, next.Item2?.Depth ?? int.MaxValue) + 1;
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

public class Solver4(int Blinks) : ISolver<int[], int>
{
  //readonly Dictionary<long, Stone> treeCache = new();
  Dictionary<long, CachedStone> countCache = new();

  public int Solve(int[] data)
  {
    var count = 0;
    foreach (var stone in data)
    {
      var sw = Stopwatch.StartNew();
      var subCount = Count(stone, Blinks);
      Console.WriteLine($"{stone}: Count: {subCount} in {sw.Elapsed}");
      count += subCount;
    }

    return count;
  }

  int Count(int engraving, int blinks)
  {
    if (blinks == 0)
      return 1;

    CachedStone? cachedStone;
    if (!countCache.TryGetValue(engraving, out cachedStone))
    {
      var s = new Stone(engraving);
      cachedStone = new(s, [1], [s]);
      countCache[engraving] = cachedStone;
    }
    else if (cachedStone.Counts.Count >= blinks)
      return cachedStone.Counts[blinks];

    var stones = cachedStone.MostBlinks;
    for(var i = cachedStone.Counts.Count; i < blinks; i++)
    {
      var newStones = new List<Stone>();
      foreach (var stone in stones)
      {
        var (engraving1, engraving2) = Blink(stone.Engraving);
        if(!countCache.TryGetValue(engraving1, out var cachedStone1))
        {
          var s1 = new Stone(engraving1);
          cachedStone1 = new(s1, [1], [s1]);
          countCache[engraving1] = cachedStone1;
        }
        var 
        var count2 = engraving2.HasValue ? Count(engraving2.Value, blinks - 1 - i) : 0;
        count += count1 + count2;
      }
      var (engraving1, engraving2) = Blink(engraving);
      var count1 = Count(engraving1, blinks - 1 - i);
      var count2 = engraving2.HasValue ? Count(engraving2.Value, blinks - 1 - i) : 0;
      count += count1 + count2;
    }
    ExpandTree(stone, Blinks - blinks, blinks);
      if (stone.Depth < Blinks - blinks) throw new InvalidOperationException("Tree incomplete");
      return stone;
    }
    while(stack.Count > 0)
    {
      var (current, blinks) = stack.Pop();
      for(var i = 0; i < blinks; i++)
      {
        var next = current.Next ?? throw new InvalidOperationException("Tree incomplete");
        if (next.Item2 != null)
          stack.Push((next.Item2, blinks - 1-i));

        current = next.Item1;
      }

      count++;
    }

    return count; 
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

  record CachedStone(Stone Stone, List<int> Counts, List<Stone> MostBlinks);
}

record Stone(long Engraving)
{
  public (Stone, Stone?)? Next { get; set; }

  public int Depth { get; set; }
}
