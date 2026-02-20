using SmartGrid.Domain.Enums;

namespace SmartGrid.Domain.Common
{
    public record Error(string Message, ErrorType Type);
    public class Result
    {
        public bool IsSuccess { get; }
        public Error? Error { get; }
        public bool IsFailure => !IsSuccess;

        protected Result(bool isSuccess, Error? error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new Result(true, null);
        public static Result Failure(string message, ErrorType type = ErrorType.Failure)
            => new Result(false, new Error(message, type));
    }

    public class Result<T> : Result
    {
        public T Value { get; }

        protected Result(T value, bool isSuccess, Error? error)
            : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Success(T value) => new Result<T>(value, true, null);
        public static new Result<T> Failure(string message, ErrorType type = ErrorType.Failure)
                    => new Result<T>(default!, false, new Error(message, type));
    }
}
