using System.Diagnostics;
using System.Drawing;
using Common;
using ArgumentException = System.ArgumentException;
using InvalidOperationException = System.InvalidOperationException;

namespace Day21;

class StringSolver(int Layers) : ISolver<string[], long>
{
  public long Solve(string[] data)
  {
    var complexitySum = 0L;
    foreach (var doorCode in data)
    {
      var sw = Stopwatch.StartNew();
      var sequences = ConvertToDirections(doorCode).GroupBy(x => x).ToDictionary(x => x.Key, x => x.LongCount());
      for (var i = 0; i < Layers; i++)
      {
        Dictionary<string, long> newSequences = new();
        foreach (var sequence in sequences)
        foreach (var nextLayerSequence in AddLayer(sequence.Key))
        {
          var previousCount = newSequences.GetValueOrDefault(nextLayerSequence);
          newSequences[nextLayerSequence] = previousCount + sequence.Value;
        }

        sequences = newSequences;
      }

      var codeNumber = int.Parse(doorCode[..3]);
      var lengthOfShortestSequence = sequences.Sum(x => x.Key.Length * x.Value);
      var complexity = codeNumber * lengthOfShortestSequence;
      complexitySum += complexity;
      Console.WriteLine($"Complexity ({lengthOfShortestSequence}x{codeNumber}): {complexity} in {sw.Elapsed}");
    }

    return complexitySum;
  }

  static IEnumerable<string> AddLayer(string sequence)
  {
    Point gap = new(0, 0);
    Point position = new(2, 0); // A
    foreach (var newPosition in sequence.Select(GetDirectionalKeyPosition))
    {
      yield return GetDirectionsToPressKeyTheShortestWay(position, newPosition, gap);
      position = newPosition;
    }
  }

  static IEnumerable<string> ConvertToDirections(string doorCode)
  {
    Point gap = new(0, 3);
    Point position = new(2, 3);
    foreach (var newPosition in doorCode.Select(GetNumericKeyPosition))
    {
      yield return GetDirectionsToPressKeyTheShortestWay(position, newPosition, gap);
      position = newPosition;
    }
  }

  static readonly Dictionary<(Point From, Point To, Point Gap), string> cache = new();

  static string GetDirectionsToPressKeyTheShortestWay(Point from, Point to, Point gap)
  {
    var cacheKey = (from, to, gap);
    if (cache.TryGetValue(cacheKey, out var cashedValue))
      return cashedValue;

    var moves = MoveToKey(from, to).ToArray();
    var permutations = GetPermutations(moves).Where(x => AvoidGap(x, from, gap)).Select(x => $"{x}A").ToList();

    if (permutations.Count == 0)
      throw new InvalidOperationException($"Cannot move between {from} -> {to}");

    var best = FindShortestPermutation(permutations).BestPermutation;
    cache[cacheKey] = best;

    return best;
  }

  static (string BestPermutation, string FinalSequence, int Layers) FindShortestPermutation(List<string> permutations)
  {
    const int maxLayering = 3;
    switch (permutations.Count)
    {
      case 0:
        throw new ArgumentException("Must be at least one.", nameof(permutations));
      case 1:
        return (permutations.Single(), permutations.Single(), 0);
    }

    var layeredPermutations = permutations.ToDictionary(x => x, x => new List<string> { x });
    var layer = 0;
    for (; layer < maxLayering; layer++)
    {
      foreach (var permutation in layeredPermutations)
      {
        var newPermutations = permutation.Value.SelectMany(GetLayeredOptions).ToList();
        var subMinLength = newPermutations.Select(x => x.Length).Min();

        permutation.Value.Clear();
        permutation.Value.AddRange(newPermutations.Where(x => x.Length == subMinLength));
      }

      var minLength = layeredPermutations.Select(x => x.Value.First().Length).Min();

      layeredPermutations = layeredPermutations
        .Where(x => x.Value.First().Length == minLength)
        .ToDictionary(x => x.Key, x => x.Value);

      if (layeredPermutations.Count == 1)
      {
        var best = layeredPermutations.Single();
        return (best.Key, best.Value.First(), layer);
      }
    }

    // TODO: How to pick if more?
    var winningSequence = layeredPermutations.First();
    if (winningSequence.Value.Count < 1)
      throw new InvalidOperationException("No winning sequence");

    return (winningSequence.Key, winningSequence.Value.First(), layer);
  }

