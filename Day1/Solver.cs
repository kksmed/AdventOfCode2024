using Common;

namespace Day1;

class Parser: IParser<Data>
{
  public Data Parse(string[] input)
  {
    var a = new List<int>();
    var b = new List<int>();
    foreach (var line in input)
    {
      var parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
      if (parts.Length != 2)
      {
        throw new ArgumentException("Invalid input");
      }

      a.Add(int.Parse(parts[0]));
      b.Add(int.Parse(parts[1]));
    }

    return new(A: a, B: b);
  }
}
class Solver1 : ISolver<Data, int>
{
  public int Solve(Data data) =>
    data.A.Order().Zip(data.B.Order()).Select(x => Math.Abs(x.First - x.Second)).Sum();
}

record Data(List<int> A, List<int> B);

class Solver2 : ISolver<Data, int>
{
  public int Solve(Data data)
  {
    var counts = data.B.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
    return data.A.Select(x => x * counts.GetValueOrDefault(x, 0)).Sum();
  }
}
