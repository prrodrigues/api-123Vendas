using Sales.Application.Common.Correlation;

namespace Sales.Api.Contracts;

public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    T? Data,
    string? CorrelationId)
{
    public static ApiResponse<T> FromSuccess(T data, ICorrelationContextAccessor accessor, string message = "Operação realizada com sucesso")
    {
        return new ApiResponse<T>(true, message, data, accessor.CorrelationId);
    }

    public static ApiResponse<T> FromFailure(string message, ICorrelationContextAccessor accessor)
    {
        return new ApiResponse<T>(false, message, default, accessor.CorrelationId);
    }
}
