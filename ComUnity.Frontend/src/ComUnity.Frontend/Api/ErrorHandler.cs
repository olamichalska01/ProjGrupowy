using ComUnity.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using MudBlazor;

namespace ComUnity.Frontend.Api;

public class ErrorHandler
{
    private readonly ISnackbar _snackbar;
    private readonly AuthenticationStateProvider _authenticationStateProvider;

    public ErrorHandler(ISnackbar snackbar, AuthenticationStateProvider authenticationStateProvider)
    {
        _authenticationStateProvider = authenticationStateProvider;
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
                    await (_authenticationStateProvider as CookieAuthenticationStateProvider)!.ClearIdentity();
                    break;
                default:
                    throw;
            }
        }
        return (false, default);
    }

    public async Task<bool> ExecuteWithErrorHandling(Func<Task> func)
    {
        try
        {
            await func();
            return true;
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
        return false;
    }
}
