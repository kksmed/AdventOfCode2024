using System.Diagnostics;
using Common;

const string example = """
  1
  10
  100
  2024
  """;

Solving.Go(example, new IntParser(), new Solver(2000));

Console.WriteLine("");
Console.WriteLine("### Part 2 ###");
const string example2 = """
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
      var secret = initialValue;
      for (var i = 0; i < secretCount; i++)
      {
        secret = MonkeyRandom.NextRandom(secret);
      }

      // Console.WriteLine($"{initialValue}: {secret} (in {sw.Elapsed})");
      sum += secret;
    }

    return sum;
  }
}

class Solver2 : ISolver<int[], int>
{
  const int priceChanges = 2000;

  public int Solve(int[] data)
  {
    Dictionary<Sequence, int> sequences = new();
    foreach (var initialValue in data)
    {
      var sw = Stopwatch.StartNew();
      HashSet<Sequence> seen = [];
      var previousChanges = new int[4];
      var secret = initialValue;
      var previousPrice = secret % 10;
      for (var i = 0; i < priceChanges; i++)
      {
        secret = MonkeyRandom.NextRandom(secret);
        var price = secret % 10;
        var change = price - previousPrice;
        previousChanges[i % 4] = change;
        previousPrice = price;

        // First 3 has no sequence
        if (i < 3)
          continue;

        var sequence = Sequence.FromArray(previousChanges, i);
        if (seen.Add(sequence))
          sequences[sequence] = sequences.TryGetValue(sequence, out var total) ? total + price : price;
      }
      // Console.WriteLine($"{initialValue} processed (in {sw.Elapsed})");
    }

    // var orderByMax = sequences.OrderByDescending(x => x.Value);
    // Console.WriteLine("Top-10:");
    // foreach (var kv in orderByMax.Take(10))
    // {
    //   Console.WriteLine($"{kv.Key} gets {kv.Value}");
    // }

    var overallBest = sequences.MaxBy(x => x.Value);
    Console.WriteLine($"Overall best sequence {overallBest.Key}: Giving {overallBest.Value} in total");
    return overallBest.Value;
  }
}

static class MonkeyRandom
{
  /// <summary>
  /// Gets the next pseudo random number from the last one.
  /// </summary>
  /// <remarks>The easy solution would have been to use <see cref="long"/> as the secret type as the number (intermediate
  /// result) gets bigger than <see cref="int.MaxValue"/>. But the higher bits do not impact the result and can be
  /// disregarded.</remarks>
  public static int NextRandom(int secret)
  {
    // Step 1: x64
    secret = MixAndPrune(secret << 6, secret);
    // Step 2: /32
    secret = MixAndPrune(secret >> 5, secret);
    // Step 3: x2048
    secret = MixAndPrune(secret << 11, secret); // We miss information about the higher bits in 'secret << 11', but they do not matter and can be disregarded
    return secret;

    int MixAndPrune(int value, int s)
    {
      // Mix
      value ^= s;

      // As value might have turned negative due to left-shift we first disregard the bits that have no further impact:
      // Prune
      const int twentyThreeOnes = 16777216 - 1;  // 16777216 = 2^24
      value &= twentyThreeOnes;

      return value;
    }
  }
}

record Sequence(int First, int Second, int Third, int Fourth)
{
  public static Sequence FromArray(int[] array, int n) =>
    new(array[(n - 3) % 4], array[(n - 2) % 4], array[(n - 1) % 4], array[n % 4]);

  public override string ToString() => $"{First},{Second},{Third},{Fourth}";
}
