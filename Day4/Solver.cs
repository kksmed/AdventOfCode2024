using System.Drawing;

using Common;

namespace Day4;

public class Solver : ISolver<char[,]>
{
  public char[,] Parse(string[] input)
  {
    var data = new char[input[0].Length, input.Length];
    for (var y = 0; y < input.Length; y++)
    {
      for (var x = 0; x < input[y].Length; x++)
      {
        data[x, y] = input[y][x];
      }
    }
    return data;
  }

  public int SolveFirst(char[,] data)
  {
    var count = 0;
    for(var x = 0; x < data.GetLength(0); x++)
    {
      for(var y = 0; y < data.GetLength(1); y++)
      {
        var c = data[x, y];
        if (c != 'X') continue;

        var candidates = DetermineCandidates(new(x, y));
        var matches = candidates.Count(candidate => CheckCandidate(data, candidate));
        count += matches;
      }
    }

    return count;
  }

  static IEnumerable<(Point M, Point A, Point S)> DetermineCandidates(Point p)
  {
    var (x, y) = (p.X, p.Y);
    // Forward
    yield return (new(x + 1, y), new(x + 2, y), new(x + 3, y));
    // Backward
    yield return (new(x - 1, y), new(x - 2, y), new(x - 3, y));
    // Downward
    yield return (new(x, y + 1), new(x, y + 2), new(x, y + 3));
    // Upward
    yield return (new(x, y - 1), new(x, y - 2), new(x, y - 3));
    // Diagonal Downward Right
    yield return (new(x + 1, y + 1), new(x + 2, y + 2), new(x + 3, y + 3));
    // Diagonal Downward Left
    yield return (new(x + 1, y - 1), new(x + 2, y - 2), new(x + 3, y - 3));
    // Diagonal Upward Right
    yield return (new(x - 1, y + 1), new(x - 2, y + 2), new(x - 3, y + 3));
    // Diagonal Upward Left
    yield return (new(x - 1, y - 1), new(x - 2, y - 2), new(x - 3, y - 3));
  }

  static bool CheckCandidate(char[,] chars, (Point M, Point A, Point S) candidate)
  {
    var maxX = chars.GetLength(0);
    var maxY = chars.GetLength(1);
    var (m, a, s) = candidate;
    return CheckPoint('M', m) && CheckPoint('A', a) && CheckPoint('S', s);

    bool CheckPoint(char c, Point p) => p.X >= 0 && p.X < maxX && p.Y >= 0 && p.Y < maxY && chars[p.X, p.Y] == c;
  }

  public int? SolveSecond(char[,] data)
  {
    return null;
  }
}
