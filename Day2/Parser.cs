using Common;

namespace Day2;

public class Parser : IParser<int[][]>
{
  public int[][] Parse(string[] input) => input.Select(x => x.Split(' ').Select(int.Parse).ToArray()).ToArray();
}
