using System.Diagnostics;
using Common;

namespace Day21;

class LegacySolver(int Layers) : ISolver<IEnumerable<DoorCode>, int>
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
      var sequence = LegacyKeypad.PushKeyTheBestWay(position, newPosition, gap);

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
        var newSubSequence = LegacyKeypad.PushKeyTheBestWay(position, newPosition, gap);
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
