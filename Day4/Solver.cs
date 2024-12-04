using Common;

namespace Day4;

public class Solver : ISolver<char[,]>
{
  public char[,] Parse(string[] input)
  {
    var data = new char[input.Length, input[0].Length];
    for (var i = 0; i < input.Length; i++)
    {
      for (var j = 0; j < input[i].Length; j++)
      {
        data[i, j] = input[i][j];
      }
    }
    return data;
  }

  public int SolveFirst(char[,] data)
  {
    throw new NotImplementedException();
  }

  public int? SolveSecond(char[,] data)
  {
    return null;
  }
}
