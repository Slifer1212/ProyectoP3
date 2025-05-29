namespace Core.BaseEntities;

public class OperationResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public List<string>? Errors { get; private set; }
    public static OperationResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    public static OperationResult<T> Failure(string error) => new() { IsSuccess = false, ErrorMessage = error };
}