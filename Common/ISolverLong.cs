namespace Common;

public interface ISolverLong<T>
{
  T Parse(string[] input);
  long SolveFirst(T data);
  long? SolveSecond(T data);
}
