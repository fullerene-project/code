namespace Fullerene.Manager.Application.Exceptions;

public sealed class InvalidBuildResultEntryException : Exception
{
    public InvalidBuildResultEntryException(string message) : base(message) { }

    public static InvalidBuildResultEntryException RequiredValueNull(string propertyName)
    {
        return new InvalidBuildResultEntryException($"Required value: \"{propertyName}\" is null");
    }
}