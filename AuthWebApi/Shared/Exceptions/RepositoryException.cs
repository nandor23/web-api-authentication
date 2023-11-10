namespace Shared.Exceptions;
public class RepositoryException : Exception
{
    public RepositoryException() : base("Sorry, we encountered an issue while processing your request")
    { }
}