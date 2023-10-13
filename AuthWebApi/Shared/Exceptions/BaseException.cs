namespace Shared.Exceptions;
public class BaseException : Exception
{
    public object Errors { get; set; } = null!;

    public BaseException()
    {
    }

    public BaseException(string message)
    {
        Errors = new { Message = message };
    }

    public BaseException(object errors)
    {
        Errors = errors;
    }
}