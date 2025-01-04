using System.Drawing;
using Common;

var example = """
  029A
  980A
  179A
  456A
  379A
  """;

Solving.Go(example, new Parser(), new Solver());

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

class Solver : ISolver<IEnumerable<DoorCode>, int>
{
  public int Solve(IEnumerable<DoorCode> data)
  {
    var complexitySum = 0;
    foreach (var doorCode in data)
    {
      var directions = ConvertToDirections(doorCode.Buttons).ToList();
      // Console.WriteLine($"Door code ({doorCode}):");
      // doorCode.Buttons.Print();
      // Console.WriteLine("Directions (1st):");
      // directions.Print();
      directions = LayerDirections(directions).ToList();
      // Console.WriteLine("Directions (2nd):");
      // directions.Print();
      directions = LayerDirections(directions).ToList();
      // Console.WriteLine("Directions (3rd):");
      // directions.Print();

      var complexity = doorCode.Code * directions.Count;
      complexitySum += complexity;
      // Console.WriteLine($"Complexity ({directions.Count}x{doorCode.Code}): {complexity}");
    }

    return complexitySum;
  }

  IEnumerable<DirectionalKeypad> ConvertToDirections(NumericKeypad[] doorCode)
  {
    var position = NumericKeypad.A.GetPosition();
    var gap = NumericKeypad.Gap.GetPosition();
    foreach (var keyToPush in doorCode)
    {
      var newPosition = keyToPush.GetPosition();
      foreach (var b in MoveToAndPush(position, keyToPush.GetPosition(), gap))
      {
        yield return b;
      }

      position = newPosition;
    }
  }

  IEnumerable<DirectionalKeypad> LayerDirections(IEnumerable<DirectionalKeypad> directionalKeypads)
  {
    var position = DirectionalKeypad.A.GetPosition();
    foreach (var keyToPush in directionalKeypads)
    {
      var newPosition = keyToPush.GetPosition();
      foreach (var b in MoveToAndPush(position, keyToPush.GetPosition()))
      {
        yield return b;
      }

      position = newPosition;
    }
  }

  static IEnumerable<DirectionalKeypad> MoveToAndPush(Point position, Point buttonPosition, Point? gap = null)
  {
    gap ??= new(0, 0);
    var xDiff = buttonPosition.X - position.X;
    var yDiff = buttonPosition.Y - position.Y;

    // Firstly, move horizontal if not in same row as "gap"
    // '<' (left) is more demanding then other buttons so we want to do these "together"
    if (position.Y != gap.Value.Y)
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

    // Now, move horizontal in case we moved vertical first
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

    // Lastly, push the button
    yield return DirectionalKeypad.A;
  }
}

static class Extensions
{
  public static void Print(this IEnumerable<NumericKeypad> keys) => Console.WriteLine(keys.Select(ToChar).ToArray());

  public static void Print(this IEnumerable<DirectionalKeypad> keys) => Console.WriteLine(keys.Select(ToChar).ToArray());

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
