using System.Drawing;
using Common;

namespace Day15;

class Parser : IParser<Data>
{
    public Data Parse(string[] input)
    {
        var map = new List<List<Element>>();
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

                if (line.All(x => x == '#'))
                    continue;

                map.Add(
                    line.Where((_, n) => n > 0 && n < line.Length - 1)
                        .Select(x =>
                            x switch
                            {
                                '#' => Element.Wall,
                                '.' => Element.Empty,
                                'O' => Element.Box,
                                '@' => Element.Robot,
                                _ => throw new NotImplementedException(),
                            }
                        )
                        .ToList()
                );
            }
            else
            {
                moves.AddRange(
                    line.Select(x =>
                        x switch
                        {
                            '^' => Direction.Up,
                            '>' => Direction.Right,
                            'v' => Direction.Down,
                            '<' => Direction.Left,
                            _ => throw new NotImplementedException(),
                        }
                    )
                );
            }
        }

        Point start = new(-1, -1);
        var warehouse = new Element[map[0].Count, map.Count];
        for (var y = 0; y < map.Count; y++)
        {
            var xRow = map[y];
            for (var x = 0; x < xRow.Count; x++)
            {
                var element = xRow[x];
                if (element == Element.Robot)
                    start = new(x, y);
                warehouse[x, y] = element;
            }
        }
        return new(start, warehouse, moves.ToArray());
    }
}

record Data(Point Start, Element[,] Warehouse, Direction[] Moves);
