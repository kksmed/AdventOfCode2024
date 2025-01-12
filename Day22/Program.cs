using System.Diagnostics;
using Common;

var testValue = 123;
var example = """
  1
  10
  100
  2024
  """;

Solving.Go(example, new IntParser(), new Solver(2000));

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
        secret = NextRandom(secret);
      }

      Console.WriteLine($"{initialValue}: {secret} (in {sw.Elapsed})");
      sum += secret;
    }

    return sum;
  }

  internal long NextRandom(long secret)
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
