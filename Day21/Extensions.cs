using System.Drawing;

namespace Day21;

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
