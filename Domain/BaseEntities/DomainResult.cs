namespace Core.BaseEntities;

public class DomainResult
{
    public bool IsSuccess { get; private set; }
    public string ErrorMessage { get; private set; }
    public List<string> Errors { get; private set; } = new();

    protected DomainResult(bool isSuccess, string errorMessage = null)
    {
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }

    public static DomainResult Success() => new(true);
    public static DomainResult Failure(string error) => new(false, error);
    public static DomainResult Failure(IEnumerable<string> errors) 
    {
        var result = new DomainResult(false);
        result.Errors.AddRange(errors);
        return result;
    }
}

public class DomainResult<T> : DomainResult
{   
    public T Data { get; private set; }

    private DomainResult(bool isSuccess, T data = default, string errorMessage = null) 
        : base(isSuccess, errorMessage)
    {
        Data = data;
    }

    public static DomainResult<T> Success(T data) => new(true, data); 
    public static new DomainResult<T> Failure(IEnumerable<string> errors)
    {
        var result = new DomainResult<T>(false, default, string.Join("; ", errors));
        result.Errors.AddRange(errors);
        return result;
    }

}