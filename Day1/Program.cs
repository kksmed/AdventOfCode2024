var example =
  """
  3   4
  4   3
  2   5
  1   3
  3   9
  3   3
  """;

var exDistance = FindDistanceSum(example.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

Console.WriteLine($"Example - distance: {exDistance}");

var exSimilarity = FindSimilaritySum(example.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries));

Console.WriteLine($"Example - similarity: {exSimilarity}");

var distanceSum = FindDistanceSum(File.ReadAllLines("input.txt"));

Console.WriteLine($"Distance: {distanceSum}");

var similaritySum = FindSimilaritySum(File.ReadAllLines("input.txt"));

Console.WriteLine($"Similarity: {similaritySum}");

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

int FindDistanceSum(string[] input)
{
  var (a, b) = Parse(input);
  var i = a.Order().Zip(b.Order()).Select(x => Math.Abs(x.First - x.Second)).Sum();
  return i;
}

int FindSimilaritySum(string[] input)
{
  var (a, b) = Parse(input);
  var counts = b.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
  return a.Select(x=> x * counts.GetValueOrDefault(x, 0)).Sum();
}