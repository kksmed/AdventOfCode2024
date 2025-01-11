using Common;

namespace Day21;

class LegacyParser : IParser<IEnumerable<DoorCode>>
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
