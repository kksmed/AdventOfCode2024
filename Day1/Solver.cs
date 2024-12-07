using Common;

namespace Day1;

class Solver : ISolverLegacy<(List<int> A, List<int> B)>
{
  public (List<int> A, List<int> B) Parse(string[] input)
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

    return (A: a, B: b);
  }

  public int SolveFirst((List<int> A, List<int> B) data) =>
    data.A.Order().Zip(data.B.Order()).Select(x => Math.Abs(x.First - x.Second)).Sum();

  public int? SolveSecond((List<int> A, List<int> B) data)
  {
    var counts = data.B.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
    return data.A.Select(x => x * counts.GetValueOrDefault(x, 0)).Sum();
  }
}
