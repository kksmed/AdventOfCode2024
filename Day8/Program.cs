
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

Solving.Go(example, new CharMapParser(), new Part1());

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

  static IEnumerable<Point> FindAntinodes(Point a1, Point a2, char[,] map)
  {
    var diff = (X: a1.X - a2.X, Y: a1.Y - a2.Y);

    var antinode = Plus(a1, diff);
    if (map.InBounds(antinode))
      yield return antinode;

    antinode = Minus(a2, diff);
    if (map.InBounds(antinode))
      yield return antinode;

    yield break;

    Point Plus(Point p, (int X, int Y) d) => new(p.X + d.X, p.Y + d.Y);
    Point Minus(Point p, (int X, int Y) d) => new(p.X - d.X, p.Y - d.Y);
  }
}