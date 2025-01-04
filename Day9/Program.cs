using Common;

var example = "2333133121414131402";

Solving.Go(example, new Parser(), new Solver(), new Solver2());

class Parser : IParser<int?[]>
{
    public int?[] Parse(string[] input) =>
        input
            .SelectMany(x => x)
            .Select(x => x - '0')
            .SelectMany((x, i) => Enumerable.Repeat(i % 2 == 0 ? i / 2 : (int?)null, x))
            .ToArray();
}

class Solver : ISolver<int?[], long>
{
    public long Solve(int?[] input) => GetCheckSum(Defragment(input));

    static long GetCheckSum(int?[] disk) => disk.Select((x, i) => (x ?? 0L) * i).Sum();

    protected virtual int?[] Defragment(int?[] disk)
    {
        var result = (int?[])disk.Clone();
        var i = 0;
        var j = disk.Length - 1;
        i = NextSpace();
        j = NextFragment();
        while (i < j && i >= 0 && j >= 0)
        {
            result[i] = result[j];
            result[j] = null;
            i = NextSpace();
            j = NextFragment();
        }

        return result;

        int NextSpace() => Array.FindIndex(result, i, j - i, x => x == null);
        int NextFragment() => Array.FindLastIndex(result, j, j - i, x => x != null);
    }
}

class Solver2 : Solver
{
    protected override int?[] Defragment(int?[] disk)
    {
        var result = (int?[])disk.Clone();
        var i = 0;
        var j = disk.Length - 1;
        i = NextSpace();
        var file = NextFile();
        while (file is { Position: >= 0, Size: > 0 })
        {
            var space = FindSpace(file.Size);
            if (space >= 0)
            {
                for (var k = 0; k < file.Size; k++)
                {
                    result[space + k] = result[file.Position + k];
                    result[file.Position + k] = null;
                }
                if (space == i)
                    i = NextSpace();
            }

            file = NextFile();
        }

        return result;

        int NextSpace() => Array.FindIndex(result, i, j - i, x => x == null);

        int FindSpace(int size)
        {
            if (i < 0 || j < 0)
                return -1;

            for (
                var spaceStart = i;
                0 <= spaceStart;
                spaceStart = Array.FindIndex(result, spaceStart, j - spaceStart, x => x == null)
            )
            {
                if (Enumerable.Range(spaceStart, size).All(k => result[k] == null))
                    return spaceStart;

                spaceStart++;
            }

            return -1;
        }

        (int Position, int Size) NextFile()
        {
            if (i < 0 || j < 0)
                return (-1, 0);
            var endPosition = Array.FindLastIndex(result, j, j - i, x => x != null);
            if (endPosition < 0)
                return (-1, 0);

            var fileId = result[endPosition];
            var beforePosition = Array.FindLastIndex(
                result,
                endPosition,
                endPosition - i,
                x => x != fileId
            );
            j = beforePosition;
            return (beforePosition + 1, endPosition - beforePosition);
        }
    }
}
