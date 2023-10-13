namespace Shared.Exceptions;
public class ServiceException : BaseException
{
    public ServiceException(object errors) : base(errors)
    {
    }

    public ServiceException(string message) : base(message)
    {
    }
}