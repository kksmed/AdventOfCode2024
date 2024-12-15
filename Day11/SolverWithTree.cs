using System.Diagnostics;

using Common;

public class SolverWithTree(int Blinks = 75) : ISolver<int[], long>
{
  readonly Dictionary<long, Stone> treeCache = new();
  readonly Dictionary<long, List<long>> countCache = new();

  public long Solve(int[] values)
  {
    var count = 0L;
    foreach (var stone in values)
    {
      var sw = Stopwatch.StartNew();
      var tree = BuildTree(stone, 0);
      Console.WriteLine($"{stone}: Tree build in {sw.Elapsed}");
      var subCount2 = Count2(tree);
      var subCount = Count3(tree, Blinks);
      Console.WriteLine($"{stone}: Count: {subCount} in {sw.Elapsed}");
      if (subCount2 != subCount)
      {
        Console.WriteLine($"### FEJL! ### {stone}: Blinks: {Blinks} Count2: {subCount2} vs Count3: {subCount}");
        return -1;
      }

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
      for(var b = 1; b <= blinks; b++)
      {
        var next = current.Next ?? throw new InvalidOperationException("Tree incomplete");
        if (next.Item2 != null)
          stack.Push((next.Item2, blinks - b));

        current = next.Item1;
      }

      count++;
    }

    return count;
  }

  // Abondoned
  long Count3(Stone stone, int blinks)
  {
    if (blinks == 0)
      return 1;

    if (!countCache.TryGetValue(stone.Engraving, out var counts))
    {
      counts = [1];
      countCache[stone.Engraving] = counts;
    }
    else if (counts.Count > blinks)
    {
      Console.WriteLine($"{stone.Engraving}: - CACHED - Blinks: {blinks} Count: {counts[blinks]}");
      return counts[blinks];
    }

    var extras = 0L;
    List<Stone> current = [stone];
    for(var b = 1; b <= blinks; b++)
    {
      var newStone = new List<Stone>();
      foreach (var c in current)
      {
        var next = c.Next ?? throw new InvalidOperationException("Tree incomplete");
        newStone.Add(next.Item1);

        if (next.Item2 == null)
          continue;

        var blinksLeft = blinks - b;
        if (countCache.TryGetValue(next.Item2.Engraving, out var nextCounts) && nextCounts.Count > blinksLeft)
          extras += nextCounts[blinksLeft];
        else
          newStone.Add(next.Item2);
      }

      if (b == counts.Count)
        counts.Add(newStone.LongCount() + extras);

      current = newStone;
    }

    var count = current.LongCount() + extras;
    Console.WriteLine($"{stone.Engraving}: Blinks: {blinks} Count: {count}");
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
