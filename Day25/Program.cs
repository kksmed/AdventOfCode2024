using Common;
using InvalidOperationException = System.InvalidOperationException;

var example = """
  #####
  .####
  .####
  .####
  .#.#.
  .#...
  .....

  #####
  ##.##
  .#.##
  ...##
  ...#.
  ...#.
  .....

  .....
  #....
  #....
  #...#
  #.#.#
  #.###
  #####

  .....
  .....
  #.#..
  ###..
  ###.#
  ###.#
  #####

  .....
  .....
  .....
  #....
  #.#..
  #.#.#
  #####

  """;

Solving.Go(example, new Parser(), new Solver());

class Parser(int width = 5, int height = 5) : IParser<Data>
{
  public Data Parse(string[] input)
  {
    List<int[]> locks = [];
    List<int[]> keys = [];

    for (var r = 0; r < input.Length; r += 8)
    {
      var first = input[r];

      switch (first)
      {
        case "#####":
          locks.Add(ParseLock(input, r));
          break;
        case ".....":
          keys.Add(ParseKey(input, r));
          break;
        default:
          throw new InvalidOperationException($"Unexpected first line: {first} at line: {r}");
      }
    }
    return new(locks, keys);
  }

  int[] ParseLock(string[] input, int start)
  {
    var l = new int[width];
    for (var i = 0; i < width; i++)
    {
      var h = 0;
      for (var j = 0; j < height; j++)
      {
        if (input[start + j + 1][i] != '#')
          break;
        h++;
      }
      l[i] = h;
    }

    return l;
  }

  int[] ParseKey(string[] input, int start)
  {
    start += 6;
    var l = new int[width];
    for (var i = 0; i < width; i++)
    {
      var h = 0;
      for (var j = 0; j < height; j++)
      {
        if (input[start - j - 1][i] != '#')
          break;
        h++;
      }
      l[i] = h;
    }

    return l;
  }
}

record Data(List<int[]> Locks, List<int[]> Keys);

class Solver(int height = 5) : ISolver<Data, int>
{
  public int Solve(Data data) => data.Locks.Sum(l => data.Keys.Count(k => l.Zip(k).All(x => x.First + x.Second <= height)));
}
