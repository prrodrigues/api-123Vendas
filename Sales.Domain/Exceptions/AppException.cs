using System;

namespace Sales.Domain.Exceptions;

public class AppException : Exception
{
    public string ErrorCode { get; }

    public AppException(string errorCode, string message) : base(message)
    {
        ErrorCode = errorCode;
    }
}
