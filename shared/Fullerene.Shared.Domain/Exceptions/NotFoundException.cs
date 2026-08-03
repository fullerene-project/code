namespace Fullerene.Shared.Domain.Exceptions;

public class NotFoundException(string message) : FullereneException(message);