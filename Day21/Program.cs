using System.Diagnostics;
using System.Drawing;
using Common;

var example = """
  029A
  980A
  179A
  456A
  379A
  """;

var gap = DirectionalKeypad.Gap.GetPosition();

// foreach (var from in Enum.GetValues<DirectionalKeypad>().Where(x => x != DirectionalKeypad.Gap))
// foreach (var to in Enum.GetValues<DirectionalKeypad>().Where(x => x != DirectionalKeypad.Gap && x != from))
// {
//   var sw = Stopwatch.StartNew();
//   Console.WriteLine($"Best way between: {from} - {to}");
//   var best = Keypad.PushKeyTheBestWay(from.GetPosition(), to.GetPosition(), gap);
//   Console.WriteLine(string.Join(", ", best));
//   Console.WriteLine($"In: {sw.Elapsed}");
// }

var best = Keypad.PushKeyTheBestWay(DirectionalKeypad.Down.GetPosition(), DirectionalKeypad.A.GetPosition(), gap);

// Solving.Go(example, new Parser(), new Solver(2));
//
// Solving.Go(null, new Parser(), new Solver(25));

class Parser : IParser<IEnumerable<DoorCode>>
{
  public IEnumerable<DoorCode> Parse(string[] input) =>
    input.Select(s => new DoorCode(s.Select(ConvertChar).ToArray(), int.Parse(s[..3])));

  static NumericKeypad ConvertChar(char c) =>
    c switch
    {
      '1' => NumericKeypad.One,
      '2' => NumericKeypad.Two,
      '3' => NumericKeypad.Three,
      '4' => NumericKeypad.Four,
      '5' => NumericKeypad.Five,
      '6' => NumericKeypad.Six,
      '7' => NumericKeypad.Seven,
      '8' => NumericKeypad.Eight,
      '9' => NumericKeypad.Nine,
      '0' => NumericKeypad.Zero,
      'A' => NumericKeypad.A,
      _ => throw new ArgumentOutOfRangeException(nameof(c), c, "Unknown character"),
    };
}

class Solver(int Layers) : ISolver<IEnumerable<DoorCode>, int>
{
  public int Solve(IEnumerable<DoorCode> data)
  {
    var complexitySum = 0;
    foreach (var doorCode in data)
    {
      var directions = ConvertToDirections(doorCode.Buttons).ToList();
      // Console.WriteLine($"{Extensions.ToString(doorCode.Buttons)}: {Extensions.ToString(directions)}");
      var complexity = doorCode.Code * directions.Count;
      complexitySum += complexity;
      Console.WriteLine($"Complexity ({directions.Count}x{doorCode.Code}): {complexity}");
    }

    return complexitySum;
  }

  IEnumerable<DirectionalKeypad> ConvertToDirections(NumericKeypad[] doorCode)
  {
    var position = NumericKeypad.A.GetPosition();
    var gap = NumericKeypad.Gap.GetPosition();
    List<DirectionalKeypad> solution = [];
    foreach (var keyToPush in doorCode)
    {
      var sw = Stopwatch.StartNew();

      var newPosition = keyToPush.GetPosition();
      var sequence = Keypad.PushKeyTheBestWay(position, newPosition, gap);

      solution.AddRange(LayerSequence(sequence, Layers));
      Console.WriteLine($"Found solution for: {keyToPush} in {sw.Elapsed}");

      position = newPosition;
    }

    return solution;
  }

  static List<DirectionalKeypad> LayerSequence(DirectionalKeypad[] sequence, int layers)
  {
    if (sequence.Length == 0)
      throw new ArgumentOutOfRangeException(nameof(sequence), sequence, "Must have at least one!");

    var gap = DirectionalKeypad.Gap.GetPosition();
    List<DirectionalKeypad> currentSequence = [.. sequence];
    for (var i = 0; i < layers; i++)
    {
      List<DirectionalKeypad> newSequence = [];
      var position = DirectionalKeypad.A.GetPosition();
      foreach (var keyToPush in currentSequence)
      {
        var newPosition = keyToPush.GetPosition();
        var newSubSequence = Keypad.PushKeyTheBestWay(position, newPosition, gap);
        newSequence.AddRange(newSubSequence);
        position = newPosition;
      }

      currentSequence = newSequence;
    }

    if (currentSequence.Count == 0)
      throw new InvalidOperationException("Cannot layer!");

    return currentSequence;
  }
}

static class Keypad
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

static class Extensions
{
  public static string ToString(this IEnumerable<NumericKeypad> keys) => string.Concat(keys.Select(ToChar));

  public static string ToString(this IEnumerable<DirectionalKeypad> keys) => string.Concat(keys.Select(ToChar));

  static char ToChar(NumericKeypad b) =>
    b switch
    {
      NumericKeypad.A => 'A',
      NumericKeypad.Zero => '0',
      NumericKeypad.One => '1',
      NumericKeypad.Two => '2',
      NumericKeypad.Three => '3',
      NumericKeypad.Four => '4',
      NumericKeypad.Five => '5',
      NumericKeypad.Six => '6',
      NumericKeypad.Seven => '7',
      NumericKeypad.Eight => '8',
      NumericKeypad.Nine => '9',
      NumericKeypad.Gap => throw new InvalidOperationException("Should never be used!"),
      _ => throw new ArgumentOutOfRangeException(nameof(b), b, null),
    };

  static char ToChar(DirectionalKeypad b) =>
    b switch
    {
      DirectionalKeypad.A => 'A',
      DirectionalKeypad.Up => '^',
      DirectionalKeypad.Down => 'v',
      DirectionalKeypad.Left => '<',
      DirectionalKeypad.Right => '>',
      DirectionalKeypad.Gap => throw new InvalidOperationException("Should never be used!"),
      _ => throw new ArgumentOutOfRangeException(nameof(b), b, null),
    };

  public static Point GetPosition(this NumericKeypad b) =>
    b switch
    {
      NumericKeypad.Seven => new(0, 0),
      NumericKeypad.Eight => new(1, 0),
      NumericKeypad.Nine => new(2, 0),
      NumericKeypad.Four => new(0, 1),
      NumericKeypad.Five => new(1, 1),
      NumericKeypad.Six => new(2, 1),
      NumericKeypad.One => new(0, 2),
      NumericKeypad.Two => new(1, 2),
      NumericKeypad.Three => new(2, 2),
      NumericKeypad.Gap => new(0, 3),
      NumericKeypad.Zero => new(1, 3),
      NumericKeypad.A => new(2, 3),
      _ => throw new ArgumentOutOfRangeException(nameof(b), b, null),
    };

  public static Point GetPosition(this DirectionalKeypad b) =>
    b switch
    {
      DirectionalKeypad.Gap => new(0, 0),
      DirectionalKeypad.Up => new(1, 0),
      DirectionalKeypad.A => new(2, 0),
      DirectionalKeypad.Left => new(0, 1),
      DirectionalKeypad.Down => new(1, 1),
      DirectionalKeypad.Right => new(2, 1),
      _ => throw new ArgumentOutOfRangeException(nameof(b), b, null),
    };
}

record DoorCode(NumericKeypad[] Buttons, int Code);

enum NumericKeypad
{
  Seven = 7,
  Eight = 8,
  Nine = 9,
  Four = 4,
  Five = 5,
  Six = 6,
  One = 1,
  Two = 2,
  Three = 3,
  Gap = -1, // Maybe not needed
  Zero = 0,
  A = 10,
};

enum DirectionalKeypad
{
  Gap, // Maybe not needed
  Up,
  A,
  Left,
  Down,
  Right,
}
