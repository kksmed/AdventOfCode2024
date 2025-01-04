using System.Diagnostics;
using Common;

namespace Day11;

class SolverAlternativeCache(int Blinks) : ISolver<int[], long>
{
    static readonly Dictionary<long, List<long[]>> cache = new();

    public long Solve(int[] data)
    {
        var count = 0L;
        foreach (var engraving in data)
        {
            var sw = Stopwatch.StartNew();
            var subCount = Blink(engraving, Blinks).LongLength;
            Console.WriteLine($"{engraving}: Count: {subCount} in {sw.Elapsed}");
            count += subCount;
        }
        throw new InvalidOperationException($"Count: {count}");
        //return count;
    }

    long[] Blink(long engraving, int blinks)
    {
        if (blinks == 0)
            return [engraving];

        if (!cache.TryGetValue(engraving, out var transformations))
        {
            transformations =
            [
                [engraving],
            ];
            cache[engraving] = transformations;
        }

        if (transformations.Count > blinks)
        {
            Console.WriteLine($"Cache hit: {engraving} blinks {blinks} -> {transformations.Count}");
            return transformations[blinks];
        }

        if (blinks == 1)
        {
            var next = Blink(engraving);
            transformations.Add(next);
            return next;
        }

        var stones = transformations.Last();
        for (var b = transformations.Count; b <= blinks; b++)
        {
            var newStones = stones.SelectMany(x => Blink(x, 1)).ToArray();
            transformations.Add(newStones);
            stones = newStones;
        }

        return stones;
    }

    static long[] Blink(long stone)
    {
        if (stone == 0)
        {
            return [1];
        }

        var engraving = stone.ToString();
        var digits = engraving.Length;
        if (digits % 2 == 0)
        {
            var half = digits / 2;
            return [long.Parse(engraving[..half]), long.Parse(engraving[half..])];
        }

        return [stone * 2024];
    }
}
