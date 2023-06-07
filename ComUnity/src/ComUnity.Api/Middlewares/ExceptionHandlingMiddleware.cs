using ComUnity.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ComUnity.Api.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly IDictionary<Type, Action<HttpContext, Exception>> _exceptionHandlers;

        public ExceptionHandlingMiddleware()
        {
            _exceptionHandlers = new Dictionary<Type, Action<HttpContext, Exception>>
            {
                {typeof(ValidationException), HandleValidationException},
                {typeof(BusinessRuleException), HandleBusinessRuleException},
                {typeof(NotFoundException), HandleNotFoundException},
                {typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException},
                {typeof(ForbiddenAccessException), HandleForbiddenAccessException}
            };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (Exception ex)
            {
                HandleException(context, ex);
            }
        }

        private void HandleException(HttpContext context, Exception exception)
        {
            Type type = exception.GetType();
            if (_exceptionHandlers.ContainsKey(type))
            {
                _exceptionHandlers[type].Invoke(context, exception);
                return;
            }

            var baseClass = type.BaseType;
            if (baseClass is not null && _exceptionHandlers.ContainsKey(baseClass))
            {
                _exceptionHandlers[baseClass].Invoke(context, exception);
                return;
            }

            HandleUnknownException(context, exception);
        }

        private void HandleValidationException(HttpContext context, Exception ex)
        {
            ValidationException? exception = ex as ValidationException;

            ValidationProblemDetails details = new(exception!.Errors)
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.WriteAsJsonAsync(details);
        }

        private void HandleBusinessRuleException(HttpContext context, Exception ex)
        {
            BusinessRuleException? exception = ex as BusinessRuleException;

            ProblemDetails details = new()
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                Title = "Request could not be processed due to buisness rules validation",
                Detail = exception!.Message
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
            context.Response.WriteAsJsonAsync(details);
        }

        private void HandleNotFoundException(HttpContext context, Exception ex)
        {
            NotFoundException? exception = ex as NotFoundException;

            ProblemDetails details = new()
            {
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                Title = "The specified resource was not found.",
                Detail = exception!.Message
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            context.Response.WriteAsJsonAsync(details);
        }

        private void HandleUnauthorizedAccessException(HttpContext context, Exception ex)
        {
            ProblemDetails details = new()
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "Unauthorized",
                Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                Detail = "Unauthorized"
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.WriteAsJsonAsync(details);
        }

        private void HandleForbiddenAccessException(HttpContext context, Exception ex)
        {
            ProblemDetails details = new()
            {
                Status = StatusCodes.Status403Forbidden,
                Title = "Forbidden",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                Detail = "You do not have sufficient permissions."
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.WriteAsJsonAsync(details);
        }

        private void HandleUnknownException(HttpContext context, Exception ex)
        {
            ProblemDetails details = new()
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred while processing your request.",
                Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                Detail = "Internal server error."
            };

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.WriteAsJsonAsync(details);
        }
    }
}
