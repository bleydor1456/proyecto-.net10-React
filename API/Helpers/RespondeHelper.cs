namespace CrudNet10.Helpers;

public static class ResponseHelper
{
    public static ApiResponse<T> Success<T>(T data, string message)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponseSimple Success(string message)
    {
        return new ApiResponseSimple
        {
            Success = true,
            Message = message
        };
    }
}