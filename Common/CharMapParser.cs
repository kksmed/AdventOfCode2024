namespace Common;

public class CharMapParser : IParser<char[,]>
{
    public char[,] Parse(string[] input)
    {
        var data = new char[input[0].Length, input.Length];
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
            data[x, y] = input[y][x];

        return data;
    }
}
