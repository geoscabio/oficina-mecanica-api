namespace OficinaMecanica.Application.Common.Exceptions;

public sealed class ValidationException : ApplicationException
{
    public ValidationException(string message)
        : base(message)
    {
    }
}
