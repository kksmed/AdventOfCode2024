namespace Common;

public interface ISolverLegacy<T>
{
  T Parse(string[] input);
  int SolveFirst(T data);
  int? SolveSecond(T data);
}