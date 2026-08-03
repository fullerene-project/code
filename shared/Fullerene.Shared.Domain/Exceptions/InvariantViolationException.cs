namespace Fullerene.Shared.Domain.Exceptions;

public class InvariantViolationException(string message) : FullereneException(message);