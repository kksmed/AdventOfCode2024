using System.Drawing;

namespace Day21;

static class LegacyKeypad
{
  static readonly Dictionary<(Point From, Point To, Point Gap), DirectionalKeypad[]> cache = new();

  public static DirectionalKeypad[] PushKeyTheBestWay(Point startPosition, Point buttonPosition, Point gapPosition)
  {
    var cacheKey = (startPosition, buttonPosition, gapPosition);
    if (cache.TryGetValue(cacheKey, out var value))
      return value;

    var moves = MoveToAndPush(startPosition, buttonPosition).ToArray();
    var permutations = GetPermutations(moves, 0, moves.Length - 1)
      .Where(x => AvoidGap(x, startPosition, gapPosition))
      .Select(x => x.Append(DirectionalKeypad.A).ToArray())
      .ToList();
    if (permutations.Count == 0)
      throw new InvalidOperationException($"Cannot move from {startPosition} -> {buttonPosition}");

    var best = FindBestPermutation(permutations).BestPermutation;
    cache[cacheKey] = best;

    return best;
  }

  static (DirectionalKeypad[] BestPermutation, DirectionalKeypad[] FinalSequence, int Layers) FindBestPermutation(
    List<DirectionalKeypad[]> permutations
  )
  {
    const int maxLayering = 4;
    if (permutations.Count == 0)
      throw new ArgumentException("Must be at least one.", nameof(permutations));
    if (permutations.Count == 1)
      return (permutations.Single(), permutations.Single(), 0);

    var layeredPermutations = permutations.Select(x => (Initial: x, Layered: new List<DirectionalKeypad[]>([x]))).ToList();
    var layer = 0;
    for (; layer < maxLayering; layer++)
    {
      for (var i = 0; i < layeredPermutations.Count; i++)
      {
        var p = layeredPermutations[i];

        var newPermutations = p.Layered.SelectMany(LayerSequence).ToList();
        var subMinLength = newPermutations.Select(x => x.Length).Min();

        layeredPermutations[i] = (p.Initial, newPermutations.Where(x => x.Length == subMinLength).ToList());
      }

      var minLength = layeredPermutations.Select(x => x.Layered.First().Length).Min();
      layeredPermutations = layeredPermutations.Where(x => x.Layered.First().Length == minLength).ToList();

      if (layeredPermutations.Count == 1)
      {
        var best = layeredPermutations.Single();
        return (best.Initial, best.Layered.First(), layer);
      }
    }

    //throw new InvalidOperationException("Cannot determine best move");
    Console.WriteLine(
      $"Tied best between {layeredPermutations.Count}: {string.Join("& ", layeredPermutations.Select(x => string.Join(", ", x.Initial)))}"
    );
    Console.WriteLine(
      $"All with minimum length of: {layeredPermutations.First().Layered.First().Length} sequence: {string.Join(", ", layeredPermutations.Select(x => x.Layered.Count))}"
    );
    // TODO: How to pick if more?
    var winningSequence = layeredPermutations.First();
    if (winningSequence.Layered.Count < 1)
      throw new InvalidOperationException("No winning sequence");

    return (winningSequence.Initial, winningSequence.Layered.First(), layer);
  }

  public static IEnumerable<DirectionalKeypad[]> GetPermutations(
    DirectionalKeypad[] elements,
    int recursionDepth,
    int maxDepth
  )
  {
    if (maxDepth < 0)
      return [elements];

    if (recursionDepth == maxDepth)
    {
      return [elements.ToArray()];
    }

    List<DirectionalKeypad[]> permutations = [];
    for (var i = recursionDepth; i <= maxDepth; i++)
    {
      Swap(ref elements[recursionDepth], ref elements[i]);
      permutations.AddRange(GetPermutations(elements, recursionDepth + 1, maxDepth));
      // backtrack
      Swap(ref elements[recursionDepth], ref elements[i]);
    }

    return permutations.DistinctBy(Extensions.ToString);

    static void Swap(ref DirectionalKeypad a, ref DirectionalKeypad b)
    {
      if (a == b)
        return;
      a ^= b;
      b ^= a;
      a ^= b;
    }
  }

  static List<DirectionalKeypad[]> LayerSequence(DirectionalKeypad[] sequence)
  {
    if (sequence.Length == 0)
      throw new ArgumentOutOfRangeException(nameof(sequence), sequence, "Must have at least one!");

    var gap = DirectionalKeypad.Gap.GetPosition();

    List<DirectionalKeypad[]> permutations =
    [
      [],
    ];
    var position = DirectionalKeypad.A.GetPosition();
    foreach (var keyToPush in sequence)
    {
      var newPosition = keyToPush.GetPosition();
      var moves = MoveToAndPush(position, keyToPush.GetPosition()).ToArray();
      var subPermutations = GetPermutations(moves, 0, moves.Length - 1)
        .Where(x => AvoidGap(x, position, gap))
        .Select(x => x.Append(DirectionalKeypad.A))
        .ToList();
      if (subPermutations.Count == 0)
        throw new InvalidOperationException($"Cannot move from {position} -> {newPosition} to push {keyToPush}");

      permutations = permutations.SelectMany(head => subPermutations.Select(tail => head.Concat(tail).ToArray())).ToList();

      position = newPosition;
    }

    if (permutations.Count == 0)
      throw new InvalidOperationException("Cannot layer!");

    var minLength = permutations.MinBy(x => x.Length)?.Length;
    return permutations.Where(x => x.Length == minLength).ToList();
  }

  static bool AvoidGap(IEnumerable<DirectionalKeypad> moves, Point start, Point gap)
  {
    var position = start;
    foreach (var move in moves)
    {
      position = move switch
      {
        DirectionalKeypad.A or DirectionalKeypad.Gap => throw new InvalidOperationException("Should never happen!"),
        DirectionalKeypad.Up => position with { Y = position.Y - 1 },
        DirectionalKeypad.Left => position with { X = position.X - 1 },
        DirectionalKeypad.Down => position with { Y = position.Y + 1 },
        DirectionalKeypad.Right => position with { X = position.X + 1 },
        _ => throw new ArgumentOutOfRangeException(nameof(moves), move, null),
      };

      if (position == gap)
        return false;
    }
    return true;
  }

  static IEnumerable<DirectionalKeypad> MoveToAndPush(Point position, Point buttonPosition)
  {
    var xDiff = buttonPosition.X - position.X;
    var yDiff = buttonPosition.Y - position.Y;

    // Firstly, move horizontal
    while (xDiff != 0)
    {
      if (xDiff < 0)
      {
        yield return DirectionalKeypad.Left;
        xDiff++;
      }
      else
      {
        yield return DirectionalKeypad.Right;
        xDiff--;
      }
    }

    // Then, move vertical
    while (yDiff != 0)
    {
      if (yDiff < 0)
      {
        yield return DirectionalKeypad.Up;
        yDiff++;
      }
      else
      {
        yield return DirectionalKeypad.Down;
        yDiff--;
      }
    }
  }
}
