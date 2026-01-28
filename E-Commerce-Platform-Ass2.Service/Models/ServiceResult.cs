namespace E_Commerce_Platform_Ass2.Service.Models
{
    public class ServiceResult
    {
        public bool IsSuccess { get; protected set; }
        public string? ErrorMessage { get; protected set; }
        public List<string> Errors { get; protected set; } = new();

        protected ServiceResult(bool isSuccess, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                Errors.Add(errorMessage);
            }
        }

        protected ServiceResult(bool isSuccess, List<string> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors ?? new List<string>();
            ErrorMessage = errors?.FirstOrDefault();
        }

        public static ServiceResult Success()
        {
            return new ServiceResult(true);
        }

        public static ServiceResult Failure(string errorMessage)
        {
            return new ServiceResult(false, errorMessage);
        }

        public static ServiceResult Failure(List<string> errors)
        {
            return new ServiceResult(false, errors);
        }
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        private ServiceResult(bool isSuccess, T? data = default, string? errorMessage = null)
            : base(isSuccess, errorMessage)
        {
            Data = data;
        }

        private ServiceResult(bool isSuccess, T? data, List<string> errors)
            : base(isSuccess, errors)
        {
            Data = data;
        }

        public static ServiceResult<T> Success(T data)
        {
            return new ServiceResult<T>(true, data);
        }

        public static new ServiceResult<T> Failure(string errorMessage)
        {
            return new ServiceResult<T>(false, default, errorMessage);
        }

        public static new ServiceResult<T> Failure(List<string> errors)
        {
            return new ServiceResult<T>(false, default, errors);
        }
    }
}
