using Common;

var example= "125 17";

Solving.Go(example, new Parser(), new Solver(), new Solver2());

public class Parser : IParser<int[]>
{
  public int[] Parse(string[] values) => values.SelectMany(x => x.Split(' ')).Select(int.Parse).ToArray();
}

public class Solver : ISolver<int[], int>
{
  protected virtual int Blinks => 25;
  public int Solve(int[] values)
  {
    var stones = values.Select(x => (long)x);
    for (var i = 0; i < Blinks; i++)
    {
      stones = Blink(stones);
    }
    return stones.Count();
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
  protected override int Blinks => 75;
}