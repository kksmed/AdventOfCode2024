namespace Common;

public interface ISolver<T>
{
  T Parse(string[] input);
  int SolveFirst(T data);
  int? SolveSecond(T data);
}