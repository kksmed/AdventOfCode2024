using System.Drawing;

using Common;

namespace Day4;

public class Solver1 : ISolver<char[,], int>
{
  protected virtual char PatternStarter => 'X';

  public int Solve(char[,] data)
  {
    var count = 0;
    for(var x = 0; x < data.GetLength(0); x++)
    {
      for(var y = 0; y < data.GetLength(1); y++)
      {
        var c = data[x, y];
        if (c != PatternStarter) continue;

        var candidates = DetermineCandidates(new(x, y));
        var matches = candidates.Count(candidate => CheckCandidate(data, candidate));
        count += matches;
      }
    }

    return count;
  }

  protected virtual IEnumerable<(Point, char)[]> DetermineCandidates(Point p)
  {
    var (x, y) = (p.X, p.Y);
    // Forward
    yield return [(new(x + 1, y), 'M'), (new(x + 2, y), 'A'), (new(x + 3, y),'S')];
    // Backward
    yield return [(new(x - 1, y), 'M'), (new(x - 2, y), 'A'), (new(x - 3, y),'S')];
    // Downward
    yield return [(new(x, y + 1), 'M'), (new(x, y + 2), 'A'), (new(x, y + 3),'S')];
    // Upward
    yield return [(new(x, y - 1), 'M'), (new(x, y - 2), 'A'), (new(x, y - 3),'S')];
    // Diagonal Downward Right
    yield return [(new(x + 1, y + 1), 'M'), (new(x + 2, y + 2), 'A'), (new(x + 3, y + 3),'S')];
    // Diagonal Downward Left
    yield return [(new(x + 1, y - 1), 'M'), (new(x + 2, y - 2), 'A'), (new(x + 3, y - 3),'S')];
    // Diagonal Upward Right
    yield return [(new(x - 1, y + 1), 'M'), (new(x - 2, y + 2), 'A'), (new(x - 3, y + 3),'S')];
    // Diagonal Upward Left
    yield return [(new(x - 1, y - 1), 'M'), (new(x - 2, y - 2), 'A'), (new(x - 3, y - 3),'S')];
  }

  static bool CheckCandidate(char[,] chars, IEnumerable<(Point Point, char C)> candidate)
  {
    var maxX = chars.GetLength(0);
    var maxY = chars.GetLength(1);
    return candidate.All(x => CheckPoint(x.Point, x.C));

    bool CheckPoint(Point p, char c) => p.X >= 0 && p.X < maxX && p.Y >= 0 && p.Y < maxY && chars[p.X, p.Y] == c;
  }
}

public class Solver2 : Solver1
{
  protected override char PatternStarter => 'A';

  protected override IEnumerable<(Point, char)[]> DetermineCandidates(Point p)
  {
    var (x, y) = (p.X, p.Y);
    var topLeft = new Point(x - 1, y - 1);
    var topRight = new Point(x + 1, y - 1);
    var bottomLeft = new Point(x - 1, y + 1);
    var bottomRight = new Point(x + 1, y + 1);
    // Downward
    yield return [(topLeft, 'M'), (topRight, 'M'), (bottomLeft, 'S'), (bottomRight, 'S')];
    // Upward
    yield return [(bottomLeft, 'M'), (bottomRight, 'M'), (topLeft, 'S'), (topRight, 'S')];
    // Forward
    yield return [(topLeft, 'M'), (bottomLeft, 'M'), (topRight, 'S'), (bottomRight, 'S')];
    // Backward
    yield return [(topRight, 'M'), (bottomRight, 'M'), (topLeft, 'S'), (bottomLeft, 'S')];
  }
}