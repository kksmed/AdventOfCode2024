using System.Diagnostics;

namespace Common;

public static class Solving
{
    public static (TData? Example, TData? Input) Go<TData>(
        string? example,
        IParser<TData> parser,
        bool alsoParseInput = false
    )
        where TData : class
    {
        TData? parsedExample = null;
        if (example != null)
            parsedExample = parser.Parse(example.Split(Environment.NewLine));

        TData? parsedInput = null;
        if (alsoParseInput)
            parsedInput = parser.Parse(File.ReadAllLines("input.txt"));

        return (parsedExample, parsedInput);
    }

    public static T1Result? Go<TData, T1Result>(
        string? example,
        IParser<TData> parser,
        ISolver<TData, T1Result> solver,
        bool solveInput = true
    )
        where TData : class
    {
        PrintDay(solver);
        var (parsedExample, parsedInput) = Go(example, parser, solveInput);
        return Solve(solver, parsedExample, parsedInput);
    }

    public static (T1Result? Part1, T2Result? Part2) Go<TData, T1Result, T2Result>(
        string? example,
        IParser<TData> parser,
        ISolver<TData, T1Result>? solverPart1 = null,
        ISolver<TData, T2Result>? solverPart2 = null
    )
        where TData : class
    {
        PrintDay(solverPart2 as object ?? solverPart1 as object ?? parser);
        var (parsedExample, parsedInput) = Go(example, parser, true);

        T1Result? resultPart1 = default(T1Result?);
        if (solverPart1 is not null)
        {
            Console.WriteLine("## Part 1 ##");
            resultPart1 = Solve(solverPart1, parsedExample, parsedInput);
        }

        T2Result? resultPart2 = default(T2Result?);
        if (solverPart2 is not null)
        {
            Console.WriteLine("## Part 2 ##");
            resultPart2 = Solve(solverPart2, parsedExample, parsedInput);
        }

        return (resultPart1, resultPart2);
    }

    static T1Result? Solve<TData, T1Result>(
        ISolver<TData, T1Result> solver,
        TData? parsedExample,
        TData? parsedInput
    )
    {
        T1Result? result = default(T1Result?);
        if (parsedExample is not null)
        {
            var sw = Stopwatch.StartNew();
            result = solver.Solve(parsedExample);
            sw.Stop();
            Console.WriteLine($"# Example: {result}");
            Console.WriteLine($"Evaluated in: {sw.Elapsed}");
        }

        if (parsedInput is not null)
        {
            var sw = Stopwatch.StartNew();
            result = solver.Solve(parsedInput);
            Console.WriteLine($"# Answer: {result}");
            Console.WriteLine($"Evaluated in: {sw.Elapsed}");
        }

        return result;
    }

    static void PrintDay(object o)
    {
        var definingType = o.GetType();
        Console.WriteLine(
            $"### {definingType.Namespace ?? definingType.Assembly.GetName().Name} ###"
        );
    }
}
