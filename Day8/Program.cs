
using System.Drawing;

using Common;

var example =
  """
  ............
  ........0...
  .....0......
  .......0....
  ....0.......
  ......A.....
  ............
  ............
  ........A...
  .........A..
  ............
  ............
  """;

Solving.Go(example, new CharMapParser(), new Part1(), new Part2());

Console.WriteLine("");
var example2 =
  """
  T.........
  ...T......
  .T........
  ..........
  ..........
  ..........
  ..........
  ..........
  ..........
  ..........
  """;

Solving.Go1(example2, new CharMapParser(), new Part2());

class Part1 : ISolver<char[,], int>
{
  public int Solve(char[,] data)
  {
    List<Point> antinodes = [];
    foreach (var type in GetAntennaTypes(data))
    {
      var antennas = FindAntennas(data, type).ToArray();
      for (var i = 0; i < antennas.Length; i++)
      {
        var a1 = antennas[i];
        for (var j = i + 1; j < antennas.Length; j++)
        {
          var a2 = antennas[j];
          antinodes.AddRange(FindAntinodes(a1, a2, data));
        }
      }
    }

    return antinodes.Distinct().Count();
  }

  static IEnumerable<char> GetAntennaTypes(char[,] map) => map.Cast<char>().Where(x => x != '.').Distinct();

  static IEnumerable<Point> FindAntennas(char[,] map, char type)
  {
    for (var x = 0; x < map.GetLength(0); x++)
    {
      for (var y = 0; y < map.GetLength(1); y++)
      {
        if (map[x, y] == type)
        {
          yield return new(x, y);
        }
      }
    }
  }

  protected virtual IEnumerable<Point> FindAntinodes(Point a1, Point a2, char[,] map)
  {
    var diff = (X: a1.X - a2.X, Y: a1.Y - a2.Y);

    var antinode = Plus(a1, diff);
    if (map.InBounds(antinode))
      yield return antinode;

    antinode = Minus(a2, diff);
    if (map.InBounds(antinode))
      yield return antinode;
  }

  protected static Point Plus(Point p, (int X, int Y) d) => new(p.X + d.X, p.Y + d.Y);
  protected static Point Minus(Point p, (int X, int Y) d) => new(p.X - d.X, p.Y - d.Y);
}

class Part2 : Part1
{
  protected override IEnumerable<Point> FindAntinodes(Point a1, Point a2, char[,] map)
  {
    yield return a1;
    yield return a2;
    var diff = (X: a1.X - a2.X, Y: a1.Y - a2.Y);

    var antinode = Plus(a1, diff);
    while (map.InBounds(antinode))
    {
      yield return antinode;
      antinode = Plus(antinode, diff);
    }

    antinode = Minus(a2, diff);
    while (map.InBounds(antinode))
    {
      yield return antinode;
      antinode = Minus(antinode, diff);
    }
  }
}