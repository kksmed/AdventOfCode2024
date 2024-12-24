using System.Drawing;

using Common;

namespace Day15;

class Parser2 : IParser<(Point Start, Element2[,] Warehouse, Direction[] Moves)>
{
  public (Point Start, Element2[,] Warehouse, Direction[] Moves) Parse(string[] input)
  {
    var map = new List<List<Element2>>();
    var moves = new List<Direction>();
    var isMap = true;
    foreach (var line in input)
    {
      if (isMap)
      {
        if (line.Length == 0)
        {
          isMap = false;
          continue;
        }

        map.Add(line.SelectMany(x => x switch
        {
          '#' => new[]
          {
            Element2.Wall, Element2.Wall
          },
          '.' => new[]
          {
            Element2.Empty, Element2.Empty
          },
          'O' => new[]
          {
            Element2.BoxLeft, Element2.BoxRight
          },
          '@' => new[]
          {
            Element2.Robot, Element2.Empty
          },
          _ => throw new NotImplementedException()
        }).ToList());
      }
      else
      {
        moves.AddRange(line.Select(x => x switch
        {
          '^' => Direction.Up,
          '>' => Direction.Right,
          'v' => Direction.Down,
          '<' => Direction.Left,
          _ => throw new NotImplementedException()
        }));
      }
    }

    Point start = new(-1, -1);
    var warehouse = new Element2[map[0].Count, map.Count];
    for (var y = 0; y < map.Count; y++)
    {
      var xRow = map[y];
      for (var x = 0; x < xRow.Count; x++)
      {
        var element = xRow[x];
        if (element == Element2.Robot)
          start = new(x, y);
        warehouse[x, y] = element;
      }
    }
    return (start, warehouse, moves.ToArray());
  }
}