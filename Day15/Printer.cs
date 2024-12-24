namespace Day15;

static class Printer
{
  static char ToChar(Direction d) => d switch
  {
    Direction.Up => '^',
    Direction.Right => '>',
    Direction.Down => 'v',
    Direction.Left => '<',
    _ => throw new ArgumentOutOfRangeException()
  };

  public static void Print(Element[,] warehouse)
  {
    var horizontalLine = Enumerable.Range(0, warehouse.GetLength(0) + 2).Select(x => '#').ToArray();
    Console.WriteLine(horizontalLine);
    for (var y = 0; y < warehouse.GetLength(1); y++)
    {
      Console.Write('#');
      for (var x = 0; x < warehouse.GetLength(0); x++)
      {
        var element = warehouse[x, y];
        Console.Write(element switch
        {
          Element.Empty => '.',
          Element.Box => 'O',
          Element.Wall => '#',
          Element.Robot => '@',
          _ => throw new ArgumentOutOfRangeException()
        });
      }
      Console.WriteLine('#');
    }
    Console.WriteLine(horizontalLine);
    Console.WriteLine("");
  }
}