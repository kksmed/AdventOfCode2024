using System.Diagnostics;
using System.Numerics;

using Common;

using Day11;

var example= "125 17";

Solving.Go(example, new Parser(), new Solver(25), new Solver2(75));

// var total = Stopwatch.StartNew();
// var input = File.ReadAllLines("input.txt");
// var data = new Parser().Parse(input);
// var answer = new SolverWithTree().Solve(data);
// Console.WriteLine($"# Answer Part 2: {answer}");
// Console.WriteLine($"# In: {total.Elapsed}");

public class Parser : IParser<int[]>
{
  public int[] Parse(string[] values) => values.SelectMany(x => x.Split(' ')).Select(int.Parse).ToArray();
}

public class Solver(int Blinks) : ISolver<int[], int>
{
  public int Solve(int[] values)
  {
    var stones = values.Select(x => (long)x).ToList();
    for (var i = 0; i < Blinks; i++)
    {
      stones = Blink(stones).ToList();
    }

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

public class Solver2(int Blinks) : ISolver<int[], BigInteger>
{
  public BigInteger Solve(int[] values)
  {
    var stones = values.ToDictionary(x => new BigInteger(x), x => BigInteger.One);
    for (var i = 0; i < Blinks; i++)
    {
      stones = Blink(stones).GroupBy(x => x.Key).ToDictionary(x => x.Key, x => x.Aggregate(BigInteger.Zero, (acc, y) => acc + y.Value));
    }

    return stones.Values.Aggregate(BigInteger.Add);
  }

  static IEnumerable<KeyValuePair<BigInteger, BigInteger>> Blink(IEnumerable<KeyValuePair<BigInteger, BigInteger>> stones)
  {
    foreach (var stone in stones)
    {
      if (stone.Key.IsZero)
      {
        yield return new(BigInteger.One, stone.Value);
        continue;
      }

      var engraving = stone.Key.ToString();
      var digits = engraving.Length;
      if (digits % 2 == BigInteger.Zero)
      {
        var half = digits / 2;
        yield return new(BigInteger.Parse(engraving[..half]), stone.Value);
        yield return new(BigInteger.Parse(engraving[half..]), stone.Value);
        continue;
      }

      yield return new(stone.Key * 2024, stone.Value);
    }
  }
}


// public class Solver4(int Blinks) : ISolver<int[], int>
// {
//   //readonly Dictionary<long, Stone> treeCache = new();
//   Dictionary<long, CachedStone> countCache = new();
//
//   public int Solve(int[] data)
//   {
//     var count = 0;
//     foreach (var stone in data)
//     {
//       var sw = Stopwatch.StartNew();
//       var subCount = Count(stone, Blinks);
//       Console.WriteLine($"{stone}: Count: {subCount} in {sw.Elapsed}");
//       count += subCount;
//     }
//
//     return count;
//   }
//
//   int Count(int engraving, int blinks)
//   {
//     if (blinks == 0)
//       return 1;
//
//     CachedStone? cachedStone;
//     if (!countCache.TryGetValue(engraving, out cachedStone))
//     {
//       var s = new Stone(engraving);
//       cachedStone = new(s, [1], [s]);
//       countCache[engraving] = cachedStone;
//     }
//     else if (cachedStone.Counts.Count >= blinks)
//       return cachedStone.Counts[blinks];
//
//     var stones = cachedStone.MostBlinks;
//     for(var i = cachedStone.Counts.Count; i < blinks; i++)
//     {
//       var newStones = new List<Stone>();
//       foreach (var stone in stones)
//       {
//         var (engraving1, engraving2) = Blink(stone.Engraving);
//         if(!countCache.TryGetValue(engraving1, out var cachedStone1))
//         {
//           var s1 = new Stone(engraving1);
//           cachedStone1 = new(s1, [1], [s1]);
//           countCache[engraving1] = cachedStone1;
//         }
//         var 
//         var count2 = engraving2.HasValue ? Count(engraving2.Value, blinks - 1 - i) : 0;
//         count += count1 + count2;
//       }
//       var (engraving1, engraving2) = Blink(engraving);
//       var count1 = Count(engraving1, blinks - 1 - i);
//       var count2 = engraving2.HasValue ? Count(engraving2.Value, blinks - 1 - i) : 0;
//       count += count1 + count2;
//     }
//     ExpandTree(stone, Blinks - blinks, blinks);
//       if (stone.Depth < Blinks - blinks) throw new InvalidOperationException("Tree incomplete");
//       return stone;
//     }
//     while(stack.Count > 0)
//     {
//       var (current, blinks) = stack.Pop();
//       for(var i = 0; i < blinks; i++)
//       {
//         var next = current.Next ?? throw new InvalidOperationException("Tree incomplete");
//         if (next.Item2 != null)
//           stack.Push((next.Item2, blinks - 1-i));
//
//         current = next.Item1;
//       }
//
//       count++;
//     }
//
//     return count; 
//   }

// static (long, long?) Blink(long stone)
// {
//   if (stone == 0)
//   {
//     return (1, null);
//   }
//
//   var engraving = stone.ToString();
//   var digits = engraving.Length;
//   if (digits % 2 == 0)
//   {
//     var half = digits / 2;
//     return (long.Parse(engraving[..half]), long.Parse(engraving[half..]));
//   }
//
//   return (stone * 2024, null);
// }
//
//   record CachedStone(Stone Stone, List<int> Counts, List<Stone> MostBlinks);

record Stone(long Engraving)
{
  public (Stone, Stone?)? Next { get; set; }

  public int Depth { get; set; }
}
