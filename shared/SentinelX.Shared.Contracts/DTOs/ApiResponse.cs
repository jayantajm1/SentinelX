namespace SentinelX.Shared.Contracts.DTOs;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; }

    public ApiResponse(T data, string message = "Success")
    {
        Success = true;
        Data = data;
        Message = message;
        Timestamp = DateTime.UtcNow;
    }

    public ApiResponse(string message, string errorCode = "ERROR")
    {
        Success = false;
        Message = message;
        ErrorCode = errorCode;
        Timestamp = DateTime.UtcNow;
    }

    public static ApiResponse<T> CreateSuccess(T data, string message = "Success") 
        => new(data, message);

    public static ApiResponse<T> CreateFailure(string message, string errorCode = "ERROR") 
        => new(message, errorCode);
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }

    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
