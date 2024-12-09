using Common;

var example = "2333133121414131402";

Solving.Go(example, new Parser(), new Solver());

class Parser : IParser<int?[]>
{
  public int?[] Parse(string[] input) =>
    input
      .SelectMany(x => x)
      .Select(x => x - '0')
      .SelectMany((x, i) => Enumerable.Repeat(i % 2 == 0 ? i / 2 : (int?)null, x))
      .ToArray();
}

class Solver : ISolver<int?[], long>
{
  public long Solve(int?[] input) => GetCheckSum(Defragment(input));

  static long GetCheckSum(int?[] disk) => disk.Select((x, i) => (x ?? 0L) * i).Sum();

  static int?[] Defragment(int?[] disk)
  {
    var result = (int?[])disk.Clone();
    var i = 0;
    var j = disk.Length - 1;
    i = NextSpace();
    j = NextFragment();
    while (i < j && i >= 0 && j >= 0)
    {
      result[i] = result[j];
      result[j] = null;
      i = NextSpace();
      j = NextFragment();
    }

    return result;

    int NextSpace() => Array.FindIndex(result, i, j - i, x => x == null);
    int NextFragment() => Array.FindLastIndex(result, j, j - i, x => x != null);
  }
}
