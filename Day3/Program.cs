using Common;

using Day3;

var example = """
              xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))
              """;

Solving.Go1(example, new Parser(), new Solver());

Console.WriteLine("### PART 2 ###");
var solver2 = new Solver2();
Solving.Go1("xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))", solver2, solver2);
