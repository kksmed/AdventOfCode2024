var example =
  """
  3   4
  4   3
  2   5
  1   3
  3   9
  3   3
  """;

var input = example.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
var (a, b) = Parse(input);
var distance = a.Order().Zip(b.Order()).Select(x => Math.Abs(x.First - x.Second)).Sum();

Console.WriteLine($"Distance: {distance}");

return;

(List<int> a, List<int> b) Parse(string[] input)
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

  return (a, b);
}