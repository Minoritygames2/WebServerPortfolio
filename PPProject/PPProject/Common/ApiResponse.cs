namespace PPProject.Common
{
    public class ApiResponse<T>
    {
        public enum ResultStatus
        {
            Success = 0,
            Error   = 1001,
        }

        public ResultStatus Status { get; private set; }
        public T? Data { get; private set; }
        public int ErrorCode { get; private set; }
        public string? ErrorMessage { get; private set; }
        public static ApiResponse<T> Success(T data)
        {
            return new ApiResponse<T>
            {
                Status = ResultStatus.Success,
                Data = data,
                ErrorCode = 0,
                ErrorMessage = null
            };
        }

        public static ApiResponse<T> Error(int errorCode, string errorMessage)
        {
            return new ApiResponse<T>
            {
                Status = ResultStatus.Error,
                Data = default,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }

        public static ApiResponse<T> Error(int errorCode, string errorMessage, T data)
        {
            return new ApiResponse<T>
            {
                Status = ResultStatus.Error,
                Data = data,
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }
    }
}
