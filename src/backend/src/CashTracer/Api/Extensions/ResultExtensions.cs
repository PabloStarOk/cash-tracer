using CashTracer.Domain.Common;

namespace CashTracer.Api.Extensions;

/// <summary>
/// Provides extension methods for converting <see cref="Result{T}"/> instances to <see cref="IResult"/> for use in ASP.NET Core minimal APIs.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// The key used to store the error code in the problem details extensions.
    /// </summary>
    public const string ErrorCodeKey = "code";

    public static IResult ToHttpResult(this Result result, Func<Result, IResult> onSuccess)
    {
        return result.IsSuccess ? onSuccess(result) : CreateResultFromError(result.Error);
    }

    /// <summary>
    /// Converts a <see cref="Result{T}"/> to an <see cref="IResult"/> for use in ASP.NET Core minimal APIs.
    /// </summary>
    /// <typeparam name="T">The type of the value contained in the result.</typeparam>
    /// <param name="result">The result to convert.</param>
    /// <param name="onSuccess">A function that takes a successful <see cref="Result{T}"/> and returns an <see cref="IResult"/>.</param>
    /// <returns>An <see cref="IResult"/> representing the outcome of the operation.</returns>
    public static IResult ToHttpResult<T>(this Result<T> result, Func<Result<T>, IResult> onSuccess)
        where T : notnull
    {
        return result.IsSuccess ? onSuccess(result) : CreateResultFromError(result.Error);
    }

    private static IResult CreateResultFromError(Error error)
    {
        return error.Type switch
        {
            ErrorType.Validation => ValidationProblem(error),
            ErrorType.NotFound => NotFoundProblem(error),
            _ => throw new InvalidOperationException($"Unexpected error type: {error.Type}."),
        };
    }

    private static IResult ValidationProblem(Error error)
    {
        var errors = new Dictionary<string, string[]>
        {
            { error.Code, [error.Message] },
        };

        return Results.ValidationProblem(
            errors,
            statusCode: StatusCodes.Status400BadRequest);
    }

    private static IResult NotFoundProblem(Error error)
    {
        return Results.Problem(
            detail: error.Message,
            statusCode: StatusCodes.Status404NotFound,
            extensions: new Dictionary<string, object?>
            {
                [ErrorCodeKey] = error.Code,
            });
    }
}