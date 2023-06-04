using Microsoft.AspNetCore.Http;
using MudBlazor;

namespace ComUnity.Frontend.Api;

public class ErrorHandler
{
    private ISnackbar _snackbar;

    public ErrorHandler(ISnackbar snackbar)
    {
        _snackbar = snackbar;
    }

    public async Task<(bool Success, TResult? Result)> ExecuteWithErrorHandling<TResult>(Func<Task<TResult>> func)
    {
        try
        {
            return (true, await func());
        }
        catch (SwaggerException<ValidationProblemDetails> exception)
        {
            _snackbar.Add(string.Join("\n", exception.Result.Errors.Select(x => $"{x.Key} {x.Value}").ToArray()), Severity.Error);
        }
        catch (SwaggerException<ProblemDetails> exception)
        {
            switch (exception.StatusCode)
            {
                case StatusCodes.Status400BadRequest:
                case StatusCodes.Status403Forbidden:
                case StatusCodes.Status422UnprocessableEntity:
                case StatusCodes.Status500InternalServerError:
                    _snackbar.Add(exception.Result.Detail, Severity.Error);
                    break;
                case StatusCodes.Status401Unauthorized:
                    break;
                default:
                    throw;
            }
        }
        return (false, default);
    }
}
