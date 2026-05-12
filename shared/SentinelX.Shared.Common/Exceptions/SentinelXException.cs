namespace SentinelX.Shared.Common.Exceptions;

public class SentinelXException : Exception
{
    public string? ErrorCode { get; }

    public SentinelXException(string message, string? errorCode = null, Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}

public class EntityNotFoundException : SentinelXException
{
    public EntityNotFoundException(string entityName, object key)
        : base($"{entityName} with ID {key} not found.", "ENTITY_NOT_FOUND") { }
}

public class ValidationException : SentinelXException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException(IDictionary<string, string[]> errors)
        : base("One or more validation failures have occurred.", "VALIDATION_FAILED")
    {
        Errors = errors;
    }
}

public class UnauthorizedAccessException : SentinelXException
{
    public UnauthorizedAccessException(string message = "Unauthorized access.")
        : base(message, "UNAUTHORIZED") { }
}

public class ForbiddenAccessException : SentinelXException
{
    public ForbiddenAccessException(string message = "Forbidden access.")
        : base(message, "FORBIDDEN") { }
}
