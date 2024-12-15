using System.Diagnostics;

using Common;

public class SolverWithTree(int Blinks = 75) : ISolver<int[], long>
{
  readonly Dictionary<LazyStone, List<long>> countCache = new();

  public long Solve(int[] values)
  {
    var count = 0L;
    foreach (var value in values)
    {
      var sw = Stopwatch.StartNew();
      var stone = new LazyStone(value);
      var subCount = Count(stone, Blinks);
      Console.WriteLine($"{value}: Count: {subCount} in {sw.Elapsed}");

      count += subCount;
    }

    return count;
  }

  long Count(LazyStone stone, int blinks)
  {
    while(stone.Next.Item2 == null && blinks > 1)
    {
      stone = stone.Next.Item1;
      blinks--;
    }

    if (blinks == 0)
    {
      return 1;
    }

    // Do not lookup when blinks is 1 (maybe a bit higher should also avoid cache-lookup)
    if (blinks == 1)
    {
      return stone.Next.Item2 == null ? 1 : 2;
    }

    if (countCache.TryGetValue(stone, out var counts) && counts.Count > blinks)
    {
      return counts[blinks];
    }

    var next = stone.Next;

    return Count(next.Item1, blinks) + (next.Item2 == null ? 0 : Count(next.Item2, blinks));
  }

  int Count2(LazyStone stone, int blinks)
  {
    var count = 0;
    var stack = new Stack<(LazyStone Stone, int Blinks)>();
    stack.Push((stone, blinks));
    while(stack.Count > 0)
    {
      var (current, blinksToGo) = stack.Pop();
      for(var b = 1; b <= blinksToGo; b++)
      {
        var next = current.Next;
        if (next.Item2 != null)
          stack.Push((next.Item2, blinksToGo - b));

        current = next.Item1;
      }

      count++;
    }

    return count;
  }

  public void BuildCache() => CountBuildCache(new (0), Blinks);

  long CountBuildCache(LazyStone start, int blinks)
  {
    if (blinks == 1)
      return start.Next.Item2 == null ? 1 : 2;

    if (!countCache.TryGetValue(start, out var counts))
    {
      counts = [1];
      countCache[start] = counts;
    }
    else if (counts.Count > blinks)
      return counts[blinks];

    var startBlinks = 1;
    while (start.Next.Item2 == null && startBlinks < blinks)
    {
      startBlinks++;
      counts.Add(1);
      start = start.Next.Item1;
    }

    if (start.Next.Item2 == null)
      return 1;

    var first = start.Next.Item1;
    var second = start.Next.Item2;

    var count = 1L;
    for (var b = startBlinks; b < blinks; b++)
    {
      var count1 = CountBuildCache(first, b);
      var count2 = CountBuildCache(second, b);
      count = count1 + count2;
      counts.Add(count1 + count2);
    }

    return count;
  }

  class LazyStone
  {
    static readonly Dictionary<long, LazyStone> treeCache = new();
    public long Engraving { get; }

    readonly Lazy<(LazyStone, LazyStone?)> lazyNext;
    public (LazyStone, LazyStone?) Next => lazyNext.Value;

    public LazyStone(long engraving)
    {
      Engraving = engraving;
      lazyNext = new(() =>
        {
          var next = Blink();
          return (GetStone(next.Item1), next.Item2.HasValue ? GetStone(next.Item2.Value) : null);
        });
    }

    static LazyStone GetStone(long engraving)
    {
      if (treeCache.TryGetValue(engraving, out var nextStone))
        return nextStone;

      nextStone = new(engraving);
      treeCache[engraving] = nextStone;
      return nextStone;
    }

    (long, long?) Blink()
    {
      if (Engraving == 0)
      {
        return (1, null);
      }

      var engravingText = Engraving.ToString();
      var digits = engravingText.Length;
      if (digits % 2 == 0)
      {
        var half = digits / 2;
        return (long.Parse(engravingText[..half]), long.Parse(engravingText[half..]));
      }

      return (Engraving * 2024, null);
    }
  }
}
