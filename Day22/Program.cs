using System.Diagnostics;
using Advent.UseCases.Day22;
using Common;

// var example = """
//   1
//   10
//   100
//   2024
//   """;
//
// // Solving.Go(example, new IntParser(), new Solver(2000));
//
// Console.WriteLine("### Part 2 ###");
// var example2 = """
//   1
//   2
//   3
//   2024
//   """;
// Solving.Go(example2, new IntParser(), new Solver2(new(-2, 1, -1, 3)), false);
Solving.Go(null, new IntParser(), new Solver2(new(-2, 3, -3, 3)));

Solving.Go(null, new NoParser(), new Day22Part2Solver(2000));

//AlternativeSolution.Main();

class IntParser : IParser<int[]>
{
  public int[] Parse(string[] input) => input.Select(int.Parse).ToArray();
}

class Solver(int secretCount) : ISolver<int[], int>
{
  public int Solve(int[] data)
  {
    var sum = 0;

    foreach (var initialValue in data)
    {
      var sw = Stopwatch.StartNew();
      var secret = initialValue;
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

class Solver2(Sequence expectedBest) : ISolver<int[], int>
{
  const int secretCount = 2000;

  public int Solve(int[] data)
  {
    var t = 0;
    Dictionary<Sequence, int> sequences = new();
    foreach (var initialValue in data)
    {
      var sw = Stopwatch.StartNew();
      HashSet<Sequence> seen = [];
      var previousChanges = new int[4];
      var previousPrice = 0; // initial value is not used
      var secret = initialValue;
      for (var i = 0; i < secretCount; i++)
      {
        secret = MonkeyRandom.NextRandom(secret);
        var price = secret % 10;

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
        if (seen.Add(sequence))
        {
          sequences[sequence] = sequences.TryGetValue(sequence, out var total) ? total + price : price;
          if (sequence == expectedBest)
            t += price;
        }
      }
    }

    // var orderByMax = totalBest.OrderByDescending(x => x.Value);
    // Console.WriteLine("Top-10:");
    // foreach (var kv in orderByMax.Take(10))
    // {
    //   Console.WriteLine($"{kv.Key} gets {kv.Value}");
    // }

    var overallBest = sequences.MaxBy(x => x.Value);
    Console.WriteLine($"Overall best sequence {overallBest.Key}: Giving {overallBest.Value} in total");
    Console.WriteLine($"t = {t}");
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
      // As value might have turned negative due to left-shift we first disregard the bits that have no further impact:
      const int twentyThreeOnes = (1 << 24) - 1;
      value &= twentyThreeOnes;

      // Mix
      value ^= s;
      // Prune
      value %= 16777216; // 2^24
      return value;
    }
  }
}

record Sequence(int First, int Second, int Third, int Fourth)
{
  public static Sequence FromArray(int[] array, int n) =>
    new(array[(n - 3) % 4], array[(n - 2) % 4], array[(n - 1) % 4], array[n % 4]);
}
