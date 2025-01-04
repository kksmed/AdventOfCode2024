using System.Drawing;
using Common;

var example = """
    89010123
    78121874
    87430965
    96549874
    45678903
    32019012
    01329801
    10456732
    """;

Solving.Go(example, new IntMapParser(), new Solver(), new Solver2());

public class Solver : ISolver<int[,], int>
{
    public virtual int Solve(int[,] data) =>
        FindTrailheads(data).Select(x => FindTops(data, x).Distinct().Count()).Sum();

    protected static IEnumerable<Point> FindTrailheads(int[,] map)
    {
        for (var x = 0; x < map.GetLength(0); x++)
        for (var y = 0; y < map.GetLength(1); y++)
            if (map[x, y] == 0)
                yield return new(x, y);
    }

    protected static IEnumerable<Point> FindTops(int[,] map, Point trailhead)
    {
        var value = map[trailhead.X, trailhead.Y];
        return FindTopsInner(trailhead, value);

        IEnumerable<Point> FindTopsInner(Point p, int v)
        {
            if (v == 9)
                yield return p;
            var nextValue = v + 1;
            var up = p with { Y = p.Y - 1 };
            if (map.InBounds(up) && map[up.X, up.Y] == nextValue)
                foreach (var top in FindTopsInner(up, nextValue))
                    yield return top;

            var down = p with { Y = p.Y + 1 };
            if (map.InBounds(down) && map[down.X, down.Y] == nextValue)
                foreach (var top in FindTopsInner(down, nextValue))
                    yield return top;

            var left = p with { X = p.X - 1 };
            if (map.InBounds(left) && map[left.X, left.Y] == nextValue)
                foreach (var top in FindTopsInner(left, nextValue))
                    yield return top;

            var right = p with { X = p.X + 1 };
            if (map.InBounds(right) && map[right.X, right.Y] == nextValue)
                foreach (var top in FindTopsInner(right, nextValue))
                    yield return top;
        }
    }
}

public class Solver2 : Solver
{
    public override int Solve(int[,] data) =>
        FindTrailheads(data).Select(x => FindTops(data, x).Count()).Sum();
}
