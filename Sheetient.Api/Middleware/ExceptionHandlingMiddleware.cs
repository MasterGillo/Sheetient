using FluentValidation;
using Sheetient.App.Exceptions;
using System.Net;

namespace Sheetient.Api.Middleware
{
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var request = context.Request;
                await HandleExceptionAsync(context, ex);
                logger.LogError("{exceptionName}" +
                                "\nMessage: {message}" +
                                "\n|{method}| Full URL: {path}{queryString}",
                                ex.GetType().Name, ex.Message, request.Method, request.Path, request.QueryString);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            (HttpStatusCode statusCode, string message) = exception switch
            {
                ArgumentException _
                or ArgumentNullException _
                or ValidationException _ => (HttpStatusCode.BadRequest, exception.Message ?? "Bad request."),
                UnauthorizedException _
                or UnauthorizedAccessException _ => (HttpStatusCode.Unauthorized, exception.Message ?? "Unauthorized."),
                ForbiddenException _ => (HttpStatusCode.Forbidden, exception.Message ?? "Forbidden."),
                NotFoundException _ => (HttpStatusCode.NotFound, exception.Message ?? "Not found."),
                ConflictException _ => (HttpStatusCode.Conflict, exception.Message ?? "Conflict."),
                _ => (HttpStatusCode.InternalServerError, exception.Message ?? "Internal server error. Please retry later.")
            };

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(message);
        }
    }
}
