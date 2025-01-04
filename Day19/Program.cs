using System.Diagnostics;
using System.Text.RegularExpressions;
using Common;

var example = """
    r, wr, b, g, bwu, rb, gb, br

    brwrr
    bggr
    gbbr
    rrbgbr
    ubwu
    bwurrg
    brgr
    bbrgwb
    """;

// Solving.Go(example, new Parser(), new SolverRegEx());
//
// Solving.Go(example, new Parser(), new SolverTree());

Solving.Go(example, new Parser(), new SolverTreeAndCache());

class Parser : IParser<Data>
{
    public Data Parse(string[] input)
    {
        return new(input[0].Split(", "), input.Skip(2).ToArray());
    }
}

class SolverRegEx : ISolver<Data, int>
{
    public int Solve(Data data)
    {
        var timeout = TimeSpan.FromSeconds(1);
        var regex = new Regex(
            "^(" + string.Join("|", data.Towels.Select(Regex.Escape)) + ")+$",
            RegexOptions.IgnoreCase,
            timeout
        );
        var count = 0;
        foreach (var p in data.Patterns)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                var isMatch = regex.IsMatch(p);
                if (isMatch)
                    count++;
                sw.Stop();
                Console.WriteLine($"{p} - {isMatch} - in {sw.Elapsed} (ticks: {sw.ElapsedTicks})");
            }
            catch (RegexMatchTimeoutException)
            {
                Console.WriteLine($"{p} - Timed out (after {timeout})");
            }
        }

        return count;
    }
}

class SolverTreeAndCache : ISolver<Data, long>
{
    public long Solve(Data data)
    {
        Dictionary<string, long> cache = new();
        var tree = BuildTree(data.Towels);
        var count = 0L;
        foreach (var p in data.Patterns)
        {
            var sw = Stopwatch.StartNew();
            var matches = FindMatches(cache, tree, p);
            count += matches;
            sw.Stop();
            Console.WriteLine($"{p} - {matches} - in {sw.Elapsed} (ticks: {sw.ElapsedTicks})");
        }

        return count;
    }

    static TowelNode BuildTree(string[] towels)
    {
        var swTreeBuilding = Stopwatch.StartNew();
        TowelNode tree = new(' ');
        foreach (var towel in towels)
        {
            var node = tree;
            foreach (var c in towel)
            {
                node = c switch
                {
                    'w' => node.White ??= new(c),
                    'u' => node.Blue ??= new(c),
                    'b' => node.Black ??= new(c),
                    'r' => node.Red ??= new(c),
                    'g' => node.Green ??= new(c),
                    _ => throw new InvalidOperationException(),
                };
            }
            if (node.Towel != null)
                throw new InvalidOperationException("Duplicate towel!");
            node.Towel = towel;
        }
        swTreeBuilding.Stop();
        Console.WriteLine(
            $"Tree built in {swTreeBuilding.Elapsed} (ticks: {swTreeBuilding.ElapsedTicks})"
        );

        return tree;
    }

    static long FindMatches(Dictionary<string, long> cache, TowelNode towels, string pattern)
    {
        if (pattern.Length == 0)
            return 1;

        if (cache.TryGetValue(pattern, out var count))
            return count;

        count = 0L;
        var node = towels;
        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];
            var newNode = c switch
            {
                'w' => node.White,
                'u' => node.Blue,
                'b' => node.Black,
                'r' => node.Red,
                'g' => node.Green,
                _ => throw new InvalidOperationException(),
            };

            if (newNode == null)
            {
                cache[pattern] = count;
                return count;
            }

            node = newNode;
            if (node.Towel != null)
            {
                count += FindMatches(cache, towels, pattern[(i + 1)..]);
            }
        }

        cache[pattern] = count;
        return count;
    }
}

class SolverCache : ISolver<Data, int>
{
    public int Solve(Data data)
    {
        // ## Maybe premature optimization:
        // var swSetup = Stopwatch.StartNew();
        // var rareChar = "wubrg".Except(data.Towels.Where(x => x.Length == 1).Select(x => x[0])).First();
        // var rareCharTowels = data.Towels.Where(x => x.Contains(rareChar)).ToArray();
        // var restTowels = data.Towels.Except(rareCharTowels).ToArray();
        //
        // Console.WriteLine($"Setup in {swSetup.Elapsed} (ticks: {swSetup.ElapsedTicks})");

        Dictionary<string, int> cache = new();

        var count = 0;
        foreach (var p in data.Patterns)
        {
            var sw = Stopwatch.StartNew();
            var matches = FindMatches(cache, data.Towels, p);
            count += matches;
            sw.Stop();
            Console.WriteLine($"{p} - {matches} - in {sw.Elapsed} (ticks: {sw.ElapsedTicks})");
        }

        return count;
    }

    int FindMatches(Dictionary<string, int> cache, string[] towels, string pattern)
    {
        if (pattern.Length == 0)
            return 1;

        if (cache.TryGetValue(pattern, out var count))
            return count;

        count = 0;
        foreach (var towel in towels)
        {
            var i = pattern.IndexOf(towel, StringComparison.Ordinal);
            if (i < 0)
                continue;

            count +=
                FindMatches(cache, towels, pattern[..i])
                * FindMatches(cache, towels, pattern[(i + towel.Length)..]);
        }

        cache[pattern] = count;
        return count;
    }
}

record Data(string[] Towels, string[] Patterns);

record TowelNode(char Color)
{
    public string? Towel { get; set; }
    public TowelNode? White { get; set; } // (w)
    public TowelNode? Blue { get; set; } // (u)
    public TowelNode? Black { get; set; } // (b)
    public TowelNode? Red { get; set; } // (r)
    public TowelNode? Green { get; set; } // (g)
}
