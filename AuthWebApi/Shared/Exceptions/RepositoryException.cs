namespace Shared.Exceptions;
public class RepositoryException : BaseException
{
    public RepositoryException(string message) : base(message)
    {
    }

    public RepositoryException(object errors) : base(errors)
    {
    }

    public RepositoryException() : base("Sorry, we encountered an issue while processing your request")
    {
    }
}