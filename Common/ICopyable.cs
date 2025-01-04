namespace Common;

public interface ICopyable<out T>
{
    T Copy();
}
