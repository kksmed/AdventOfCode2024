namespace Common;

public class IntMapParser : IParser<int[,]>
{
    public int[,] Parse(string[] input)
    {
        var data = new int[input[0].Length, input.Length];
        for (var y = 0; y < input.Length; y++)
        for (var x = 0; x < input[y].Length; x++)
            data[x, y] = input[y][x] - '0';

        return data;
    }
}