  static IEnumerable<string> GetLayeredOptions(string sequence)
  {
    if (sequence.Length == 0)
      throw new ArgumentOutOfRangeException(nameof(sequence), sequence, "Must have at least one!");

    Point gap = new(0, 0);

    List<string> permutations = [string.Empty];
    Point position = new(2, 0); // A
    foreach (var keyToPush in sequence)
    {
      var newPosition = GetDirectionalKeyPosition(keyToPush);

      var cacheKey = (position, newPosition, gap);

      if (cache.TryGetValue(cacheKey, out var cashedValue))
      {
        permutations = permutations.Select(x => $"{x}{cashedValue}").ToList();
        position = newPosition;
        continue;
      }

      var moves = MoveToKey(position, GetDirectionalKeyPosition(keyToPush)).ToArray();
      var subPermutations = GetPermutations(moves).Where(x => AvoidGap(x, position, gap)).Select(x => $"{x}A").ToList();
      if (subPermutations.Count == 0)
        throw new InvalidOperationException($"Cannot move from {position} -> {newPosition} to push {keyToPush}");

      if (subPermutations.Count == 1)
      {
        cache[cacheKey] = subPermutations.Single();
      }

      permutations = permutations.SelectMany(head => subPermutations.Select(tail => $"{head}{tail}")).ToList();

      position = newPosition;
    }

    if (permutations.Count == 0)
      throw new InvalidOperationException("Cannot layer!");

    var minLength = permutations.MinBy(x => x.Length)?.Length;
    return permutations.Where(x => x.Length == minLength);
  }

  static bool AvoidGap(string moves, Point from, Point gap)
  {
    var position = from;
    foreach (var move in moves)
    {
      position = move switch
      {
        'A' => throw new InvalidOperationException("Should never happen!"),
        '^' => position with { Y = position.Y - 1 },
        '<' => position with { X = position.X - 1 },
        'v' => position with { Y = position.Y + 1 },
        '>' => position with { X = position.X + 1 },
        _ => throw new ArgumentOutOfRangeException(nameof(moves), move, null),
      };

      if (position == gap)
        return false;
    }
    return true;
  }

  static List<string> GetPermutations(char[] elements, int recursionDepth = 0, int? maxDepth = null)
  {
    maxDepth ??= elements.Length - 1;

    if (maxDepth < 0)
      return [string.Empty];

    if (recursionDepth == maxDepth)
    {
      return [new(elements)];
    }

    List<string> permutations = [];
    for (var i = recursionDepth; i <= maxDepth; i++)
    {
      Swap(ref elements[recursionDepth], ref elements[i]);
      permutations.AddRange(GetPermutations(elements, recursionDepth + 1, maxDepth));
      // backtrack
      Swap(ref elements[recursionDepth], ref elements[i]);
    }

    return permutations.Distinct().ToList();

    static void Swap(ref char a, ref char b)
    {
      if (a == b)
        return;
      a ^= b;
      b ^= a;
      a ^= b;
    }
  }

  static IEnumerable<char> MoveToKey(Point from, Point to)
  {
    var xDiff = to.X - from.X;
    // Firstly, move horizontal
    while (xDiff != 0)
    {
      if (xDiff < 0)
      {
        yield return '<';
        xDiff++;
      }
      else
      {
        yield return '>';
        xDiff--;
      }
    }

    // Then, move vertical
    var yDiff = to.Y - from.Y;
    while (yDiff != 0)
    {
      if (yDiff < 0)
      {
        yield return '^';
        yDiff++;
      }
      else
      {
        yield return 'v';
        yDiff--;
      }
    }
  }

  static Point GetNumericKeyPosition(char key) =>
    key switch
    {
      '7' => new(0, 0),
      '8' => new(1, 0),
      '9' => new(2, 0),
      '4' => new(0, 1),
      '5' => new(1, 1),
      '6' => new(2, 1),
      '1' => new(0, 2),
      '2' => new(1, 2),
      '3' => new(2, 2),
      '0' => new(1, 3),
      'A' => new(2, 3),
      _ => throw new ArgumentOutOfRangeException(nameof(key), key, null),
    };

  static Point GetDirectionalKeyPosition(char key) =>
    key switch
    {
      '^' => new(1, 0),
      'A' => new(2, 0),
      '<' => new(0, 1),
      'v' => new(1, 1),
      '>' => new(2, 1),
      _ => throw new ArgumentOutOfRangeException(nameof(key), key, null),
    };
}
