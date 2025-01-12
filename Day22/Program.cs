using System.Diagnostics;
using Common;

var example = """
  1
  10
  100
  2024
  """;

Solving.Go(example, new IntParser(), new Solver(2000));

Console.WriteLine("### Part 2 ###");
var example2 = """
  1
  2
  3
  2024
  """;
Solving.Go(example2, new IntParser(), new Solver2(), false);
Solving.Go(null, new IntParser(), new Solver2());

class IntParser : IParser<int[]>
{
  public int[] Parse(string[] input) => input.Select(int.Parse).ToArray();
}

class Solver(int secretCount) : ISolver<int[], long>
{
  public long Solve(int[] data)
  {
    var sum = 0L;

    foreach (var initialValue in data)
    {
      var sw = Stopwatch.StartNew();
      long secret = initialValue;
      for (var i = 0; i < secretCount; i++)
      {
        secret = MonkeyRandom.NextRandom(secret);
      }

      Console.WriteLine($"{initialValue}: {secret} (in {sw.Elapsed})");
      sum += secret;
    }

    return sum;
  }
}

class Solver2 : ISolver<int[], int>
{
  const int secretCount = 2000;

  public int Solve(int[] data)
  {
    Dictionary<Sequence, int> totalBest = new();
    foreach (var initialValue in data)
    {
      var sw = Stopwatch.StartNew();
      Dictionary<Sequence, int> best = new();
      var previousChanges = new int[4];
      var previousPrice = 0; // initial value is not used
      long secret = initialValue;
      for (var i = 0; i < secretCount; i++)
      {
        secret = MonkeyRandom.NextRandom(secret);
        var price = (int)(secret % 10);

        // First
        if (i == 0)
        {
          previousPrice = price;
          continue;
        }

        var change = price - previousPrice;
        previousChanges[i % 4] = change;
        previousPrice = price;

        // First 3 has no sequence
        if (i < 3)
          continue;

        var sequence = Sequence.FromArray(previousChanges, i);
        best.TryAdd(sequence, price);
      }

      foreach (var kv in best)
      {
        totalBest[kv.Key] = totalBest.TryGetValue(kv.Key, out var total) ? total + kv.Value : kv.Value;
      }

      Console.WriteLine($"{initialValue}: {secret} (in {sw.Elapsed})");
    }

    var overallBest = totalBest.MaxBy(x => x.Value);
    Console.WriteLine($"Overall best sequence {overallBest.Key}: Giving {overallBest.Value} in total");
    return overallBest.Value;
  }
}

static class MonkeyRandom
{
  public static long NextRandom(long secret)
  {
    // Step 1: x64
    secret = MixAndPrune(secret << 6, secret);
    // Step 2: /32
    secret = MixAndPrune(secret >> 5, secret);
    // Step 3: x2048
    secret = MixAndPrune(secret << 11, secret);
    return secret;

    long MixAndPrune(long value, long s)
    {
      // Mix
      s ^= value;
      // Prune
      s %= 16777216; // 2^24
      return s;
    }
  }
}

record Sequence(int First, int Second, int Third, int Fourth)
{
  public static Sequence FromArray(int[] array, int n) =>
    new(array[(n - 3) % 4], array[(n - 2) % 4], array[(n - 1) % 4], array[n % 4]);
}
