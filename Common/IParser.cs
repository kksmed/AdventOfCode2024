namespace Common;

public interface IParser<out T>
{
    T Parse(string[] input);
}
