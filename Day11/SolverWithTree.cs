using System.Diagnostics;

using Common;

public class SolverWithTree(int Blinks = 75) : ISolver<int[], long>
{
  readonly Dictionary<long, List<long>> countCache = new();

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

  static int Count(LazyStone stone, int blinks)
  {
    if (blinks == 0)
    {
      // Console.Write($"{stone.Engraving} ");
      return 1;
    }

    blinks--;

    var next = stone.Next;

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
