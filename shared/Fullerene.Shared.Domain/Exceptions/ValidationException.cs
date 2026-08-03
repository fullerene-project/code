namespace Fullerene.Shared.Domain.Exceptions;

public class ValidationException : FullereneException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; init; }

    public ValidationException(
        IReadOnlyDictionary<string, string[]> errors, 
        string message = "Validation exception") : base(message)
    {
        Errors = errors;
    }

    public static ValidationException FromSingleError(string propertyName, string errorMessage)
    {
        return new ValidationException(
            new Dictionary<string, string[]>
            {
                [propertyName] = [errorMessage]
            },
            $"{propertyName}: {errorMessage}");
    }
}